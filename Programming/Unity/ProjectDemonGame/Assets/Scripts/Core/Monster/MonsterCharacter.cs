using Main.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Main.Character
{
    public class MonsterCharacter : CharacterModule
    {
        [Header("Monster Setting")]
        [SerializeField]
        private string m_MonsterName;
        public string MonsterName { get { return m_MonsterName; } private set { } }

        [SerializeField]
        private int m_Hp;

        [SerializeField]
        private float m_HitDazeTime = 0.35f;

        [SerializeField]
        private ContactFilter2D m_EnemyContactFilter;
        private Collider2D[] m_EnemyColliderInSensor = new Collider2D[5];

        [SerializeField]
        [Range(0f,360f)]
        private float m_LineOfSightField = 0f;

        [SerializeField]
        [Range(0f, 360f)]
        private float m_LineOfSightDegree = 0f;

        [SerializeField]
        private float m_LineOfSightRange = 1f;

        [SerializeField]
        [Range(-1f, 10f)]
        private float m_LostTargetTime = 0f;

        [SerializeField]
        private Hitter m_ContactHitter;

        [SerializeField]
        private Hitter m_AttackHitter;

        [SerializeField]
        private List<MonsterAttackMove> m_AttackMoveList;

        private int m_CurrentHp;
        private int m_MaxHp = 10;
        private float m_DazeTimer;
        public bool IsDazed { get { return m_DazeTimer > 0; } private set { } }

        private GameObject m_SensedTarget;
        private GameObject m_EnemyTarget;
        private float m_TimeSinceLastTargetView;
        private MonsterAttackMove m_CurrentAttackMove;

        protected readonly Vector3 FLIP_ROTATE_SCALE = new Vector3(-1, 1, 1);

        public int Hp
        {
            get
            {
                return m_CurrentHp;
            }

            set
            {
                m_CurrentHp = value;
            }
        }
        public int CurrentHp
        {
            get
            {
                return m_CurrentHp;
            }
            set
            {
                m_CurrentHp = Mathf.Min(value, m_MaxHp);
            }
        }
        public int MaxHp
        {
            get
            {
                return m_MaxHp;
            }

            set
            {
                m_MaxHp = Mathf.Min(value, 0);
            }
        }

        private void Start()
        {
            MaxHp = m_Hp;
            CurrentHp = m_Hp;
            BehaviourLinkedSMB<MonsterCharacter>.Init(CharacterAnimator, this);
        }

        protected override void Update()
        {
            UpdateDazeTimer();
            UpdateAttackMove();

            ScanEnemyInRangeOfSight();
            CheckEnemyStillVisible();

            HorizontalMovement();
            VerticalMovement();

            CheckHeadHit();
            CheckGroundHit();

            UpdateKnockback();
            CharacterMovement.Move(m_MoveVector);

            CheckContactHitter();
            CheckAttackHitter();

            UpdateAnimation();
        }

        public void TakeHit(Hitter hitter)
        {
            m_EnemyTarget = hitter.gameObject;
            Knockback(hitter.HitDirection, hitter.HitPower);
            m_DazeTimer = m_HitDazeTime;
            m_TimeSinceLastTargetView = m_LostTargetTime;

            CharacterAnimator.SetTrigger("Flick");
            CharacterAnimator.SetBool("TargetSpotted", IsEnemyTargetSpotted());
        }

        public void ContactHit(Hitter hitter)
        {
            m_EnemyTarget = hitter.GetHitColliderList()[0].gameObject;
            m_TimeSinceLastTargetView = m_LostTargetTime;
            CharacterAnimator.SetBool("TargetSpotted", IsEnemyTargetSpotted());
        }

        public void AttackHit(Hitter hitter)
        {
            m_EnemyTarget = hitter.GetHitColliderList()[0].gameObject;
        }

        protected override void HorizontalMovement()
        {
            if (IsDazed || m_KnockbackPower > 0)
                return;

            CheckFacing();

            if (FacingRight)
                transform.localScale = FLIP_ROTATE_SCALE;
            else
                transform.localScale = Vector3.one;
        }

        protected void ScanEnemyInRangeOfSight()
        {
            Physics2D.OverlapCircle(transform.position, m_LineOfSightRange, m_EnemyContactFilter, m_EnemyColliderInSensor);
        }

        protected void UpdateDazeTimer()
        {
            if(m_DazeTimer > 0)
            {
                m_DazeTimer -= Time.deltaTime;
            }
        }

        protected void UpdateAttackMove()
        {
            foreach(MonsterAttackMove move in m_AttackMoveList)
            {
                if (move.MoveCooldownTimer > 0 && !move.OnMoving)
                {
                    move.MoveCooldownTimer -= Time.deltaTime;
                    move.FacingRight = FacingRight;
                }
            }

            if (m_CurrentAttackMove != null)
                return;

            foreach (MonsterAttackMove move in m_AttackMoveList)
            {
                if(move.CanMove() &&
                  (move.HasTarget() || move.CanMoveWithoutTarget))
                {
                    m_CurrentAttackMove = move;
                    StartMoveAttack();
                    return;
                }
            }
        }

        protected void StartMoveAttack()
        {
            if (m_CurrentAttackMove == null)
                return;

            CharacterAnimator.SetTrigger(m_CurrentAttackMove.MoveName);
            
            m_CurrentAttackMove.OnMoving = true;
            m_CurrentAttackMove.MoveCooldownTimer = m_CurrentAttackMove.MoveCooldown;
        }

        public void EndMoveAttack()
        {
            CancelMove();
        }

        public void CancelMove()
        {
            if (m_CurrentAttackMove == null)
                return;

            m_CurrentAttackMove.OnMoving = false;
            m_CurrentAttackMove = null;
        }

        protected void CheckContactHitter()
        {
            if (m_ContactHitter == null || !m_ContactHitter.Actived)
                return;

            m_ContactHitter.HitDirection = m_MoveVector;
            m_ContactHitter.HitDirection.x *= 2f;
            m_ContactHitter.HitDirection.y = 0.2f;
            m_ContactHitter.HitPower = 0.2f;
            m_ContactHitter.CheckTargetHit();
        }

        protected void CheckAttackHitter()
        {
            if (m_AttackHitter == null || !m_AttackHitter.Actived)
                return;

            m_AttackHitter.HitDirection.x = Mathf.Abs(m_AttackHitter.HitDirection.x);
            m_AttackHitter.HitDirection.x = FacingRight ? m_AttackHitter.HitDirection.x : -m_AttackHitter.HitDirection.x;
            m_AttackHitter.CheckTargetHit();
        }

        public void OrientForTarget()
        {
            if (m_EnemyTarget != null || IsDazed)
                return;

            FacingRight = !FacingRight;
        }

        public void OrientToTarget()
        {
            if (m_EnemyTarget == null || IsDazed)
                return;

            FacingRight = m_EnemyTarget.transform.position.x > transform.position.x;
        }

        public bool IsEnemyTargetSpotted()
        {
            return m_EnemyTarget != null;
        }

        public void Patrol()
        {
            if (m_EnemyTarget != null || IsDazed)
                return;

            if (FacingRight)
                m_MoveVector.x = MoveSpeed * Time.deltaTime;
            else
                m_MoveVector.x = -MoveSpeed * Time.deltaTime;
        }

        public void WalkToEnemyTarget()
        {
            if (m_EnemyTarget == null || IsDazed)
                return;

            if (m_EnemyTarget.transform.position.x > transform.position.x)
                m_MoveVector.x = MoveSpeed * Time.deltaTime;
            else if (m_EnemyTarget.transform.position.x < transform.position.x)
                m_MoveVector.x = -MoveSpeed * Time.deltaTime;
            CheckFacing();
        }

        public void Stand()
        {
            m_MoveVector.x = 0;
        }

        public void JumpWithPower(float jumpPower)
        {
            m_MoveVector.y += jumpPower;
        }

        public void Move(Vector2 moveVector)
        {
            moveVector.x = FacingRight ? moveVector.x : -moveVector.x;

            m_MoveVector.x = 0;
            m_MoveVector += moveVector * Time.deltaTime;
        }

        protected void CheckEnemyStillVisible()
        {
            if (m_EnemyTarget != null)
            {
                if (m_EnemyColliderInSensor[0] != null)
                    m_SensedTarget = m_EnemyTarget;
            }
            else
            {
                if (m_EnemyColliderInSensor[0] != null)
                    m_SensedTarget = m_EnemyColliderInSensor[0].gameObject;
            }

            if (m_SensedTarget == null)
                return;

            Vector3 toTarget = m_SensedTarget.transform.position - transform.position;
            Vector3 forward = FacingRight ? Vector3.right : Vector3.left;

            // Check Enemy in range
            if (toTarget.sqrMagnitude < m_LineOfSightRange * m_LineOfSightRange)
            {
                //Check Enemy in Line of sight Degree

                // Rotate Vector with Degree on z axis
                Vector3 testForward = Quaternion.Euler(0, 0, FacingRight ? -m_LineOfSightDegree : m_LineOfSightDegree) * forward;

                float angle = Vector3.Angle(testForward, toTarget);

                // 0.5f mean field pivot on center of forward vector
                if (angle <= m_LineOfSightField * 0.5f)
                {
                    //Debug.Log("See Player in Line of Sight");
                    m_TimeSinceLastTargetView = m_LostTargetTime;
                    m_EnemyTarget = m_SensedTarget;
                }
            }
            m_TimeSinceLastTargetView -= Time.deltaTime;

            if (m_TimeSinceLastTargetView <= 0.0f && m_LostTargetTime != -1f)
            {
                ForgetTarget();
            }
        }

        void ForgetTarget()
        {
            m_EnemyTarget = null;
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
            Vector3 forward = FacingRight ? Vector2.right : Vector2.left;
            forward = Quaternion.Euler(0, 0, FacingRight ? -m_LineOfSightDegree : m_LineOfSightDegree) * forward;

            Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, m_LineOfSightField * 0.5f) * forward);

            Handles.color = new Color(0, 1.0f, 0, 0.2f);
            Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, m_LineOfSightField, m_LineOfSightRange);
        }
#endif
    }
}
