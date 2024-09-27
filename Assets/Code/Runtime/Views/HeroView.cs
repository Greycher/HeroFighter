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
        [SerializeField] private Transform damageNumberSpawnPointTr;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int defaultAnimState;
        [SerializeField, AnimatorState] private int selectedAnimState;
        [SerializeField, AnimatorState] private int attackAnimState;
        [SerializeField, AnimatorState] private int deathAnimation;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        
        public UnityEvent<PointerEventData> onPointerDown = new();
        public UnityEvent<PointerEventData, bool> onPointerDrag = new();
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

        public Vector2 ScreenPos => WorldToScreenPos(transform.position);

        public Vector3 DamageNumberSpawnPoint => WorldToScreenPos(damageNumberSpawnPointTr.position);

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
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
            animator.Play(attackAnimState);
        }

        public void OnDeath()
        {
            animator.Play(deathAnimation);
        }

        protected override void OnSinglePointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke(eventData);
        }

        protected override void OnSinglePointerDrag(PointerEventData eventData)
        {
            var inside = RectTransformUtility.RectangleContainsScreenPoint(RectTransform, eventData.position,
                eventData.pressEventCamera);
            onPointerDrag.Invoke(eventData, inside);
        }
        
        protected override void OnSinglePointerUp(PointerEventData eventData)
        {
            var inside = RectTransformUtility.RectangleContainsScreenPoint(RectTransform, eventData.position,
                    eventData.pressEventCamera);
            onPointerUp.Invoke(eventData, inside);
        }

        private Vector2 WorldToScreenPos(Vector3 pos)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            return screenPos / _canvas.scaleFactor;
        }
    }
}