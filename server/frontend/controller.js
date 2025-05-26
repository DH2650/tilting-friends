// controller.js
document.addEventListener('DOMContentLoaded', () => {
    const socket = io(); // Connect to the server (same host and port)
    const connectionStatus = document.getElementById('connectionStatus');
    const controllerIdDisplay = document.getElementById('controllerIdDisplay');
    const playerIdDisplay = document.getElementById('playerIdDisplay');
    const controlsDiv = document.getElementById('controls');
    const roomCodeInputDiv = document.getElementById('roomCodeInput');
    const playerIdInput = document.getElementById('playerIdInput');
    const joinGameButton = document.getElementById('joinGameButton');

    // Gyroscope specific elements
    const alphaElement = document.getElementById('alpha');
    const betaElement = document.getElementById('beta');
    const gammaElement = document.getElementById('gamma');
    const requestPermissionButton = document.getElementById('requestPermission');
    const messageBox = document.getElementById('messageBox');

    let myControllerId = null;
    let assignedPlayerId = null;
    let gyroDataActive = false; // Flag to indicate if gyroscope data is being sent

    // Gyroscope data sending throttle
    let lastGyroSendTime = 0;
    const GYRO_SEND_INTERVAL = 100; // Send gyro data every 100ms (10 times per second)

    // --- Socket.IO Event Handlers ---
    socket.on('connect', () => {
        myControllerId = socket.id;
        connectionStatus.textContent = 'Connected to server!';
        connectionStatus.classList.remove('error');
        connectionStatus.classList.add('info');
        controllerIdDisplay.textContent = `My Controller ID: ${myControllerId}`;
        roomCodeInputDiv.style.display = 'block';
    });

    socket.on('unityGameConnected', () => {
        connectionStatus.textContent = 'Unity Game Connected. Ready to join!';
        connectionStatus.classList.remove('error');
        connectionStatus.classList.add('info');
    });

    socket.on('unityGameDisconnected', () => {
        connectionStatus.textContent = 'Unity Game Disconnected. Please wait.';
        connectionStatus.classList.add('error');
        controlsDiv.style.display = 'none';
        roomCodeInputDiv.style.display = 'block'; // Allow re-joining
        // Stop gyro data if unity game disconnects
        if (gyroDataActive) {
            window.removeEventListener('deviceorientation', handleDeviceOrientation);
            gyroDataActive = false;
        }
    });

    joinGameButton.addEventListener('click', () => {
        const desiredPlayerId = playerIdInput.value.trim();
        if (desiredPlayerId) {
            socket.emit('registerController', { playerId: desiredPlayerId });
        } else {
            showMessage('Please enter a Player ID/Name to join.', 'error');
        }
    });

    socket.on('controllerRegistered', (data) => {
        assignedPlayerId = data.assignedPlayerId;
        playerIdDisplay.textContent = `Assigned Player ID: ${assignedPlayerId}`;
        controlsDiv.style.display = 'block';
        roomCodeInputDiv.style.display = 'none';
        connectionStatus.textContent = 'Controller Registered. You can now play!';
        connectionStatus.classList.remove('error');
        connectionStatus.classList.add('info');
        hideMessageBox(); // Hide any general messages

        // Automatically try to setup gyroscope once registered
        // This will show the "Enable Gyroscope" button if needed
        setupGyroscope();
    });

    socket.on('disconnect', () => {
        connectionStatus.textContent = 'Disconnected from server.';
        connectionStatus.classList.add('error');
        controlsDiv.style.display = 'none';
        roomCodeInputDiv.style.display = 'block';
        if (gyroDataActive) {
            window.removeEventListener('deviceorientation', handleDeviceOrientation);
            gyroDataActive = false;
        }
    });

    socket.on('error', (data) => {
        connectionStatus.textContent = `Error: ${data.message}`;
        connectionStatus.classList.add('error');
    });

    // --- Button Input Handling ---
    document.getElementById('buttonUp').addEventListener('pointerdown', () => sendInput('up_pressed'));
    document.getElementById('buttonUp').addEventListener('pointerup', () => sendInput('up_released'));
    document.getElementById('buttonDown').addEventListener('pointerdown', () => sendInput('down_pressed'));
    document.getElementById('buttonDown').addEventListener('pointerup', () => sendInput('down_released'));
    document.getElementById('buttonLeft').addEventListener('pointerdown', () => sendInput('left_pressed'));
    document.getElementById('buttonLeft').addEventListener('pointerup', () => sendInput('left_released'));
    document.getElementById('buttonRight').addEventListener('pointerdown', () => sendInput('right_pressed'));
    document.getElementById('buttonRight').addEventListener('pointerup', () => sendInput('right_released'));

    // Ensure buttonActionA exists in your index.html
    const buttonActionA = document.getElementById('buttonActionA');
    if (buttonActionA) {
        buttonActionA.addEventListener('pointerdown', () => sendInput('actionA_pressed'));
        buttonActionA.addEventListener('pointerup', () => sendInput('actionA_released'));
    }

    function sendInput(inputType) {
        if (socket.connected && assignedPlayerId) {
            socket.emit('controllerInput', { input: inputType });
        }
    }

    // --- Gyroscope Functionality ---

    // Function to display messages in the custom message box
    function showMessage(message, type = 'info') {
        messageBox.textContent = message;
        messageBox.className = 'message-box ' + type; // Add type class for styling
        messageBox.style.display = 'block';
    }

    // Function to hide the message box
    function hideMessageBox() {
        messageBox.style.display = 'none';
    }

    // Function to handle gyroscope data updates and emit to server
    function handleDeviceOrientation(event) {
        // event.alpha: Rotation around the Z-axis (0 to 360 degrees)
        // event.beta: Rotation around the X-axis (-180 to 180 degrees)
        // event.gamma: Rotation around the Y-axis (-90 to 90 degrees)

        const currentAlpha = event.alpha ? parseFloat(event.alpha.toFixed(2)) : 0;
        const currentBeta = event.beta ? parseFloat(event.beta.toFixed(2)) : 0;
        const currentGamma = event.gamma ? parseFloat(event.gamma.toFixed(2)) : 0;

        // Always update display
        alphaElement.textContent = currentAlpha;
        betaElement.textContent = currentBeta;
        gammaElement.textContent = currentGamma;

        // Throttle sending data to the server
        const now = Date.now();
        if (now - lastGyroSendTime > GYRO_SEND_INTERVAL) {
            if (socket.connected && assignedPlayerId) { // Ensure connection and registration
                socket.emit('gyroData', {
                    alpha: currentAlpha,
                    beta: currentBeta,
                    gamma: currentGamma
                });
                lastGyroSendTime = now;
            }
        }
    }

    // Check for DeviceOrientationEvent support and handle permissions
    function setupGyroscope() {
        // Check if DeviceOrientationEvent is supported by the browser
        if (window.DeviceOrientationEvent) {
            // For iOS 13+ and some other browsers, explicit permission is required
            if (typeof DeviceOrientationEvent.requestPermission === 'function') {
                // Show the permission request button for iOS 13+
                requestPermissionButton.classList.remove('hidden');

                requestPermissionButton.onclick = () => {
                    DeviceOrientationEvent.requestPermission()
                        .then(permissionState => {
                            if (permissionState === 'granted') {
                                // Permission granted, add the event listener
                                window.addEventListener('deviceorientation', handleDeviceOrientation);
                                gyroDataActive = true;
                                requestPermissionButton.classList.add('hidden'); // Hide button
                                showMessage('Gyroscope enabled. Move your device!', 'info');
                            } else {
                                // Permission denied
                                showMessage(
                                    'Permission to access device motion was denied. ' +
                                    'On iOS, this often requires the page to be served over HTTPS. ' +
                                    'If you are already on HTTPS, check your iPhone Settings > Safari > Motion & Orientation Access.',
                                    'error'
                                );
                                requestPermissionButton.classList.remove('hidden'); // Keep button visible if denied
                            }
                        })
                        .catch(error => {
                            console.error("Error requesting device orientation permission:", error);
                            showMessage(
                                'An error occurred while requesting permission: ' + error.message + '. ' +
                                'On iOS, this typically means the page is NOT served over HTTPS. ' +
                                'Please ensure you are using a secure connection (HTTPS).',
                                'error'
                            );
                            requestPermissionButton.classList.remove('hidden'); // Keep button visible if error
                        });
                };
            } else {
                // No explicit permission required (Android, desktop, older iOS)
                window.addEventListener('deviceorientation', handleDeviceOrientation);
                gyroDataActive = true;
                showMessage('Gyroscope data is being streamed. Move your device!', 'info');
            }
        } else {
            // DeviceOrientationEvent is not supported at all
            showMessage('Your browser does not support Device Orientation events. Gyroscope data cannot be accessed.', 'error');
            requestPermissionButton.classList.add('hidden'); // Hide button if not supported
        }
    }

    // Call setupGyroscope when the DOM is fully loaded, but before player registration.
    // The actual permission request will still be tied to the button click.
    // It will then be re-called after 'controllerRegistered' to ensure UI updates.
    setupGyroscope();

});