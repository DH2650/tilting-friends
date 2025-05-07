document.addEventListener('DOMContentLoaded', () => {
    const socket = io(); // Connect to the server (same host and port)
    const connectionStatus = document.getElementById('connectionStatus');
    const controllerIdDisplay = document.getElementById('controllerIdDisplay');
    const playerIdDisplay = document.getElementById('playerIdDisplay');
    const controlsDiv = document.getElementById('controls');
    const roomCodeInputDiv = document.getElementById('roomCodeInput');
    const playerIdInput = document.getElementById('playerIdInput');
    const joinGameButton = document.getElementById('joinGameButton');

    let myControllerId = null;
    let assignedPlayerId = null;

    socket.on('connect', () => {
        myControllerId = socket.id;
        connectionStatus.textContent = 'Connected to server!';
        controllerIdDisplay.textContent = `My Controller ID: ${myControllerId}`;
        // Show room code input or player ID selection
        roomCodeInputDiv.style.display = 'block';
    });

    socket.on('unityGameConnected', () => {
        connectionStatus.textContent = 'Unity Game Connected. Ready to join!';
        // Potentially auto-join or enable join button
    });

    socket.on('unityGameDisconnected', () => {
        connectionStatus.textContent = 'Unity Game Disconnected. Please wait.';
        controlsDiv.style.display = 'none';
        roomCodeInputDiv.style.display = 'block'; // Allow re-joining
    });

    joinGameButton.addEventListener('click', () => {
        const desiredPlayerId = playerIdInput.value.trim();
        if (desiredPlayerId) {
            socket.emit('registerController', { playerId: desiredPlayerId });
        } else {
            // Default to socket ID if no name is entered, or prompt user
            socket.emit('registerController', {});
        }
    });

    socket.on('controllerRegistered', (data) => {
        assignedPlayerId = data.assignedPlayerId;
        playerIdDisplay.textContent = `Assigned Player ID: ${assignedPlayerId}`;
        controlsDiv.style.display = 'block';
        roomCodeInputDiv.style.display = 'none';
        connectionStatus.textContent = 'Controller Registered. You can now play!';
    });

    socket.on('disconnect', () => {
        connectionStatus.textContent = 'Disconnected from server.';
        controlsDiv.style.display = 'none';
        roomCodeInputDiv.style.display = 'block';
    });

    socket.on('error', (data) => {
        connectionStatus.textContent = `Error: ${data.message}`;
    });

    // Example input handling
    document.getElementById('buttonUp').addEventListener('pointerdown', () => sendInput('up_pressed'));
    document.getElementById('buttonUp').addEventListener('pointerup', () => sendInput('up_released'));
    document.getElementById('buttonDown').addEventListener('pointerdown', () => sendInput('down_pressed'));
    document.getElementById('buttonDown').addEventListener('pointerup', () => sendInput('down_released'));
    document.getElementById('buttonLeft').addEventListener('pointerdown', () => sendInput('left_pressed'));
    document.getElementById('buttonLeft').addEventListener('pointerup', () => sendInput('left_released'));
    document.getElementById('buttonRight').addEventListener('pointerdown', () => sendInput('right_pressed'));
    document.getElementById('buttonRight').addEventListener('pointerup', () => sendInput('right_released'));
    document.getElementById('buttonActionA').addEventListener('pointerdown', () => sendInput('actionA_pressed'));
    document.getElementById('buttonActionA').addEventListener('pointerup', () => sendInput('actionA_released'));
    // Add more for touchstart/touchend if needed for mobile specifically

    function sendInput(inputType) {
        if (socket.connected && assignedPlayerId) {
            socket.emit('controllerInput', { input: inputType });
            // Optionally, could also send { playerId: assignedPlayerId, input: inputType } if server needs it explicitly here
        }
    }
});