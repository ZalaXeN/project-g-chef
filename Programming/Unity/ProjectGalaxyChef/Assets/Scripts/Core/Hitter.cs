using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Battle
{
    [System.Serializable]
    public class HitEvent : UnityEvent<Hitter>
    {
        
    }

    public class Hitter : MonoBehaviour
    {
        public Collider2D AttackCollider;
        public LayerMask TargetLayer;
        public bool IgnoreInvincible;
        public bool Actived;
        public float HitPower;
        public Vector2 HitDirection;
        public HitEvent OnHit;

        private ContactFilter2D m_ContactFilter;
        private List<Collider2D> m_HitColliderList = new List<Collider2D>();

        private void Start()
        {
            // Knowledge
            // layermask = layermask[1] | layermask[2] | ...;

            m_ContactFilter.layerMask = TargetLayer;
            m_ContactFilter.useLayerMask = true;
        }

        public void CheckTargetHit()
        {
            m_HitColliderList.Clear();
            if (AttackCollider.OverlapCollider(m_ContactFilter, m_HitColliderList) > 0)
            {
                foreach(Collider2D col in m_HitColliderList)
                {
                    Hitable hit = col.GetComponent<Hitable>();
                    if(hit != null)
                    {
                        hit.OnTakeHit.Invoke(this);
                    }
                }

                OnHit.Invoke(this);
            }
        }

        public List<Collider2D> GetHitColliderList()
        {
            return m_HitColliderList;
        }
    }
}
