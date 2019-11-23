using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objectsensor : MonoBehaviour
{
    [SerializeField]
    private ContactFilter2D m_EnemyContactFilter;
    private Collider2D[] m_EnemyColliderInSensor = new Collider2D[5];

    [SerializeField]
    [Range(0f, 360f)]
    private float m_LineOfSightField = 0f;

    [SerializeField]
    [Range(0f, 360f)]
    private float m_LineOfSightDegree = 0f;

    [SerializeField]
    private float m_LineOfSightRange = 1f;

    private GameObject m_SensedTarget;
    private GameObject m_SightedTarget;
    private bool m_FacingRight;

    protected void Update()
    {
        ScanTargetInRangeOfSight();
        CheckTargetStillVisible();
    }

    public GameObject GetSensedTarget()
    {
        return m_SensedTarget;
    }

    public GameObject GetSightedTarget()
    {
        return m_SightedTarget;
    }

    protected void ScanTargetInRangeOfSight()
    {
        Physics2D.OverlapCircle(transform.position, m_LineOfSightRange, m_EnemyContactFilter, m_EnemyColliderInSensor);
    }

    protected void CheckTargetStillVisible()
    {
        if (m_SightedTarget != null)
            m_SensedTarget = m_SightedTarget;
        else
        {
            if (m_EnemyColliderInSensor[0] != null)
                m_SensedTarget = m_EnemyColliderInSensor[0].gameObject;
        }

        if (m_SensedTarget == null)
            return;

        Vector3 toTarget = m_SensedTarget.transform.position - transform.position;
        Vector3 forward = m_FacingRight ? Vector3.right : Vector3.left;

        // Check Enemy in range
        if (toTarget.sqrMagnitude < m_LineOfSightRange * m_LineOfSightRange)
        {
            //Check Enemy in Line of sight Degree

            // Rotate Vector with Degree on z axis
            Vector3 testForward = Quaternion.Euler(0, 0, m_FacingRight ? -m_LineOfSightDegree : m_LineOfSightDegree) * forward;

            float angle = Vector3.Angle(testForward, toTarget);

            // 0.5f mean field pivot on center of forward vector
            if (angle <= m_LineOfSightField * 0.5f)
            {
                //Debug.Log("See Player in Line of Sight");
                m_SightedTarget = m_SensedTarget;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawScanEnemyGizmo();
        DrawLineOfSightGizmo();
    }

    private void DrawScanEnemyGizmo()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_LineOfSightRange);
    }

    private void DrawLineOfSightGizmo()
    {
        //draw the line of sight
        Vector3 forward = m_FacingRight ? Vector2.right : Vector2.left;
        forward = Quaternion.Euler(0, 0, m_FacingRight ? -m_LineOfSightDegree : m_LineOfSightDegree) * forward;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, m_LineOfSightField * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, m_LineOfSightField, m_LineOfSightRange);
    }
#endif
}
