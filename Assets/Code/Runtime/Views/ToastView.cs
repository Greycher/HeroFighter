using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime.Views
{
    public class ToastView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int toastLabelAnimState;

        public UnityEvent<ToastView> onAnimationEnd = new();

        public string Message => label.text;
        
        public void ToastMessage(string message)
        {
            label.text = message;
            animator.Play(toastLabelAnimState);
        }
        
        //Note for reviewers
        //Not ideal but would love hear if you have any better solution for getting animation end callback
        /// <summary>
        /// <seealso cref="AnimationEndBroadcaster"/>
        /// </summary>
        /// <param name="stateHash"></param>
        private void OnAnimationEnd(int stateHash)
        {
            if (stateHash == toastLabelAnimState)
            {
                onAnimationEnd?.Invoke(this);
            }
        }
    }
}