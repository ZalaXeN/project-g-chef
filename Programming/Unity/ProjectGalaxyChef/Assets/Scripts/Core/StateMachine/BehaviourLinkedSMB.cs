using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BehaviourLinkedSMB<TMonobehaviour> : SealedSMB where TMonobehaviour : MonoBehaviour
{
    protected TMonobehaviour m_MonoBehaviour;

    public static void Init(Animator animator, TMonobehaviour mono)
    {
        BehaviourLinkedSMB<TMonobehaviour>[] behaviourLinkedSMB = animator.GetBehaviours<BehaviourLinkedSMB<TMonobehaviour>>();

        for (int i = 0; i < behaviourLinkedSMB.Length ; i++)
        {
            behaviourLinkedSMB[i].InternalInit(animator, mono);
        }
    }

    protected void InternalInit(Animator animator, TMonobehaviour mono)
    {
        m_MonoBehaviour = mono;
        OnStart(animator);
    }

    public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        OnLinkedStateEnter(animator, stateInfo, layerIndex);
    }

    public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        if (!animator.gameObject.activeSelf)
            return;

        OnLinkedStateUpdate(animator, stateInfo, layerIndex);
    }

    public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        OnLinkedStateExit(animator, stateInfo, layerIndex);
    }

    //----------------------------------------

    protected virtual void OnStart(Animator animator)
    {

    }

    protected virtual void OnLinkedStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    protected virtual void OnLinkedStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    protected virtual void OnLinkedStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
