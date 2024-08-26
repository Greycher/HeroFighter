using UnityEngine;
using UnityEngine.Events;

namespace HeroFighter.Runtime
{
    public class HeroModel
    {
        public readonly HeroDefinition heroDefinition;
        public readonly HealthModel healthModel;
        
        private readonly HeroConfiguration _heroConfiguration;

        public readonly UnityEvent onDied = new();

        public int AttackPower { get; }
        public int Level { get; }
        public string Identifier { get; }
        public bool Died => Mathf.Approximately(healthModel.Health, 0);

        public HeroModel(HeroConfiguration heroConfiguration, HeroDefinition heroDefinition, string identifier, int level, bool logHealth = false)
        {
            _heroConfiguration = heroConfiguration;
            this.heroDefinition = heroDefinition;
            Identifier = identifier;
            Level = level;
            AttackPower = CalculateAttackPower(level);
            var health = CalculateHealth(level);
            healthModel = new HealthModel(health, logHealth);
            healthModel.onHealthChangedEvent.AddListener(OnHealthChanged);
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
            var ap = heroDefinition.attackPower;
            return Mathf.FloorToInt(ap + ap * _heroConfiguration.attackPowerIncreasePerLevel * level);
        }

        public int Damage(HeroModel model)
        {
            model.TakeDamage(AttackPower);
            return AttackPower;
        }

        private void TakeDamage(int attackPower)
        {
            healthModel.Decrement(attackPower);
        }
    }
}