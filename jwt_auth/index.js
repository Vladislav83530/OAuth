const express = require('express');
const bodyParser = require('body-parser');
const jwt = require('jsonwebtoken');
const path = require('path');
const fs = require('fs');

const app = express();
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const SESSION_KEY = 'Authorization';
const SECRET = 'SECRET';

class Session {
    #sessions = {}

    constructor() {
        try {
            this.#sessions = fs.readFileSync('./sessions.json', 'utf8');
            this.#sessions = JSON.parse(this.#sessions.trim());
        } catch(e) {
            this.#sessions = {};
        }
    }

    #storeSessions() {
        fs.writeFileSync('./sessions.json', JSON.stringify(this.#sessions), 'utf-8');
    }

    set(key, value) {
        if (!value) {
            value = {};
        }
        this.#sessions[key] = value;
        this.#storeSessions();
    }

    get(key) {
        return this.#sessions[key];
    }

    init(res, payload) {
        const token = jwt.sign(payload, SECRET);
        this.set(token, payload);
        
        return token;
    }

    destroy(token) {
        delete this.#sessions[token];
        this.#storeSessions();
    }
}

const sessions = new Session();

app.use((req, res, next) => {
    let currentSession = {};
    let token = req.get(SESSION_KEY);

    if (token) {
        currentSession = sessions.get(token);
        if (!currentSession) {
            currentSession = {};
            token = sessions.init(res, currentSession);
        }
    } else {
        currentSession = {};
        token = sessions.init(res, currentSession);
    }

    req.session = currentSession;
    req.token = token;

    next();
});

app.get('/', (req, res) => {
    if (req.session.username) {
        return res.json({
            username: req.session.username,
            logout: 'http://localhost:3000/logout'
        });
    }
    res.sendFile(path.join(__dirname+'/index.html'));
});

app.get('/logout', (req, res) => {
    sessions.destroy(req.token);
    res.redirect('/');
});

const users = [
    {
        login: 'Login',
        password: 'Password',
        username: 'Username',
    },
    {
        login: 'Login1',
        password: 'Password1',
        username: 'Username1',
    }
];

app.post('/api/login', (req, res) => {
    const { login, password } = req.body;

    const user = users.find((user) => {
        if (user.login == login && user.password == password) {
            return true;
        }
        return false;
    });

    if (user) {
        const payload = { username: user.username, login: user.login };
        const token = sessions.init(res, payload);
        res.json({ token });
    } else {
        res.status(401).send();
    }
});

app.listen(3000, () => {
    console.log('Server running on port 3000');
});