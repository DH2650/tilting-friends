const express = require('express');
const http = require('http');
const socketIo = require('socket.io');

const port = process.env.PORT || 3000; // Port for the server

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

let unitySocket = null; // To store the Unity game's socket connection
let playerControllers = {}; // To store player controller sockets, mapping controller ID to player ID

// Serve the HTML controller page (you'll create this later)
app.use(express.static('../frontend')); // Create a 'public' folder for your controller.html

io.on('connection', (socket) => {
    console.log('New client connected:', socket.id);

    // Differentiate between Unity game and web controllers
    socket.on('registerUnityGame', () => {
        console.log('Unity Game registered:', socket.id);
        unitySocket = socket;
        // Potentially, clear old controllers or notify them
        playerControllers = {}; // Reset controllers on new game instance for simplicity
        io.emit('unityGameConnected'); // Notify web clients that the game is ready
    });

    socket.on('registerController', (data) => {
        const playerId = data.playerId; // Use a provided ID or socket ID
        console.log('Controller registered for player:', playerId, 'Socket ID:', socket.id);
        playerControllers[socket.id] = playerId; // Map socket ID to player ID

        console.log("Unity Socket:", unitySocket.id)
        // Notify Unity that a new player (controller) has joined
        if (unitySocket) {
            console.log("Join player", playerId)
            unitySocket.emit('playerJoined', { controllerId: socket.id, assignedPlayerId: playerId });
        } else {
            console.log("Did not join player", playerId)
            socket.emit('error', { message: 'Unity game not connected.' });
        }

        socket.emit('controllerRegistered', { controllerId: socket.id, assignedPlayerId: playerId });
    });

    // Handle input from a web controller
    socket.on('controllerInput', (data) => {
        // data should contain { playerId: "...", input: "..." }
        // Or, if using socket.id as controllerId directly: { input: "..." }
        const playerId = playerControllers[socket.id];
        console.log('Receive input');
        if (unitySocket && playerId) {
            console.log(`Input from controller ${socket.id} (Player ${playerId}):`, data.input);
            unitySocket.emit('controllerInput', { controllerId: socket.id, input: data.input });
        }
    });

    socket.on('disconnect', () => {
        console.log('Client disconnected:', socket.id);
        if (socket.id === unitySocket?.id) {
            console.log('Unity Game disconnected');
            unitySocket = null;
            io.emit('unityGameDisconnected'); // Notify web clients
        } else if (playerControllers[socket.id]) {
            const playerId = playerControllers[socket.id];
            console.log('Controller for player', playerId, 'disconnected');
            if (unitySocket) {
                unitySocket.emit('playerLeft', { playerId: playerId });
            }
            delete playerControllers[socket.id];
        }
    });
});

server.listen(port, () => console.log(`Server listening on port ${port}`));