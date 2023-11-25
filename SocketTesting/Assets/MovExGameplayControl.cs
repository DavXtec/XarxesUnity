using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovExGameplayControl : MonoBehaviour {
    [SerializeField] GameObject gamePlayObject;

    private void Awake() {
        MovExWebSocketManager.instance.onConnectionEstablished.AddListener(OnConnectionEstablished);
        // MovExWebSocketManager.instance.onConnectionFailed.AddListener(OnConnectionClosed);
        MovExWebSocketManager.instance.onConnectionLost.AddListener(OnConnectionClosed);
        MovExWebSocketManager.instance.onConnectionClosed.AddListener(OnConnectionClosed);

        gamePlayObject.SetActive(false);
    }

    private void OnConnectionEstablished() {
        gamePlayObject.SetActive(true);
    }
    private void OnConnectionClosed() {
        gamePlayObject.SetActive(false);
    }

}
