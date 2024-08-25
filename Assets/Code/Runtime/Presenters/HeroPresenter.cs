using System;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroPresenter : MonoBehaviour
    {
        [SerializeField] private HeroView heroView;
        [SerializeField] private bool logHealth;
        
        private HeroModel _heroModel;

        public UnityEvent<HeroPresenter> onClicked = new();
        public UnityEvent<HeroPresenter> onDied = new();
        public HeroModel HeroModel => _heroModel;
        public HeroView HeroView => heroView;

        public void Present(HeroModel heroModel)
        {
            _heroModel = heroModel;
            heroModel.healthModel.LogHealth = logHealth;
            heroView.UpdateView(heroModel.heroDefinition.name[^1]);
            _heroModel?.onDied.RemoveListener(OnDied);
            _heroModel?.onDied.AddListener(OnDied);
        }

        private void OnEnable()
        {
            heroView.OnClicked.AddListener(OnClick);
            _heroModel?.onDied.RemoveListener(OnDied);
            _heroModel?.onDied.AddListener(OnDied);
        }

        private void OnDisable()
        {
            heroView.OnClicked.RemoveListener(OnClick);
            _heroModel?.onDied.RemoveListener(OnDied);
        }

        private void OnClick()
        {
            onClicked.Invoke(this);
        }
        
        private void OnDied()
        {
            onDied.Invoke(this);
        }

        public void Attack(HeroModel randomHero)
        {
            HeroModel.Damage(randomHero);
            HeroView.OnAttack();
        }
    }
}