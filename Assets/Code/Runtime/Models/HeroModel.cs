using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace HeroFighter.Runtime
{
    [CreateAssetMenu(menuName = Constants.ProjectName + "/" + FileName, fileName = FileName)]
    public class HeroModel : SerializedScriptableObject
    {
        private const string FileName = nameof(HeroModel);
        private const string IsHeroOwnedPrefPostfix = "_is_owned";
        private const string SelectedHeroPrefix = "selected_hero_";

        public Dictionary<string, Hero> heroCollection = new();
        
        [NonSerialized] public List<string> SelectedHeroIdentifiers = new(Constants.MaxSelectableHeroCount);

        public int TotalHeroCount => heroCollection.Count;

        public void Initialize()
        {
            MakeSureInitialHeroesSetOwned();
            for (int i = 0; i < Constants.MaxSelectableHeroCount; i++)
            {
                var id = GetSelectedHeroIdentifier(i);
                if (!String.IsNullOrEmpty(id))
                {
                    SelectedHeroIdentifiers.Add(id);
                }
            }
        } 
 
        public void MakeSureInitialHeroesSetOwned()
        {
            foreach (var pair in heroCollection)
            {
                if (pair.Value.initialHero)
                {
                    SetHeroOwned(pair.Key);
                }
            }
        }

        public List<KeyValuePair<string, Hero>> GetOwnedHeroPairs()
        {
            var heroes = new List<KeyValuePair<string, Hero>>();
            foreach (var pair in heroCollection)
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
            Assert.IsTrue(heroCollection.ContainsKey(key));

            if (heroCollection[key].initialHero)
            {
                return true;
            }

            return PlayerPrefs.GetInt(GetIsHeroOwnedPrefKey(key), 0) == 1;
        }
        
        public void SetHeroOwned(string key)
        {
            Assert.IsTrue(heroCollection.ContainsKey(key));
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
            for (var i = 0; i < SelectedHeroIdentifiers.Count; i++)
            {
                SetSelectedHeroKey(i, SelectedHeroIdentifiers[i]);
            }
        }
    }
}