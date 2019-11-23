using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterAttackMove : MonoBehaviour
{
    public string MoveName;

    [Range(0f, 300f)]
    public float MoveCooldown;

    [HideInInspector]
    public float MoveCooldownTimer;

    public bool CanMoveWithoutTarget;
    public bool FacingRight;
    public bool OnMoving;

    [SerializeField]
    private ContactFilter2D m_EnemyContactFilter;
    private Collider2D[] m_EnemyColliderInSensor = new Collider2D[5];

    [SerializeField]
    [Range(0f, 360f)]
    private float m_MoveStartAreaField = 0f;

    [SerializeField]
    [Range(0f, 360f)]
    private float m_MoveStartAreaDegree = 0f;

    [SerializeField]
    private float m_MoveStartAreaRange = 1f;

    private GameObject m_SensedTarget;
    private GameObject m_Target;

    protected void Update()
    {
        ScanTargetInRangeOfSight();
        CheckTargetStillVisible();
    }

    public bool CanMove()
    {
        return MoveCooldownTimer <= 0;
    }

    public bool HasTarget()
    {
        return m_Target != null;
    }

    public GameObject GetSensedTarget()
    {
        return m_SensedTarget;
    }

    public GameObject GetTarget()
    {
        return m_Target;
    }

    protected void ScanTargetInRangeOfSight()
    {
        Physics2D.OverlapCircle(transform.position, m_MoveStartAreaRange, m_EnemyContactFilter, m_EnemyColliderInSensor);
    }

    protected void CheckTargetStillVisible()
    {
        if (m_Target != null)
            m_SensedTarget = m_Target;
        else
        {
            if (m_EnemyColliderInSensor[0] != null)
                m_SensedTarget = m_EnemyColliderInSensor[0].gameObject;
        }

        if (m_SensedTarget == null)
        {
            m_Target = null;
            return;
        }

        Vector3 toTarget = m_SensedTarget.transform.position - transform.position;
        Vector3 forward = FacingRight ? Vector3.right : Vector3.left;

        // Check Enemy in range
        if (toTarget.sqrMagnitude < m_MoveStartAreaRange * m_MoveStartAreaRange)
        {
            //Check Enemy in Line of sight Degree

            // Rotate Vector with Degree on z axis
            Vector3 testForward = Quaternion.Euler(0, 0, FacingRight ? -m_MoveStartAreaDegree : m_MoveStartAreaDegree) * forward;

            float angle = Vector3.Angle(testForward, toTarget);

            // 0.5f mean field pivot on center of forward vector
            if (angle <= m_MoveStartAreaField * 0.5f)
            {
                //Debug.Log("See Player in Line of Sight");
                m_Target = m_SensedTarget;
            }
            else
            {
                m_Target = null;
            }
        }
        else
        {
            m_Target = null;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MoveStartAreaRange);
    }

    private void DrawLineOfSightGizmo()
    {
        //draw the line of sight
        Vector3 forward = FacingRight ? Vector2.right : Vector2.left;
        forward = Quaternion.Euler(0, 0, FacingRight ? -m_MoveStartAreaDegree : m_MoveStartAreaDegree) * forward;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, m_MoveStartAreaField * 0.5f) * forward);

        Handles.color = new Color(1.0f, 0, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, m_MoveStartAreaField, m_MoveStartAreaRange);
    }
#endif
}
