using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoBehavior : MonoBehaviour
{
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


    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.positionCount = precision;
        waveSize = StartWaveSize;
        strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
    }

    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < precision; i++)
        {
            m_lineRenderer.SetPosition(i, ThrowingArm.FirePoint.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (!strightLine)
        {
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

    void DrawRopeWaves()
    {
        for (int i = 0; i < precision; i++)
        {
            float delta = (float)i / ((float)precision - 1f);
            Vector2 offset = Vector2.Perpendicular(ThrowingArm.LassoDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(ThrowingArm.FirePoint.position, ThrowingArm.LassoPoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(ThrowingArm.FirePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, ThrowingArm.FirePoint.position);
        m_lineRenderer.SetPosition(1, ThrowingArm.LassoPoint);
        pBehav.Throwing = false;
    }
}
