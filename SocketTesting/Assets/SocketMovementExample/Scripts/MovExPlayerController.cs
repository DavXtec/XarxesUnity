using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovExPlayerController : MonoBehaviour {
    public int playerIndex;
    [SerializeField] float speed = 2.0f;
    internal void SetDesiredMovementDirection(Vector3 inputMovement) {
        transform.position += inputMovement * Time.deltaTime * speed;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        MovExWebSocketManager.instance.SendPosition(transform.position);
    }
}
