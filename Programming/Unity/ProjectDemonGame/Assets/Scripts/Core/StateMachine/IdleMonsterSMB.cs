﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Character;

public class IdleMonsterSMB : BehaviourLinkedSMB<MonsterCharacter>
{
    protected override void OnLinkedStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_MonoBehaviour == null)
            return;

        m_MonoBehaviour.Stand();
    }
}
