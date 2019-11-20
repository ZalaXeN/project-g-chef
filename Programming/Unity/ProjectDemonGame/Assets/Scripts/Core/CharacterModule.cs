using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Main.Battle;

namespace Main.Character
{
    [System.Serializable]
    public class HeadHitEvent : UnityEvent
    {

    }

    [System.Serializable]
    public class LandingEvent : UnityEvent
    {

    }

    [System.Serializable]
    public class JumpEvent : UnityEvent
    {

    }

    public class CharacterModule : MonoBehaviour
    {
        public CharacterMovement CharacterMovement;
        public Animator CharacterAnimator;

        public float Gravity = -0.98f;
        public float MoveSpeed = 10f;
        public float StartJumpSpeed = 15f;
        public float MaxJumpSpeed = 25f;
        public float SensitiveJumpSpeed = 1f;
        public int JumpNumber = 1;
        public bool FacingRight = false;

        public HeadHitEvent OnHeadHit;
        public LandingEvent OnLanding;
        public JumpEvent OnJump;

        protected Vector2 m_MoveVector = new Vector2();
        protected float m_GravitySpeed;

        protected bool m_JumpHold;
        protected int m_JumpCounter;
        protected float m_JumpPower;

        protected Vector2 m_KnockbackDirection;
        protected float m_KnockbackPower;

        protected const float START_JUMP_SCALE = 0.0175f;

        protected virtual void Update()
        {
            HorizontalMovement();
            VerticalMovement();
            CheckHeadHit();
            CheckGroundHit();
            UpdateKnockback();
            CharacterMovement.Move(m_MoveVector);
            UpdateAnimation();
        }

        protected virtual void UpdateAnimation()
        {
            
        }

        protected virtual void HorizontalMovement()
        {
            m_MoveVector.x = 0;
            CheckFacing();
        }

        protected virtual void CheckFacing()
        {
            if (m_MoveVector.x != 0)
            {
                FacingRight = m_MoveVector.x > 0;
            }
        }

        protected virtual void VerticalMovement()
        {
            GravityAttract();
        }

        protected virtual void CheckHeadHit()
        {
            if (CharacterMovement.IsHeadHit)
                OnHeadHit.Invoke();
        }

        protected virtual void CheckGroundHit()
        {
            if (CharacterMovement.IsGrounded)
                OnLanding.Invoke();
        }

        protected virtual void GravityAttract()
        {
            if (CharacterMovement.IsGrounded)
            {
                if (m_MoveVector.y < 0)
                {
                    m_MoveVector.y = 0;
                }
            }
            else
            {
                m_GravitySpeed = Gravity * Time.deltaTime;
                m_MoveVector.y += m_GravitySpeed;
            }
        }

        protected virtual void Knockback(Vector2 hitDirection, float hitPower)
        {
            m_KnockbackDirection = hitDirection;
            if (m_KnockbackDirection.x == 0f)
                m_KnockbackDirection.x = FacingRight ? -0.2f : 0.2f;

            if (m_KnockbackDirection.y > 0.3f)
                m_KnockbackDirection.y = 0.3f;

            m_KnockbackPower = hitPower;
        }

        protected virtual void UpdateKnockback()
        {
            if (m_KnockbackPower > 0)
            {
                m_MoveVector.x = 0;
                m_KnockbackPower -= Time.deltaTime;
                m_MoveVector += (m_KnockbackDirection);
                m_KnockbackDirection.y = 0f;

                if (m_KnockbackPower <= 0 && !CharacterMovement.IsGrounded)
                    m_KnockbackPower = Time.deltaTime;
            }
        }

        protected virtual void Jump()
        {
            //Debug.Log("Jump");
            m_JumpCounter += 1;
            m_JumpPower = StartJumpSpeed;
            m_MoveVector.y = StartJumpSpeed * START_JUMP_SCALE;
            m_JumpHold = true;
            OnJump.Invoke();
        }

        protected virtual void AddJumpPower()
        {
            //Debug.Log("Add Jump Power");
            m_JumpPower += SensitiveJumpSpeed;
            m_MoveVector.y += SensitiveJumpSpeed * Time.deltaTime;
        }
    }
}
