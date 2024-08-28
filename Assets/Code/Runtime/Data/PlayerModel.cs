using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HeroFighter.Runtime
{
    public class PlayerModel
    {
        private const string IsHeroOwnedPrefPostfix = "_is_owned";
        private const string SelectedHeroPrefix = "selected_hero_";
        private const string HeroExperiencePostfix = "_experience";
        
        private static PlayerModel _instance;
        private readonly HeroConfiguration _heroConfig;

        private Dictionary<string, HeroDefinition> HeroDefCollection => _heroConfig.heroDefinitionCollection;
        
        public readonly List<string> SelectedHeroIdentifiers = new(Constants.MaxSelectableHeroCount);

        public static PlayerModel Instance
        {
            get { return _instance ??= new PlayerModel(GameContext.Instance.heroConfiguration); }
        }

        public PlayerModel(HeroConfiguration heroConfiguration)
        {
            _heroConfig = heroConfiguration;
            MakeSureInitialHeroesSetOwned();
            GetSelectedHeroes();
        }

        private void MakeSureInitialHeroesSetOwned()
        {
            foreach (var pair in HeroDefCollection)
            {
                if (pair.Value.initialHero)
                {
                    SetHeroOwned(pair.Key);
                }
            }
        }
        
        private void GetSelectedHeroes()
        {
            var ownedHeroes = GetOwnedHeroes();
            Assert.IsTrue(ownedHeroes.Count >= Constants.MaxSelectableHeroCount);
            for (int i = 0; i < Constants.MaxSelectableHeroCount; i++)
            {
                var id = GetSelectedHero(i);
                if (!String.IsNullOrEmpty(id))
                {
                    SelectedHeroIdentifiers.Add(id);
                }
                else
                {
                    SelectedHeroIdentifiers.Add(ownedHeroes[i]);
                }
            }
        }
        
        public void SaveSelectedHeroes()
        {
            for (var i = 0; i < SelectedHeroIdentifiers.Count; i++)
            {
                SetSelectedHero(i, SelectedHeroIdentifiers[i]);
            }
        }
        
        public List<string> GetOwnedHeroes()
        {
            var heroes = new List<string>();
            foreach (var pair in HeroDefCollection)
            {
                if (GetHeroOwned(pair.Key))
                {
                    heroes.Add(pair.Key);
                }
            }

            return heroes;
        }
        
        public List<string> GetUnownedHeroes()
        {
            var heroes = new List<string>();
            foreach (var pair in HeroDefCollection)
            {
                if (!GetHeroOwned(pair.Key))
                {
                    heroes.Add(pair.Key);
                }
            }

            return heroes;
        }
        
        public bool GetHeroOwned(string heroId)
        {
            Assert.IsTrue(HeroDefCollection.ContainsKey(heroId));

            if (HeroDefCollection[heroId].initialHero)
            {
                return true;
            }

            return PlayerPrefs.GetInt(GetHeroOwnedPrefKey(heroId), 0) == 1;
        }

        public void SetHeroOwned(string heroId)
        {
            Assert.IsTrue(HeroDefCollection.ContainsKey(heroId));
            PlayerPrefs.SetInt(GetHeroOwnedPrefKey(heroId), 1);
        }

        private string GetHeroOwnedPrefKey(string heroId)
        {
            return $"{heroId}{IsHeroOwnedPrefPostfix}";
        } 
        
        private string GetSelectedHero(int slotIndex)
        {
            Assert.IsTrue(slotIndex is >= 0 and < Constants.MaxSelectableHeroCount);
            return PlayerPrefs.GetString(GetSelectedHeroPrefKey(slotIndex), null);
        }
        
        private void SetSelectedHero(int slotIndex, string heroId)
        {
            Assert.IsTrue(slotIndex is >= 0 and < Constants.MaxSelectableHeroCount);
            PlayerPrefs.SetString(GetSelectedHeroPrefKey(slotIndex), heroId);
        }
        
        private string GetSelectedHeroPrefKey(int heroId)
        {
            return $"{SelectedHeroPrefix}{heroId}";
        }

        public int GetExperience(string heroId)
        {
            Assert.IsTrue(HeroDefCollection.ContainsKey(heroId));
            return PlayerPrefs.GetInt(GetExperiencePrefKey(heroId), 0);
        } 
        
        public void SetExperience(string heroId, int experience)
        {
            Assert.IsTrue(HeroDefCollection.ContainsKey(heroId));
            PlayerPrefs.SetInt(GetExperiencePrefKey(heroId), experience);
        } 
        
        private string GetExperiencePrefKey(string heroId)
        {
            return $"{heroId}{HeroExperiencePostfix}";
        }
        
        public int GetLevel(string heroId)
        {
            return GetExperience(heroId) / _heroConfig.experiencePerLevel;
        }
    }
}