using HeroFighter.Runtime.Views;
using UnityEngine;

namespace HeroFighter.Runtime.Presenters
{
    public class HealthPresenter : MonoBehaviour
    {
        [SerializeField] private HealthView healthView;
        
        private HealthModel _healthModel;

        public void Present(HealthModel healthModel)
        {
            _healthModel = healthModel;
            healthView.UpdateView(GetHealthPercentage(healthModel), true);
            healthModel.onHealthChangedEvent.RemoveListener(OnHealthChanged);
            healthModel.onHealthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnHealthChanged(HealthModel healthModel)
        {
            healthView.UpdateView(GetHealthPercentage(healthModel));
        }

        private void OnEnable()
        {
            _healthModel?.onHealthChangedEvent.RemoveListener(OnHealthChanged);
            _healthModel?.onHealthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnDisable()
        {
            _healthModel?.onHealthChangedEvent.AddListener(OnHealthChanged);
        }

        private float GetHealthPercentage(HealthModel healthModel)
        {
            return healthModel.Health / (float)healthModel.InitialHealth;
        }
    }
}