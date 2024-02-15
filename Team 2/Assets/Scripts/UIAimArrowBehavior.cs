/*****************************************************************************
// File Name :         UIAimArrowBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Rotates the aiming arrow around whatever need aiming

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAimArrowBehavior : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    /// <summary>
    /// moves the arrow here
    /// </summary>
    public void ShowArrow()
    {
        arrow.SetActive(true);
    }

    /// <summary>
    /// the arrow leaves here
    /// </summary>
    public void HideArrow()
    {
        arrow.SetActive(false);
    }

    /// <summary>
    /// rotates with the player
    /// </summary>
    /// <param name="aimingVector"> the player's aiming vector </param>
    /// <param name="controllerDeadzone"> controller deadzone </param>
    /// <param name="controllerRotateSmoothing"> how much rotate smoothing there is </param>
    public void Aim(Vector2 aimingVector, float controllerDeadzone, float controllerRotateSmoothing)
    {
        if ((Mathf.Abs(aimingVector.x) > controllerDeadzone || Mathf.Abs(aimingVector.y) > controllerDeadzone))
        {
            Vector2 playerDirection = new Vector2(aimingVector.x, aimingVector.y);
            if (playerDirection.sqrMagnitude > 0.0f)
            {
                Quaternion newRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, controllerRotateSmoothing * Time.deltaTime);
            }
        }
    }
}
