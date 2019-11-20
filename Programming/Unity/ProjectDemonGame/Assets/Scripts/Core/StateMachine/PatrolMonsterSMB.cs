using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Character;

public class PatrolMonsterSMB : BehaviourLinkedSMB<MonsterCharacter>
{
    protected override void OnLinkedStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        m_MonoBehaviour.OrientForTarget();
        m_MonoBehaviour.OrientToTarget();
    }

    protected override void OnLinkedStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        m_MonoBehaviour.Patrol();
        m_MonoBehaviour.WalkToEnemyTarget();
        animator.SetBool("TargetSpotted", m_MonoBehaviour.IsEnemyTargetSpotted());
    }
}
