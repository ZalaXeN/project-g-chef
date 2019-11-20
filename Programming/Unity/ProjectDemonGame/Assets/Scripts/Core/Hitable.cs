using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Battle
{
    [System.Serializable]
    public class TakeHitEvent : UnityEvent<Hitter>
    {

    }
    
    public class Hitable : MonoBehaviour
    {
        public TakeHitEvent OnTakeHit;
    }
}
