using UnityEngine;

/// For Better Performance with Sealed Class
/// --------------------------
public abstract class SealedSMB : StateMachineBehaviour
{
    public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
