const express = require('express');
const path = require('path');

const DEFAULT_PORT = process.env.PORT || 8081;

// initialize express.
const app = express();

// Setup app folders.
app.use(express.static('./'));

// Set up a route for signout.html
app.get('/signout', (req, res) => {
    res.sendFile(path.join(__dirname + '/App/signout.html'));
});

// Set up a route for index.html
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname + '/index.html'));
});

// Start the server.
app.listen(DEFAULT_PORT);
console.log(`Listening on port ${DEFAULT_PORT}...`);
