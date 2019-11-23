using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Character;

public class AttackMonsterSMB : BehaviourLinkedSMB<MonsterCharacter>
{
    public float JumpPower;
    public bool IsMoveFollowTarget;
    public Vector2 AttackMoveVector;

    protected override void OnLinkedStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        if(JumpPower != 0)
            m_MonoBehaviour.JumpWithPower(JumpPower);
    }

    protected override void OnLinkedStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        if(AttackMoveVector != Vector2.zero)
            m_MonoBehaviour.Move(AttackMoveVector);

        if (IsMoveFollowTarget)
            m_MonoBehaviour.WalkToEnemyTarget();
    }

    protected override void OnLinkedStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        m_MonoBehaviour.EndMoveAttack();
    }
}
