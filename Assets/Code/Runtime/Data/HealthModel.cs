using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime
{
    public class HealthModel
    {
        private readonly int _initialHealth;
        private int _health;
        
        public readonly UnityEvent<HealthModel> onHealthChangedEvent = new();

        public int Health
        {
            get => _health;
            private set
            {
                if (value == _health)
                {
                    return;
                }

                _health = Mathf.Clamp(value, 0, _initialHealth);
                onHealthChangedEvent?.Invoke(this);
            }
        }

        public int InitialHealth => _initialHealth;

        public bool LogHealth { get; set; }

        public HealthModel(int health, bool logHealth = false)
        {
            _initialHealth = health;
            Health = health;
            LogHealth = logHealth;
        }
        
        public void Increment(int amount)
        {
            Health += amount;
            if (LogHealth)
            {
                Debug.Log($"Increased health, amount: {amount}, new val: {Health}");
            }
        }
        
        public void Decrement(int amount)
        {
            Health -= amount;
            if (LogHealth)
            {
                Debug.Log($"Decreased health, amount: {amount}, new val: {Health}");
            }
        }
        
        public void Reset()
        {
            Health = _initialHealth;
            if (LogHealth)
            {
                Debug.Log($"Reset health, new val: {Health}");
            }
        }
    }
}