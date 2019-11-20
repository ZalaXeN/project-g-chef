using Main.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Character
{
    public class PlayerCharacter : CharacterModule
    {
        [SerializeField]
        protected SpriteRenderer m_SpriteRenderer;
        protected Color m_FlashColor;
        protected float m_FlashRepeatTime = 0.1f;

        protected float m_InvincibleTimer = 0f;

        protected readonly Vector3 FLIP_ROTATE_SCALE = new Vector3(-1, 1, 1);

        protected override void Update()
        {
            UpdateInvincibleTime();
            HorizontalMovement();
            VerticalMovement();
            CheckHeadHit();
            CheckGroundHit();
            UpdateKnockback();
            CharacterMovement.Move(m_MoveVector);
            UpdateAnimation();
        }

        protected void UpdateInvincibleTime()
        {
            if (m_InvincibleTimer <= 0)
            {
                ResetFlashSprite();
                return;
            }

            m_InvincibleTimer -= Time.deltaTime;
        }

        protected void FlashSprite()
        {
            m_FlashColor = m_SpriteRenderer.color;
            if (m_SpriteRenderer.color.a > 0)
                m_FlashColor.a = 0;
            else
                m_FlashColor.a = 1;

            m_SpriteRenderer.color = m_FlashColor;
        }

        protected void ResetFlashSprite()
        {
            CancelInvoke("FlashSprite");
            m_FlashColor = m_SpriteRenderer.color;
            m_FlashColor.a = 1;
            m_SpriteRenderer.color = m_FlashColor;
        }

        protected override void UpdateAnimation()
        {
            CharacterAnimator.SetBool("Grounded", CharacterMovement.IsGrounded);

            //if(m_MoveVector.x != 0)
            //    CharacterSpriteRenderer.flipX = m_MoveVector.x < 0;

            if (FacingRight)
                transform.localScale = FLIP_ROTATE_SCALE;
            else
                transform.localScale = Vector3.one;

            if (m_MoveVector.x < 0)
                CharacterAnimator.SetInteger("Horizontal", -1);
            else if (m_MoveVector.x > 0)
                CharacterAnimator.SetInteger("Horizontal", 1);
            else
                CharacterAnimator.SetInteger("Horizontal", 0);

            if (m_MoveVector.y < 0)
                CharacterAnimator.SetInteger("Vertical", -1);
            else if (m_MoveVector.y > 0)
                CharacterAnimator.SetInteger("Vertical", 1);
            else
                CharacterAnimator.SetInteger("Vertical", 0);
        }

        protected override void HorizontalMovement()
        {
            float moveDirection = Input.GetAxis("Horizontal");
            if (moveDirection != 0)
            {
                m_MoveVector.x = MoveSpeed * moveDirection * Time.deltaTime;
            }
            else
            {
                m_MoveVector.x = 0;
            }

            CheckFacing();
        }

        protected override void VerticalMovement()
        {
            if (GameInputManager.GetKey(DefaultGameInput.JUMP_KEY_CODE))
            {
                JumpInputHandle();
            }
            else
            {
                m_JumpHold = false;
            }

            GravityAttract();
        }

        void JumpInputHandle()
        {
            // Knockback can't Jump
            // TODO Air Break
            if (m_KnockbackPower > 0)
                return;

            if (CharacterMovement.IsGrounded)
            {
                if (m_MoveVector.y == 0 && m_JumpCounter == 0 && !m_JumpHold)
                {
                    Jump();
                }
            }
            else
            {
                if (CharacterMovement.IsHeadHit)
                    return;

                if (m_MoveVector.y > 0 && m_JumpHold && m_JumpPower < MaxJumpSpeed)
                {
                    AddJumpPower();
                }
                else if (!m_JumpHold && m_JumpCounter < JumpNumber)
                {
                    Jump();
                }
            }
            //Debug.Log(m_JumpCounter);
        }

        public void HandleGroundHit()
        {
            if (CharacterMovement.IsGrounded && 
                m_JumpCounter > 0 && m_MoveVector.y <= 0)
            {
                //Debug.Log("Reset Jump");
                m_JumpPower = 0;
                m_JumpCounter = 0;
            }
        }

        public void HandleLandingOnEnemy()
        {
            if (CharacterMovement.IsGroundHitTag("Enemy") &&
                m_MoveVector.y <= 0)
            {
                //AttackJump();
            }
        }

        public void HandleHeadHit()
        {
            // Jump and Head Hit
            // -- Reset Jump Power
            if (CharacterMovement.IsHeadHit && !CharacterMovement.IsGrounded && m_MoveVector.y > 0)
            {
                m_JumpPower = 0;
                m_MoveVector.y = 0;
            }
        }

        public void AttackJump()
        {
            m_MoveVector.y += StartJumpSpeed * START_JUMP_SCALE;
        }

        public void TakeHit(Hitter hitter)
        {
            if (m_InvincibleTimer > 0f && !hitter.IgnoreInvincible)
                return;

            m_InvincibleTimer = Player.InvincibleTime;
            Knockback(hitter.HitDirection, hitter.HitPower);

            if(!IsInvoking("FlashSprite"))
                InvokeRepeating("FlashSprite", 0f, m_FlashRepeatTime);
        }
    }
}
