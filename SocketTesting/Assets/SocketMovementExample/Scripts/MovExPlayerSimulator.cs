using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovExPlayerSimulator : MonoBehaviour {
    public int playerIndex;
    public Vector3 positionToBeSet;
    bool markedForDestruction = false;

    private void Update() {
        if (markedForDestruction) {
            Destroy(gameObject);
        }
        transform.position = positionToBeSet;
    }

    public void UpdatePosition(float x, float y, float z) {
        Vector3 aPos = new Vector3(x, y, z);
        aPos /= 100.0f;
        positionToBeSet = aPos;
    }

    public void GetDestroyed() {
        markedForDestruction = true;
    }
}
