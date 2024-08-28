using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HeroFighter.Runtime
{
    [CreateAssetMenu(menuName = Constants.ProjectName + "/" + FileName, fileName = FileName)]
    public class HeroConfiguration : SerializedScriptableObject
    {
        private const string FileName = nameof(HeroConfiguration);

        public Dictionary<string, HeroDefinition> heroDefinitionCollection = new();
        public int baseHeroHealth = 100;
        public int experiencePerLevel = 5;
        public float healthIncreasePerLevel = 0.1f;
        public float attackPowerIncreasePerLevel = 0.1f;
        public float heroInfoInputHoldDuration = 3f;
        [Tooltip("If true, drag input will not be executed in the case of player dragging out of hero's rect.")]
        public bool cancelHoldInputOnDragOutside;
        public int experienceIncreasePerWin = 1;
        public int unlockNewHeroEveryXLevel = 5;
        
        public int TotalHeroCount => heroDefinitionCollection.Count;
    }
}