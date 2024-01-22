/*****************************************************************************
// File Name :         FloatingJoystick.cs
// Author :            Cade R. Naylor
// Code by :           LlamAcademy on YouTube
// Link to Video :      https://www.youtube.com/watch?v=MKnLPA5hnPA&t=222s
// Creation Date :     January 21, 2024
//
// Brief Description : Creates floating Joystick functionality

*****************************************************************************/
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class FloatingJoystick : MonoBehaviour
{
    [HideInInspector]
    public RectTransform RectTransform;
    public RectTransform Knob;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
}