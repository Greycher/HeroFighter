using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class LevelEndMenuView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private HeroView newHeroView;
        [SerializeField] private HeroStatUpView heroStatUpViewTemplate;
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int successAnimState;
        [SerializeField, AnimatorState] private int failAnimState;

        public UnityEvent OnContinuePressed => button.onClick;

        private void Awake()
        {
            heroStatUpViewTemplate.gameObject.SetActive(false);
        }
        
        public void UpdateView(bool playerWin)
        {
            animator.Play(playerWin ? successAnimState : failAnimState);
        }

        public void EnableNewHeroView(string heroName)
        {
            newHeroView.gameObject.SetActive(true);
            newHeroView.UpdateView(heroName);
        }

        public HeroStatUpView GetHeroStatUpView(string heroName)
        {
            var view = Instantiate(heroStatUpViewTemplate, heroStatUpViewTemplate.transform.parent);
            view.gameObject.SetActive(true);
            view.UpdateView(heroName);
            return view;
        }
    }
}