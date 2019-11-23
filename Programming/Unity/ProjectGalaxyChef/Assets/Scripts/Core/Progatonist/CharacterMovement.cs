using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        public LayerMask GroundLayerMask;
        public float GroundRaycastRange = 0.1f;
        public float HeadHitRaycastRange = 0.1f;

        public bool IsGrounded;
        public bool IsHeadHit;
        public Vector2 Velocity { get; protected set; }
        public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
        public Collider2D[] GroundColliders { get { return m_GroundColliders; } }
        public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }
        public Collider2D[] HeadHitColliders { get { return m_HeadHitColliders; } }

        Rigidbody2D m_Rigidbody2D;
        BoxCollider2D m_Capsule;
        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;

        ContactFilter2D m_ContactFilter;
        RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
        RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
        Collider2D[] m_GroundColliders = new Collider2D[3];
        Vector2[] m_RaycastPositions = new Vector2[3];
        Vector2 m_RaycastDirection = new Vector2();

        RaycastHit2D[] m_FoundHeadHits = new RaycastHit2D[3];
        Collider2D[] m_HeadHitColliders = new Collider2D[3];
        Vector2[] m_HeadHitRayCastPositions = new Vector2[3];
        Vector2 m_HeadHitRaycastDirection = Vector2.up;

        void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Capsule = GetComponent<BoxCollider2D>();

            m_CurrentPosition = m_Rigidbody2D.position;
            m_PreviousPosition = m_Rigidbody2D.position;

            m_ContactFilter.layerMask = GroundLayerMask;
            m_ContactFilter.useLayerMask = true;
            m_ContactFilter.useTriggers = false;
        }

        void Update()
        {
            
        }

        void FixedUpdate()
        {
            CheckGrounded();
            CheckHeadHit();

            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_PreviousPosition + m_NextMovement;
            Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.fixedDeltaTime;

            m_Rigidbody2D.MovePosition(m_CurrentPosition);
            m_NextMovement = Vector2.zero;
        }

        void OnDrawGizmos()
        {
            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                // Remove from if because it always true
                // m_FoundHits[i] != null

                if (i < m_FoundHits.Length && m_FoundHits[i].point != Vector2.zero)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                // Draw Ray with 0.4f Long Unit
                Gizmos.DrawRay(m_RaycastPositions[i], m_RaycastDirection.normalized * 0.4f);

                //if (i < m_FoundHits.Length && m_FoundHits[i] != null && m_FoundHits[i].point != Vector2.zero)
                //{
                //    Gizmos.color = Color.red;
                //    Gizmos.DrawLine(m_RaycastPositions[i], m_FoundHits[i].point);
                //}
            }

            for (int i = 0; i < m_HeadHitRayCastPositions.Length; i++)
            {
                if (i < m_FoundHeadHits.Length && m_FoundHeadHits[i].point != Vector2.zero)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                // Draw Ray with 0.4f Long Unit
                Gizmos.DrawRay(m_HeadHitRayCastPositions[i], m_HeadHitRaycastDirection.normalized * 0.4f);
            }
        }

        public void Move(Vector2 movement)
        {
            m_NextMovement += movement;
        }

        public bool IsGroundHitTag(string tag)
        {
            foreach(Collider2D col in m_GroundColliders)
            {
                if (col == null)
                    continue;

                if (col.CompareTag(tag))
                    return true;
            }
            return false;
        }

        void CheckGrounded()
        {
            Vector2 raycastStart;
            float raycastDistance;

            if (m_Capsule == null)
            {
                raycastStart = m_Rigidbody2D.position + Vector2.down;
                raycastDistance = 1f + GroundRaycastRange;

                m_RaycastDirection = Vector2.down;

                m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                m_RaycastPositions[1] = raycastStart;
                m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
            }
            else
            {
                raycastStart = m_Rigidbody2D.position + m_Capsule.offset;
                //raycastDistance = m_Capsule.size.x * 0.5f + GroundLaycastRange * 2f;
                raycastDistance = GroundRaycastRange;

                m_RaycastDirection = Vector2.down;
                Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (m_Capsule.size.y * 0.51f);

                m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.left * m_Capsule.size.x * 0.5f;
                m_RaycastPositions[1] = raycastStartBottomCentre;
                m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.right * m_Capsule.size.x * 0.5f;
            }

            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                int count = Physics2D.Raycast(m_RaycastPositions[i], m_RaycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

                m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                m_GroundColliders[i] = m_FoundHits[i].collider;
            }

            Vector2 groundNormal = Vector2.zero;
            int hitCount = 0;

            for (int i = 0; i < m_FoundHits.Length; i++)
            {
                if (m_FoundHits[i].collider != null)
                {
                    groundNormal += m_FoundHits[i].normal;
                    hitCount++;
                }
            }

            if (hitCount > 0)
            {
                groundNormal.Normalize();
            }

            Vector2 relativeVelocity = Velocity;

            if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
            {
                IsGrounded = false;
            }
            else
            {
                IsGrounded = relativeVelocity.y <= 0f;

                if (m_Capsule != null)
                {
                    if (m_GroundColliders[1] != null)
                    {
                        float capsuleBottomHeight = m_Rigidbody2D.position.y + m_Capsule.offset.y - m_Capsule.size.y * 0.5f;
                        float middleHitHeight = m_FoundHits[1].point.y;
                        bool footHit = middleHitHeight <= (capsuleBottomHeight + GroundRaycastRange);
                        IsGrounded &= footHit;
                    }
                }
            }

            // Clear Hit Buffer
            for (int i = 0; i < m_HitBuffer.Length; i++)
            {
                m_HitBuffer[i] = new RaycastHit2D();
            }
        }

        void CheckHeadHit()
        {
            Vector2 raycastStart;
            float raycastDistance;

            if (m_Capsule == null)
            {
                raycastStart = m_Rigidbody2D.position + Vector2.up;
                raycastDistance = 1f + HeadHitRaycastRange;

                m_HeadHitRayCastPositions[0] = raycastStart + Vector2.left * 0.4f;
                m_HeadHitRayCastPositions[1] = raycastStart;
                m_HeadHitRayCastPositions[2] = raycastStart + Vector2.right * 0.4f;
            }
            else
            {
                raycastStart = m_Rigidbody2D.position + m_Capsule.offset;
                raycastDistance = HeadHitRaycastRange;

                Vector2 raycastStartTopCentre = raycastStart + Vector2.up * (m_Capsule.size.y * 0.51f);

                m_HeadHitRayCastPositions[0] = raycastStartTopCentre + Vector2.left * m_Capsule.size.x * 0.5f;
                m_HeadHitRayCastPositions[1] = raycastStartTopCentre;
                m_HeadHitRayCastPositions[2] = raycastStartTopCentre + Vector2.right * m_Capsule.size.x * 0.5f;
            }

            for (int i = 0; i < m_HeadHitRayCastPositions.Length; i++)
            {
                int count = Physics2D.Raycast(m_HeadHitRayCastPositions[i], m_HeadHitRaycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

                m_FoundHeadHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                m_HeadHitColliders[i] = m_FoundHits[i].collider;
            }

            Vector2 groundNormal = Vector2.zero;
            int hitCount = 0;

            for (int i = 0; i < m_FoundHeadHits.Length; i++)
            {
                if (m_FoundHeadHits[i].collider != null)
                {
                    groundNormal += m_FoundHeadHits[i].normal;
                    hitCount++;
                }
            }

            if (hitCount > 0)
            {
                groundNormal.Normalize();
            }

            Vector2 relativeVelocity = Velocity;

            if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
            {
                IsHeadHit = false;
            }
            else
            {
                IsHeadHit = relativeVelocity.y > 0f;

                if (m_Capsule != null)
                {
                    if (m_HeadHitColliders[1] != null)
                    {
                        float capsuleHeadHeight = m_Rigidbody2D.position.y + m_Capsule.offset.y + m_Capsule.size.y * 0.5f;
                        float middleHitHeight = m_FoundHeadHits[1].point.y;
                        bool headHit = middleHitHeight <= (capsuleHeadHeight + HeadHitRaycastRange);
                        IsHeadHit &= headHit;
                    }
                }
            }

            // Clear Hit Buffer
            for (int i = 0; i < m_HitBuffer.Length; i++)
            {
                m_HitBuffer[i] = new RaycastHit2D();
            }
        }
    }
}
