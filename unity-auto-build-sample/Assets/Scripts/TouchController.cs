using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {
    [SerializeField] Transform cube;
    [SerializeField] float speed = 20f;

    bool isStop;

    void Awake() {
        Application.targetFrameRate = 60;
    }

    void Update() {
        if(Input.touchCount > 0) {
            OnTouchPhase();
        } else if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
            OnMousePhase();
        }
        if(isStop) {return;}
        cube.Rotate(new Vector3(speed, -speed * 0.5f, -speed) * Time.deltaTime);
    }

    void OnTouchPhase() {
        foreach(var touch in Input.touches) {
            if(touch.phase == TouchPhase.Began && touch.fingerId == 0) {
                TouchStart(touch.position);
            }
        }
    }

    void OnMousePhase() {
        if(Input.GetMouseButtonDown(0)) {
            TouchStart(Input.mousePosition);
        }
    }

    void TouchStart(Vector3 position) {
        Debug.Log($"touch: {position} : {isStop}");
        isStop = !isStop;
    }
}
