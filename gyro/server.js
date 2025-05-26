// server.js
// A very simple Node.js server using Express to serve a static HTML file.

// Import the Express.js library. Express is a popular web framework for Node.js
// that simplifies building web applications and APIs.
const express = require('express');
// Create an instance of the Express application.
const app = express();
// Define the port on which the server will listen.
const port = 3000;

// Serve static files from the 'public' directory.
// This means any file placed in a folder named 'public' (e.g., public/index.html)
// will be accessible directly via the server (e.g., http://localhost:3000/index.html).
// We will place our mobile frontend HTML file in this 'public' directory.
app.use(express.static('public'));

// Start the server and listen for incoming requests on the specified port.
app.listen(port, () => {
    // Log a message to the console once the server starts successfully.
    console.log(`Server running at http://localhost:${port}`);
    console.log(`Open http://localhost:${port}/index.html in your browser (or mobile device)`);
    console.log(`Make sure to place 'index.html' inside a 'public' folder in the same directory as 'server.js'.`);
});

