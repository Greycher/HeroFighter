using System;
using UnityEngine;

namespace HeroFighter.Runtime
{
    [Serializable]
    public class HeroDefinition
    { 
        public string name;
        public int attackPower;
        [Tooltip("Check if hero should be contained in player's initial collection.")]
        public bool initialHero;
    }
}