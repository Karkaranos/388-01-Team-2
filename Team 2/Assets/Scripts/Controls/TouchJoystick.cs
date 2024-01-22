/*****************************************************************************
// File Name :         TouchJoystick.cs
// Author :            Cade R. Naylor
// Creation Date :     January 21, 2024
//
// Brief Description : Handles most touch events. Creates and handles player 
                        movement using a joystick. Detects swipes and taps. 
                        Displays user action. 

//Credit to LlamAcademy on YouTube for basic joystick controls
                        https://www.youtube.com/watch?v=MKnLPA5hnPA&t=222s
*****************************************************************************/
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchJoystick : MonoBehaviour
{
    #region Variables 

    //Displays current information
    public TMP_Text directionText;
    
    //Handles swipes and taps
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    Vector2 dragDistance;
    private string dir;
    private bool joystickActive;
    private bool touchActive;


    //References to the joystick and finger
    [SerializeField]
    private FloatingJoystick joystick;
    [SerializeField]
    private Vector2 joystickSize = new Vector2(300,300);
    private Finger movementFinger;
    private Vector2 movementAmt;


    //Handles player movement and restrictions
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private GameObject player;
    private int maxWorldHeight = 4;
    private int maxWorldWidth = 10;

    #endregion

    #region

    /// <summary>
    /// Occurs when the game object is first loaded. 
    /// Links InputActions with respective functions
    /// </summary>
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
        ETouch.Touch.onFingerUp += Touch_onFingerUp;
        ETouch.Touch.onFingerMove += Touch_onFingerMove;
    }

    /// <summary>
    /// Occurs when the game object is deleted. 
    /// Unlinks InputActions with respective functions
    /// </summary>
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
        ETouch.Touch.onFingerUp -= Touch_onFingerUp;
        ETouch.Touch.onFingerMove -= Touch_onFingerMove;
        EnhancedTouchSupport.Disable();
    }

    /// <summary>
    /// Handles visual joystick movement and saves new joystick value.
    /// Calls MovePlayer
    /// </summary>
    /// <param name="MovedFinger">The finger moved</param>
    private void Touch_onFingerMove(Finger MovedFinger)
    {
        //If the moved finger is the current finger, move the joystick
        if(MovedFinger==movementFinger&&joystickActive)
        {
            Vector2 knobPos;
            float maxMovement = joystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            //Clamp the knob's position so it stays within the circle
            if(Vector2.Distance(currentTouch.screenPosition, 
                joystick.RectTransform.anchoredPosition)
                > maxMovement)
            {
                knobPos = (currentTouch.screenPosition - 
                    joystick.RectTransform.anchoredPosition).normalized * 
                    maxMovement;

            }
            else
            {
                knobPos = currentTouch.screenPosition - 
                    joystick.RectTransform.anchoredPosition;
            }

            //Set the new knob position
            joystick.Knob.anchoredPosition = knobPos;
            movementAmt = knobPos / maxMovement;

            //What occurs after the joystick is moved properly
            MovePlayer();

        }
    }

    /// <summary>
    /// Handles finger being removed from the screen. Stops joystick movement and
    /// resets values
    /// </summary>
    /// <param name="LostFinger">The finger lost</param>
    private void Touch_onFingerUp(Finger LostFinger)
    {
        //If the moved finger is the current finger, stop the joystick. Reset
        if (LostFinger == movementFinger)
        {
            touchEndPos = LostFinger.screenPosition;
            movementFinger = null;
            joystick.Knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            movementAmt = Vector2.zero;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            directionText.text = "";
            joystickActive = false;
            touchActive = false;

        }
    }

    /// <summary>
    /// Handles finger being added from the screen. Depending on the location, 
    /// enables the joystick or enables swipe/tap detection.
    /// </summary>
    /// <param name="TouchedFinger">The finger added</param>
    private void Touch_onFingerDown(Finger TouchedFinger)
    {
        //If there is no finger AND the added finger is within the joystick's area
        if(movementFinger == null && TouchedFinger.screenPosition.x <= Screen.width/
            3f && TouchedFinger.screenPosition.y <= Screen.height/3f)
        {
            //Enable the joystick and set it to be active
            movementFinger = TouchedFinger;
            movementAmt = Vector2.zero;
            joystick.gameObject.SetActive(true);
            joystick.RectTransform.sizeDelta = joystickSize;
            joystick.RectTransform.anchoredPosition = new Vector2(100, 50);
            joystickActive = true;
            touchActive = true;
        }
        //Otherwise, start Tap and Swipe detection
        else
        {
            movementFinger = TouchedFinger;
            touchActive = true;
            touchStartPos = TouchedFinger.screenPosition;
            StartCoroutine(CheckDirection(TouchedFinger));
        }
    }

    /// <summary>
    /// Handles Swipe and Tap direction. Tracks current finger position
    /// and displays tap or swipe direction
    /// </summary>
    /// <param name="currentFinger">The finger being swiped/tapped with</param>
    /// <returns></returns>
    IEnumerator CheckDirection(Finger currentFinger)
    {
        //Run until the joystick has been activated
        while (!joystickActive&&touchActive)
        {
            //Set the drag distance to the difference between touch start and end
            touchEndPos = currentFinger.screenPosition;
            dragDistance.x = touchEndPos.x - touchStartPos.x;
            dragDistance.y = touchEndPos.y - touchStartPos.y;

            //If the difference is small, it has been tapped
            if (Mathf.Abs(dragDistance.x) <= .5f && Mathf.Abs(dragDistance.y) <= .5f)
            {
                dir = "Tapped";
            }
            //Otherwise, the user swiped the screen.
            //If there is more horizontal distance swiped, display right or left
            else if (Mathf.Abs(dragDistance.x) > Mathf.Abs(dragDistance.y))
            {
                //Checks if the user swiped right
                if (dragDistance.x > 0)
                {
                    dir = "Right";
                }
                else
                {
                    dir = "Left";
                }
            }
            //Otherwise, more vertical distance was swiped, display up or down
            else
            {
                //Checks if the user swiped up
                if (dragDistance.y > 0)
                {
                    dir = "Up";
                }
                else
                {
                    dir = "Down";
                }
            }
            //Display the result and briefly wait
            directionText.text = dir;
            yield return new WaitForSeconds(Time.deltaTime);

        }
        if(!touchActive)
        {
            //Removes text when touch is complete
            dir = "";
            directionText.text = "";
        }
    }

    /// <summary>
    /// Moves the player based on joystick information. 
    /// Calls ClampPlayer
    /// </summary>
    private void MovePlayer()
    {
        player.transform.Translate(movementAmt * playerSpeed, Space.World);
        ClampPlayer(player.transform.position);
    }

    /// <summary>
    /// Clamps the player's position so they remain on screen with a bit of leeway
    /// </summary>
    /// <param name="playerPos">The player's unclamped position</param>
    private void ClampPlayer(Vector2 playerPos)
    {
        
        if (playerPos.x > maxWorldWidth)
        {
            playerPos.x = maxWorldWidth;
        }
        else if (playerPos.x < -maxWorldWidth)
        {
            playerPos.x = -maxWorldWidth;
        }
        if (playerPos.y > maxWorldHeight)
        {
            playerPos.y = maxWorldHeight;
        }
        else if (playerPos.y < -maxWorldHeight)
        {
            playerPos.y = -maxWorldHeight;
        }

        //Set their clamped position
        player.transform.position = playerPos;

        if(joystickActive)
        {
            //Display the joystick information
            directionText.text = "Joystick In Use";
        }

    }

    #endregion
}
