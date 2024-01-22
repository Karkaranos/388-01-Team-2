using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TouchEvents : MonoBehaviour
{
    public TMP_Text tapText;
    public TMP_Text posText;
    private Input touchControls;
    public GameObject ob;
    bool beingTouched = false;

    Vector2 touchPos;
    private void Awake()
    {
        touchControls = new Input();
        touchControls.Enable();

        touchControls.Touch.Tap.started += ctx => StartTouch(ctx);
        touchControls.Touch.Tap.canceled += ctx => EndTouch(ctx);
    }

    private void EndTouch(InputAction.CallbackContext ctx)
    {
        beingTouched = false;
        touchPos = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        tapText.text = "Touch Ended";
        //ob.transform.position = new Vector3(touchPos.x/(Screen.width/2), touchPos.y/(Screen.height/2), 0);
        //posText.text = "Circle Position: " + ob.transform.position;

    }

    private void StartTouch(InputAction.CallbackContext ctx)
    {
        touchPos = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        tapText.text = "Touch Started at " + touchPos;

    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

}
