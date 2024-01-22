using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using System;

public class TouchJoystick : MonoBehaviour
{
    /*public TMP_Text directionText;
    private Touch playerInput;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    Vector2 dragDistance;

    private string dir;
    */
    [SerializeField]
    private FloatingJoystick joystick;
    [SerializeField]
    private Vector2 joystickSize = new Vector2(300,300);
    private Finger movementFinger;
    private Vector2 movementAmt;

    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private GameObject player;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
        ETouch.Touch.onFingerUp += Touch_onFingerUp;
        ETouch.Touch.onFingerMove += Touch_onFingerMove;
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
        ETouch.Touch.onFingerUp -= Touch_onFingerUp;
        ETouch.Touch.onFingerMove -= Touch_onFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void Touch_onFingerMove(Finger MovedFinger)
    {
        if(MovedFinger==movementFinger)
        {
            Vector2 knobPos;
            float maxMovement = joystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            if(Vector2.Distance(currentTouch.screenPosition, joystick.RectTransform.anchoredPosition)
                > maxMovement)
            {
                knobPos = (currentTouch.screenPosition - joystick.RectTransform.anchoredPosition).normalized * maxMovement;

            }
            else
            {
                knobPos = currentTouch.screenPosition - joystick.RectTransform.anchoredPosition;
            }

            joystick.Knob.anchoredPosition = knobPos;
            movementAmt = knobPos / maxMovement;

            MovePlayer();

        }
    }

    private void Touch_onFingerUp(Finger LostFinger)
    {
        if(LostFinger == movementFinger)
        {
            movementFinger = null;
            joystick.Knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            movementAmt = Vector2.zero;
        }
    }

    private void Touch_onFingerDown(Finger TouchedFinger)
    {
        if(movementFinger == null && TouchedFinger.screenPosition.x <= Screen.width/ 2f)
        {
            movementFinger = TouchedFinger;
            movementAmt = Vector2.zero;
            joystick.gameObject.SetActive(true);
            joystick.RectTransform.sizeDelta = joystickSize;
            joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPos)
    {
        if(startPos.x < joystickSize.x/2)
        {
            startPos.x = joystickSize.x/2;
        }
        if (startPos.y < joystickSize.y / 2)
        {
            startPos.y = joystickSize.y / 2;
        }
        else if (startPos.y > Screen.height - joystickSize.y/2)
        {
            startPos.y = Screen.height - joystickSize.y / 2;
        }

        return startPos;
    }

    private void MovePlayer()
    {
        Vector2 newPos = new Vector2(player.transform.position.x + movementAmt.x, player.transform.position.y + movementAmt.y) * playerSpeed; ;
        
        player.transform.position = newPos; 
    }


    /*
    private void Update()
    {
        /*
        if(Input.touchCount > 0)
        {
            playerInput = Input.GetTouch(0);

            if(playerInput.phase == TouchPhase.Began)
            {
                touchStartPos = playerInput.position;
            }

            else if (playerInput.phase == TouchPhase.Moved || playerInput.phase == TouchPhase.Ended)
            {
                touchEndPos = playerInput.position;
                dragDistance.x = touchEndPos.x - touchStartPos.x;
                dragDistance.y = touchEndPos.y - touchStartPos.y;

                if(Mathf.Abs(dragDistance.x)<= .5f && Mathf.Abs(dragDistance.y)<= .5f)
                {
                    dir = "Tapped";
                }
                else if (Mathf.Abs(dragDistance.x) > Mathf.Abs(dragDistance.y))
                {
                    if(dragDistance.x > 0)
                    {
                        dir = "Right";
                    }
                    else
                    {
                        dir = "Left";
                    }
                }
                else
                {
                    if (dragDistance.y > 0)
                    {
                        dir = "Up";
                    }
                    else
                    {
                        dir = "Down";
                    }
                }
            }
            directionText.text = dir;
        }
    }*/


}
