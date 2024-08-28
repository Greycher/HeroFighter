using UnityEngine;

namespace HeroFighter.Runtime
{
    public class AnimationEndBroadcaster : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            animator.gameObject.SendMessage("OnAnimationEnd", stateInfo.shortNameHash);
        }
    }
}