using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchJoystick : MonoBehaviour
{
    public TMP_Text directionText;
    private Touch playerInput;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    Vector2 dragDistance;

    private string dir;

    private void Update()
    {
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
    }


}
