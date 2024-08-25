using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class HeroView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Button button;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int defaultAnimState;
        [SerializeField, AnimatorState] private int selectedAnimState;
        [SerializeField, AnimatorState] private int attackAnimState;
        
        public UnityEvent OnClicked => button.onClick;

        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        public void UpdateView(char c)
        {
            label.text = c.ToString();
        }

        public void SetSelected(bool selected)
        {
            animator.Play(selected ? selectedAnimState : defaultAnimState);
        }

        public void OnAttack()
        {
            animator.Play(attackAnimState, 0, 0);
        }
        
        private void OnDestroy()
        {
            OnClicked.RemoveAllListeners();
        }
    }
}