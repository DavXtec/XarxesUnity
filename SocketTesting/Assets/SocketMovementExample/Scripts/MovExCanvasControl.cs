using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovExCanvasControl : MonoBehaviour {
    [SerializeField] GameObject pauseGroupObj;
    [SerializeField] GameObject mainGroupObj;
    [SerializeField] GameObject connectButtonObj;
    [SerializeField] GameObject connectionFailObj;
    [SerializeField] GameObject connectionLostObj;


    bool isPaused;

    static MovExCanvasControl canvasControl;

    static MovExCanvasControl instance {
        get {
            return RequestMovExCanvasControl();
        }
    }

    private void Awake() {
        MovExWebSocketManager.instance.onConnectionEstablished.AddListener(OnEstablishedConnectionInternal);
        MovExWebSocketManager.instance.onConnectionLost.AddListener(OnConnectionLostInternal);
        MovExWebSocketManager.instance.onConnectionClosed.AddListener(OnConnectionLostInternal);
    }

    void OnEstablishedConnectionInternal() {
        UnPause();
        pauseGroupObj.SetActive(false);
        mainGroupObj.SetActive(false);
    }

    void OnConnectionLostInternal() {
        pauseGroupObj.SetActive(false);
        mainGroupObj.SetActive(true);
        connectionLostObj.SetActive(true);
        StartCoroutine(_DisableAfterSeconds(connectionLostObj, 3.0f));
    }

    IEnumerator _DisableAfterSeconds(GameObject obj, float time) {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    private static MovExCanvasControl RequestMovExCanvasControl() {
        if (canvasControl == null) {
            canvasControl = FindObjectOfType<MovExCanvasControl>();
        }
        return canvasControl;
    }

    public static void Pause() {
        instance.isPaused = true;
        Time.timeScale = 0;
        instance.pauseGroupObj.SetActive(true);
    }

    public static void UnPause() {
        instance.isPaused = false;
        Time.timeScale = 1;
        instance.pauseGroupObj.SetActive(false);
    }

    public static void TogglePause() {
        if (instance.isPaused) {
            UnPause();
        } else {
            Pause();
        }
    }

}
