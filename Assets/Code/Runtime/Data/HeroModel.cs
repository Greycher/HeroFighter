using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime
{
    public class HeroModel
    {
        private readonly HeroDefinition _heroDefinition;
        private readonly HealthModel _healthModel;
        
        private readonly HeroConfiguration _heroConfiguration;

        public readonly UnityEvent onDied = new();

        public HeroDefinition HeroDefinition => _heroDefinition;
        public HealthModel HealthModel => _healthModel;
        public int AttackPower { get; }
        public int Level { get; }
        public string Identifier { get; }
        public int Health => _healthModel.Health;
        public bool Died => Mathf.Approximately(Health, 0);

        public HeroModel(HeroConfiguration heroConfiguration, HeroDefinition heroDefinition, string identifier, int level, bool logHealth = false)
        {
            _heroConfiguration = heroConfiguration;
            this._heroDefinition = heroDefinition;
            Identifier = identifier;
            Level = level;
            AttackPower = CalculateAttackPower(level);
            var health = CalculateHealth(level);
            _healthModel = new HealthModel(health, logHealth);
            _healthModel.onHealthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnHealthChanged(HealthModel healthModel)
        {
            if (Died)
            {
                onDied.Invoke();
            }
        }
        
        private int CalculateHealth(int level)
        {
            var h = _heroConfiguration.baseHeroHealth;
            return  Mathf.FloorToInt(h + h * _heroConfiguration.healthIncreasePerLevel * level);
        }
        
        private int CalculateAttackPower(int level)
        {
            var ap = _heroDefinition.attackPower;
            return Mathf.FloorToInt(ap + ap * _heroConfiguration.attackPowerIncreasePerLevel * level);
        }

        public int Damage(HeroModel model)
        {
            model.TakeDamage(AttackPower);
            return AttackPower;
        }

        private void TakeDamage(int attackPower)
        {
            _healthModel.Decrement(attackPower);
        }
    }
}