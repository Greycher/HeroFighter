using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime.Views
{
    public class DamageNumberEffectView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int floatingAnimState;
        
        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                {
                    _rectTransform = transform as RectTransform;
                }

                return _rectTransform;
            }
        }

        public UnityEvent<DamageNumberEffectView> onFloatingAnimEnd = new();
        
        public void Play(int number)
        {
            label.text = $"-{number}";
            animator.Play(floatingAnimState);
        }
        
        //Note for reviewers
        //Not ideal but would love hear if you have any better solution for getting animation end callback
        /// <summary>
        /// <seealso cref="AnimationEndBroadcaster"/>
        /// </summary>
        /// <param name="stateHash"></param>
        private void OnAnimationEnd(int stateHash)
        {
            if (stateHash == floatingAnimState)
            {
                onFloatingAnimEnd?.Invoke(this);
            }
        }
    }
}