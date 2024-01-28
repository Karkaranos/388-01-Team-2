using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAimArrowBehavior : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    public void ShowArrow()
    {
        arrow.SetActive(true);
    }
    public void HideArrow()
    {
        arrow.SetActive(false);
    }

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
