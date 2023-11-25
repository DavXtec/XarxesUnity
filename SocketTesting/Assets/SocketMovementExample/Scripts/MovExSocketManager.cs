using SimpleJSON;
using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class MovExWebSocketManager : MonoBehaviour {
    string serverUrl = "ws://127.0.0.1:3000"; // Replace with your server's IP or hostname
    private WebSocket ws;
    [SerializeField] MovExPlayerController playerController;
    [SerializeField] List<MovExPlayerSimulator> playerSimulators;
    [SerializeField] GameObject playerSimulatorPrefab;

    static bool connectionWasClosed;
    static bool connectionWasEstablished;
    static bool connectionWasLost;

    public UnityEvent onConnectionEstablished;
    //public UnityEvent onConnectionFailed;
    public UnityEvent onConnectionLost;
    public UnityEvent onConnectionClosed;

    static MovExWebSocketManager webSocketManager;

    List<int> playerIndexesToInstantiate = new List<int>();
    [SerializeField] bool debug = false;

    public static MovExWebSocketManager instance {
        get {
            return RequestWebSocketManager();
        }
    }

    static MovExWebSocketManager RequestWebSocketManager() {
        if (webSocketManager == null) {
            webSocketManager = FindObjectOfType<MovExWebSocketManager>();
        }
        return webSocketManager;
    }

    [System.Serializable]
    public class ClientMessage {
        public string messageType;
        //public int intData;
    }

    private void Awake() {
        Application.runInBackground = true;
        if (webSocketManager == null) {
            webSocketManager = this;
            playerSimulators = new List<MovExPlayerSimulator>();
        } else {
            if (webSocketManager != this) {
                Destroy(gameObject);
            }
        }
    }

    void Start() {

    }

    private void Update() {
        if (playerIndexesToInstantiate.Count > 0) {
            for (int i = 0; i < playerIndexesToInstantiate.Count; i++) {
                GameObject playerSimulatorInstance = GameObject.Instantiate(playerSimulatorPrefab);
                MovExPlayerSimulator playerSimulatorInstanceRef = playerSimulatorInstance.GetComponentInChildren<MovExPlayerSimulator>();
                playerSimulators.Add(playerSimulatorInstanceRef);
                playerSimulatorInstanceRef.playerIndex = playerIndexesToInstantiate[i];
            }
            playerIndexesToInstantiate.Clear();
        }
        if (connectionWasClosed) {
            ConnectionWasClosed();
            connectionWasClosed = false;
        }
        if (connectionWasEstablished) {
            ConnectionWasEstablished();
            connectionWasEstablished = false;
        }
        if (connectionWasLost) {
            ConnectionWasLost();
            connectionWasLost = false;
        }

    }

    private void ConnectionWasLost() {
        onConnectionLost?.Invoke();
    }

    private void ConnectionWasEstablished() {
        if (debug) {
            Debug.Log("Connected to server");
        }
        onConnectionEstablished?.Invoke();
        ws.Send("Hello from Unity!");
    }

    public void EstablishConnection() {
        //string serverUrl = "ws://localhost:3000"; // Replace with your server's IP or hostname
        ws = new WebSocket(serverUrl);

        ws.OnOpen += (sender, e) => {
            connectionWasEstablished = true;
        };

        ws.OnMessage += (sender, e) => {
            if (debug) {
                Debug.Log("Message from server: " + e.Data);
            }
            HandleServerMessage(e);
            // Handle the data received from the server
        };

        ws.OnClose += (sender, e) => {
            connectionWasClosed = true;
        };

        ws.OnError += (sender, e) => {
            connectionWasLost = true;
            //onConnectionFailed?.Invoke();
        };

        // Start the WebSocket connection

        ws.Connect();



    }

    public void SendPosition(Vector3 position) {
        string jsonString =
            "{" +
            "\"MessageType\": \"UpdatePosition\", " +
            "\"playerIndex\": " + playerController.playerIndex + "," +
            "\"positionX\": " + Mathf.Floor(position.x * 100) + "," +
            "\"positionY\": " + Mathf.Floor(position.y * 100) + "," +
            "\"positionZ\": " + Mathf.Floor(position.z * 100) +
            "}";
        if (ws.ReadyState == WebSocketState.Open)
            ws.Send(jsonString);
    }

    public void CloseConnection() {
        if (ws.ReadyState == WebSocketState.Open) {
            ws.Close();
        }
    }

    public void ResetConnection() {
        CloseConnection();
        EstablishConnection();
    }

    public ClientMessage currentMessage;
    public void HandleServerMessage(MessageEventArgs e) {
        JSONNode json = JSON.Parse(e.Data.ToString());

        MovExServerMessages.MessageType messageType = MovExServerMessages.GetTypeFromString(json[MovExServerMessages.DataFieldStrings[(int)MovExServerMessages.DataFields.MessageType]]);
        switch (messageType) {
            case MovExServerMessages.MessageType.Welcome:
                playerController.playerIndex = json[(int)MovExServerMessages.DataFields.PlayerIndex];
                break;
            case MovExServerMessages.MessageType.NewPlayer: {
                    int playerIndexToInstantiate = json[(int)MovExServerMessages.DataFields.PlayerIndex];
                    if (playerIndexToInstantiate != playerController.playerIndex) {
                        playerIndexesToInstantiate.Add(playerIndexToInstantiate);
                    }
                }
                break;
            case MovExServerMessages.MessageType.OldPlayers: {
                    JSONNode playerIndexesArray = json[(int)MovExServerMessages.DataFields.PlayerIndex];
                    List<int> playerIndexesArrayList = new List<int>();
                    for (int i = 0; i < playerIndexesArray.Count; i++) {
                        int number = playerIndexesArray[i].AsInt;
                        if (number != playerController.playerIndex) {
                            playerIndexesToInstantiate.Add(number);
                        }
                    }
                    break;
                }
            case MovExServerMessages.MessageType.DisconnectPlayer: {
                    int aPlayerIndex = json[(int)MovExServerMessages.DataFields.PlayerIndex];
                    GetPlayerSimWithIndex(aPlayerIndex).GetDestroyed();


                    break;
                }
            case MovExServerMessages.MessageType.Answer:
                break;
            case MovExServerMessages.MessageType.Forfeit:
                break;
            case MovExServerMessages.MessageType.PlayerUpdate: {
                    int aPlayerIndex = json[(int)MovExServerMessages.DataFields.PlayerIndex];
                    if (aPlayerIndex != playerController.playerIndex) {

                        Vector3 newPosition =
                            new Vector3(
                                json[MovExServerMessages.DataFieldStrings[(int)MovExServerMessages.DataFields.PlayerPositionX]],
                                json[MovExServerMessages.DataFieldStrings[(int)MovExServerMessages.DataFields.PlayerPositionY]],
                                json[MovExServerMessages.DataFieldStrings[(int)MovExServerMessages.DataFields.PlayerPositionZ]]);

                        GetPlayerSimWithIndex(aPlayerIndex).UpdatePosition(newPosition.x, newPosition.y, newPosition.z);

                    }
                    break;
                }
            case MovExServerMessages.MessageType.Length:
                Debug.Log("Unhandled message received -->  " + json[(int)MovExServerMessages.DataFields.MessageType]);
                break;
        }

    }

    MovExPlayerSimulator GetPlayerSimWithIndex(int anIndex) {
        MovExPlayerSimulator retVal = null;
        for (int i = 0; i < playerSimulators.Count && retVal == null; i++) {
            if (playerSimulators[i].playerIndex == anIndex) {
                retVal = playerSimulators[i];
            }
        }
        return retVal;
    }



    void ConnectionWasClosed() {
        if (ws.IsAlive)
            onConnectionLost?.Invoke();
        else
            onConnectionClosed?.Invoke();

        if (debug) {
            Debug.Log("Disconnected from server");
        }
    }


    void OnDestroy() {
        // Close the WebSocket connection when the GameObject is destroyed
        if (ws != null && ws.IsAlive) {
            ws.Close();
        }
    }

}
