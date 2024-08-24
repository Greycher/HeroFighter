using System;
using System.Collections.Generic;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroSelectionPresenter : MonoBehaviour
    {
        [SerializeField] private HeroView heroViewTemplate; 
        [SerializeField] private Button notOwnedHeroViewTemplate;
        [SerializeField] private Button battleButton;
        
        private HeroModel _heroModel;
        private List<HeroView> _heroViews = new List<HeroView>();
        private List<Button> _notOwnedHeroViews = new List<Button>();

        private void Awake()
        {
            heroViewTemplate.gameObject.SetActive(false);
            notOwnedHeroViewTemplate.gameObject.SetActive(false);
            _heroModel = GameContext.Instance.heroModel;
        }

        private void Start()
        {
            var ownedHeroPairs = _heroModel.GetOwnedHeroPairs();
            foreach (var pair in ownedHeroPairs)
            {
                var view = Instantiate(heroViewTemplate, heroViewTemplate.transform.parent);
                view.gameObject.SetActive(true);
                view.UpdateView(pair.Value, pair.Key);
                view.OnClicked.AddListener(OnHeroViewClicked);
                view.SetSelected(_heroModel.SelectedHeroIdentifiers.Contains(pair.Key));
                _heroViews.Add(view);
            }

            for (int i = ownedHeroPairs.Count; i < _heroModel.TotalHeroCount; i++)
            {
                var view = Instantiate(notOwnedHeroViewTemplate, notOwnedHeroViewTemplate.transform.parent);
                view.gameObject.SetActive(true);
                view.onClick.AddListener(OnNotOwnedHeroViewClicked);
                _notOwnedHeroViews.Add(view);
            }
            
            battleButton.onClick.AddListener(OnBattleClicked);
        }

        private void OnDestroy()
        {
            foreach (var view in _heroViews)
            {
                view.OnClicked.RemoveListener(OnHeroViewClicked);
            }
            
            foreach (var view in _notOwnedHeroViews)
            {
                view.onClick.RemoveListener(OnNotOwnedHeroViewClicked);
            }
            
            battleButton.onClick.RemoveListener(OnBattleClicked);
        }

        private void OnHeroViewClicked(HeroView heroView, string heroIdentifier)
        {
            if (_heroModel.SelectedHeroIdentifiers.Remove(heroIdentifier))
            {
                heroView.SetSelected(false);
                return;
            }

            if (_heroModel.SelectedHeroIdentifiers.Count == Constants.MaxSelectableHeroCount)
            {
                //TODO toast this here
                return;
            }
            
            _heroModel.SelectedHeroIdentifiers.Add(heroIdentifier);
            heroView.SetSelected(true);
        }
        
        private void OnNotOwnedHeroViewClicked()
        {
            //TODO toast this here
        }
        
        private void OnBattleClicked()
        {
            if (_heroModel.SelectedHeroIdentifiers.Count != Constants.MaxSelectableHeroCount)
            {
                //TODO toast this here
                return;
            }

            _heroModel.SaveSelectedHeroes();
            SceneManager.LoadScene(GameContext.Instance.battleSceneIndex);
        }
    }
}