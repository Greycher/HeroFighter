using System;
using UnityEngine;

namespace HeroFighter.Runtime
{
    [Serializable]
    public class Hero
    {
        public string name;
        public float attackPower;
        [Tooltip("Check if hero should be contained in player's initial collection.")]
        public bool initialHero;
    }
}