const WebSocket = require('ws');
const http = require('http');
const { json } = require('express');
var  indexCount = 0;
const server = http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('WebSocket Server\n');
});

const wss = new WebSocket.Server({ server });

// Create an array to store connected clients
const clients = [];
const games = [];

function newGame(host){
    const game = {
        clients: [],
        questions: [],
    };
    game.clients.push(host);
    games.push(game);
}

function connectPlayer(game, player){
    game.clients.push(player);
}

function sendPlayerPosition(game,playerIndex,playerPosX,playerPosY,playerPosZ){
    console.log(playerIndex + " --> " + playerPosX + ", " + playerPosY + ", " + playerPosZ);
    clientJson = '{\"MessageType\": PlayerUpdate, \"PlayerIndex\":'+ playerIndex 
    +",\"PlayerPositionX\": " + playerPosX
    +",\"PlayerPositionY\":" + playerPosY
    +",\"PlayerPositionZ\":" + playerPosZ 
    + "}";



    console.log("Sending " + clientJson);
    for(let i = 0;i<clients.length;i++){
        clients[i].ws.send(clientJson);
    }
}

function welcomeNewPlayer(client){
    clientJson = '{\"MessageType\": \"NewPlayer\",\"PlayerIndex\":' + client.id+ '}';
    for(let i =0;i< clients.length;i++){
        clients[i].ws.send(clientJson);
    }

    newClientJson = '{\"MessageType\": \"OldPlayers\",\"PlayerIndex\": [';

    for(let i =0;i< clients.length;i++){
        newClientJson += + clients[i].id +',';
    }
    newClientJson +=  ']}';
    client.ws.send(newClientJson);
}

function onClientDisconnected(indexToDisconnect){
    clientJson = '{\"MessageType\": \"DisconnectPlayer\",\"PlayerIndex\":' + client.id+ '}';
    for(let i =0;i< clients.length;i++){
        clients[i].ws.send(clientJson);
    }
}

wss.on('connection', (ws) => {
    console.log('A user connected');

    // Add the new client to the clients array
    client = {
        ws : ws,
    }
    client.id = indexCount;
    clients.push(client);
    indexCount++;

    clientJson = '{\"MessageType\": \"Welcome\",\"PlayerIndex\":' + client.id+ '}';
    //clientJson = '{\"messageType\": \"Welcome\"}';

    client.ws.send(clientJson);
    
    welcomeNewPlayer(client);


    ws.on('message', (message) => {
        try {
            const jsonData = JSON.parse(message);
            console.log('Received JSON data:', jsonData);
    
            // You can now handle the JSON data as an object
            switch (jsonData.MessageType){
                case "UpdatePosition":
                    
                    console.log(jsonData.playerIndex);
                    //console.log("Sending all players info on player " + jsonData.playerIndex + ", position was " + jsonData.positionX + "," + jsonData.positionY + "," + jsonData.positionZ);
                    sendPlayerPosition(null,jsonData.playerIndex,jsonData.positionX,jsonData.positionY,jsonData.positionZ);
                    break;
            }


        } catch (error) {
            console.error('Error parsing JSON data:', error);
            console.log("No JSON to be read, instead I can instead read", message.toString());
        }
    });

    ws.on('close', () => {
        console.log('User disconnected');
        
        // Remove the disconnected client from the clients array
        let indexToRemove= clients.indexOf(ws);
        clients.splice(indexToRemove, 1);
        onClientDisconnected(indexToRemove);
        

    });
});

const PORT = 3000;
server.listen(PORT, () => {
    console.log(`WebSocket server is listening on port ${PORT}`);
});