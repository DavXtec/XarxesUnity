using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovExInputHandler : MonoBehaviour {
    [SerializeField] MovExPlayerController playerController;


    private void Start() {


    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MovExCanvasControl.TogglePause();
        }
        Vector3 inputMovement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            inputMovement.z += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            inputMovement.z -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputMovement.x += 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            inputMovement.x -= 1;
        }

        playerController.SetDesiredMovementDirection(inputMovement);
    }

}
