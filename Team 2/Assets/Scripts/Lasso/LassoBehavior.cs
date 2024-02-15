/*****************************************************************************
// File Name :         LassoBehavior.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Handles the drawing of the lasso

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
    //the headers explain what the variables do/are for
    [Header("General Refernces:")]
    public ThrowingArmBehavior ThrowingArm;
    public LineRenderer m_lineRenderer;
    [SerializeField] private PlayerBehavior pBehav;

    [Header("General Settings:")]
    [SerializeField] private int precision = 40;
    [Range(0, 20)][SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    [SerializeField] float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField][Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;
    [HideInInspector] public bool Missed = true;
    private bool strightLine = true;


    /// <summary>
    /// start moving the lasso immediately when its enabled
    /// </summary>
    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.positionCount = precision;
        waveSize = StartWaveSize;
        strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    /// <summary>
    /// disables the line renderer when the game object holding it is disabled
    /// </summary>
    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
    }

    /// <summary>
    /// adds a point to the line renderer equal to precision
    /// </summary>
    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < precision; i++)
        {
            m_lineRenderer.SetPosition(i, ThrowingArm.FirePoint.position);
        }
    }

    /// <summary>
    /// updates the rope every frame
    /// </summary>
    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    /// <summary>
    /// Draws the rope
    /// </summary>
    void DrawRope()
    {
        //if it's not straight yet, straightens the rope
        if (!strightLine)
        {
            //checks if the line is straight, sets it to straight if true
            if ((m_lineRenderer.GetPosition(precision - 1).x >= ThrowingArm.LassoPoint.x && m_lineRenderer.GetPosition(precision - 1).x < ThrowingArm.LassoPoint.x + 0.01f) ||
                (m_lineRenderer.GetPosition(precision - 1).x <= ThrowingArm.LassoPoint.x && m_lineRenderer.GetPosition(precision - 1).x > ThrowingArm.LassoPoint.x - 0.01f))
            {
                strightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            //this keeps the lasso attached to the player if they move
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }

                DrawRopeNoWaves();
            }
        }
    }

    /// <summary>
    /// Uses trig to calculate the wave of the rope as it gets straighter
    /// </summary>
    void DrawRopeWaves()
    {
        //this calculates each point
        for (int i = 0; i < precision; i++)
        {
            float delta = (float)i / ((float)precision - 1f);
            Vector2 offset = Vector2.Perpendicular(ThrowingArm.LassoDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(ThrowingArm.FirePoint.position, ThrowingArm.LassoPoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(ThrowingArm.FirePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    /// <summary>
    /// keeps the rope straight
    /// </summary>
    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, ThrowingArm.FirePoint.position);
        m_lineRenderer.SetPosition(1, ThrowingArm.LassoPoint);
        pBehav.Throwing = false;
    }
}
