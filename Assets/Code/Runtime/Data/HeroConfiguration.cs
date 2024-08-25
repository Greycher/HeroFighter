using System;
using System.Collections.Generic;
using HeroFighter.Runtime.Views;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace HeroFighter.Runtime
{
    [CreateAssetMenu(menuName = Constants.ProjectName + "/" + FileName, fileName = FileName)]
    public class HeroConfiguration : SerializedScriptableObject
    {
        private const string FileName = nameof(HeroConfiguration);
        private const string IsHeroOwnedPrefPostfix = "_is_owned";
        private const string SelectedHeroPrefix = "selected_hero_";

        public Dictionary<string, HeroDefinition> heroDefinitionCollection = new();
        public HeroInfoPopupView heroInfoPopupViewPrefab;
        public int baseHeroHealth = 100;
        public int experiencePerLevel = 5;
        public float healthIncreasePerLevel = 0.1f;
        public float attackPowerIncreasePerLevel = 0.1f;
        public float heroInfoInputHoldDuration = 3f;
        
        [NonSerialized] public readonly List<string> selectedHeroIdentifiers = new(Constants.MaxSelectableHeroCount);

        public int TotalHeroCount => heroDefinitionCollection.Count;

        public void Initialize()
        {
            MakeSureInitialHeroesSetOwned();
            for (int i = 0; i < Constants.MaxSelectableHeroCount; i++)
            {
                var id = GetSelectedHeroIdentifier(i);
                if (!String.IsNullOrEmpty(id))
                {
                    selectedHeroIdentifiers.Add(id);
                }
            }
        } 
 
        public void MakeSureInitialHeroesSetOwned()
        {
            foreach (var pair in heroDefinitionCollection)
            {
                if (pair.Value.initialHero)
                {
                    SetHeroOwned(pair.Key);
                }
            }
        }

        public List<KeyValuePair<string, HeroDefinition>> GetOwnedHeroPairs()
        {
            var heroes = new List<KeyValuePair<string, HeroDefinition>>();
            foreach (var pair in heroDefinitionCollection)
            {
                if (GetIsHeroOwned(pair.Key))
                {
                    heroes.Add(pair);
                }
            }

            return heroes;
        }

        public bool GetIsHeroOwned(string key)
        {
            Assert.IsTrue(heroDefinitionCollection.ContainsKey(key));

            if (heroDefinitionCollection[key].initialHero)
            {
                return true;
            }

            return PlayerPrefs.GetInt(GetIsHeroOwnedPrefKey(key), 0) == 1;
        }
        
        public void SetHeroOwned(string key)
        {
            Assert.IsTrue(heroDefinitionCollection.ContainsKey(key));
            PlayerPrefs.SetInt(GetIsHeroOwnedPrefKey(key), 1);
        }

        private string GetIsHeroOwnedPrefKey(string key)
        {
            return $"{key}{IsHeroOwnedPrefPostfix}";
        } 
        
        private string GetSelectedHeroIdentifier(int index)
        {
            Assert.IsTrue(index < Constants.MaxSelectableHeroCount);
            return PlayerPrefs.GetString(GetSelectedHeroPrefKey(index), null);
        }
        
        private void SetSelectedHeroKey(int index, string key)
        {
            Assert.IsTrue(index < Constants.MaxSelectableHeroCount);
            PlayerPrefs.SetString(GetSelectedHeroPrefKey(index), key);
        }
        
        private string GetSelectedHeroPrefKey(int index)
        {
            return $"{SelectedHeroPrefix}{index}";
        }

        public void SaveSelectedHeroes()
        {
            for (var i = 0; i < selectedHeroIdentifiers.Count; i++)
            {
                SetSelectedHeroKey(i, selectedHeroIdentifiers[i]);
            }
        }
        
        private const string HeroExperiencePostfix = "_experience";

        public int GetExperience(string heroId)
        {
            return PlayerPrefs.GetInt(GetExperiencePrefKey(heroId), 0);
        } 
        
        public void SetExperience(string heroId, int experience)
        {
            PlayerPrefs.SetInt(GetExperiencePrefKey(heroId), experience);
        } 
        
        private string GetExperiencePrefKey(string heroId)
        {
            return $"{heroId}{HeroExperiencePostfix}";
        }
        
        public int GetLevel(string heroId)
        {
            return GetExperience(heroId) / experiencePerLevel;
        }
    }
}