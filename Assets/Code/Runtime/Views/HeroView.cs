using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HeroFighter.Runtime.Views
{
    public class HeroView : SinglePointerHandler
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int defaultAnimState;
        [SerializeField, AnimatorState] private int selectedAnimState;
        [SerializeField, AnimatorState] private int attackAnimState;

        private RectTransform _rectTransform;
        
        public UnityEvent<PointerEventData> onPointerDown = new();
        public UnityEvent<PointerEventData, bool> onPointerUp = new();

        private RectTransform RectTransform
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
        
        public void UpdateView(string heroName)
        {
            label.text = heroName;
        }

        public void SetSelected(bool selected)
        {
            animator.Play(selected ? selectedAnimState : defaultAnimState);
        }

        public void OnAttack()
        {
            animator.Play(attackAnimState, 0, 0);
        }

        protected override void OnSinglePointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke(eventData);
        }
        
        protected override void OnSinglePointerDrag(PointerEventData eventData) {}
        
        protected override void OnSinglePointerUp(PointerEventData eventData)
        {
            var inside = RectTransformUtility.RectangleContainsScreenPoint(RectTransform, eventData.position,
                    eventData.pressEventCamera);
            onPointerUp.Invoke(eventData, inside);
        }
    }
}