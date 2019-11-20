using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Character;
using UnityEngine.Experimental.Rendering.LWRP;

namespace Main.Battle
{
    public class PlayerMeleeAttack : MonoBehaviour
    {
        public PlayerCharacter PlayerCharacter;
        public Hitter[] MeleeAttacks;
        public SpriteRenderer SlashSprite;
        public Light2D SlashLight;

        private readonly float DEFAULT_ATTACK_DELAY = 0.3f;

        private float m_AttackDelayCounter;
        private float m_StartAttackDelay;
        private bool m_MouseDown;
        private bool m_MouseClick;

        private Vector2 HIT_DIRECTION_LEFT = new Vector2(-0.15f, 0.15f);
        private Vector2 HIT_DIRECTION_RIGHT = new Vector2(0.15f, 0.15f);

        private void Update()
        {
            if(m_AttackDelayCounter > 0)
            {
                m_AttackDelayCounter -= Time.deltaTime;
                SlashSprite.color = Color.Lerp(Color.clear, Color.white, m_AttackDelayCounter / m_StartAttackDelay);
                SlashLight.intensity = Mathf.Lerp(0f, 1f, m_AttackDelayCounter / m_StartAttackDelay);
            }
            else
            {
                SlashSprite.color = Color.clear;
                SlashLight.intensity = 0f;
            }

            if(!m_MouseDown && Input.GetAxis("Fire1") > 0)
            {
                m_MouseDown = true;
            }

            if (m_MouseDown && Input.GetAxis("Fire1") == 0)
            {
                m_MouseClick = true;
                m_MouseDown = false;
            }

            if(m_MouseClick && m_AttackDelayCounter <= 0)
            {
                Attack(MeleeAttacks[0], DEFAULT_ATTACK_DELAY);
            }

            if (m_MouseClick)
                m_MouseClick = !m_MouseClick;
        }

        private void Attack(Hitter move, float atkDelay)
        {
            m_StartAttackDelay = atkDelay;
            m_AttackDelayCounter = atkDelay;

            if (MeleeAttacks.Length <= 0)
                return;

            move.HitDirection = PlayerCharacter.FacingRight ? HIT_DIRECTION_RIGHT : HIT_DIRECTION_LEFT;
            move.HitPower = 0.2f;
            move.CheckTargetHit();
        }
    }
}
