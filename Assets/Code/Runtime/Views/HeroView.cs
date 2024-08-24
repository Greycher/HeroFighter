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
        
        public UnityEvent<HeroView, string> OnClicked = new();
        private string _heroIdentifier;

        private void OnEnable()
        {
            button.onClick.AddListener(OnBtnClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnBtnClicked);
        }

        private void OnBtnClicked()
        {
            OnClicked.Invoke(this, _heroIdentifier);
        }

        public void UpdateView(Hero hero, string heroIdentifier)
        {
            _heroIdentifier = heroIdentifier;
            label.text = hero.name[^1].ToString();
        }

        public void SetSelected(bool selected)
        {
            animator.Play(selected ? selectedAnimState : defaultAnimState);
        }

        private void OnDestroy()
        {
            OnClicked.RemoveAllListeners();
        }
    }
}