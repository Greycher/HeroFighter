using System.Collections.Generic;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroSelectionPresenter : MonoBehaviour
    {
        [SerializeField] private HeroPresenter heroPresenterTemplate; 
        [SerializeField] private Button notOwnedHeroViewTemplate;
        [SerializeField] private Button battleButton;
        
        private HeroConfiguration _heroConfiguration;
        private readonly List<HeroPresenter> _heroPresenters = new List<HeroPresenter>();
        private readonly List<Button> _notOwnedHeroViews = new List<Button>();

        private void Awake()
        {
            heroPresenterTemplate.gameObject.SetActive(false);
            notOwnedHeroViewTemplate.gameObject.SetActive(false);
            _heroConfiguration = GameContext.Instance.heroConfiguration;
        }

        private void Start()
        {
            var hc = _heroConfiguration;
            PopulatedHeroPresenters(hc);
            battleButton.onClick.AddListener(OnBattleClicked);
        }

        private void PopulatedHeroPresenters(HeroConfiguration hc)
        {
            PopulateOwnedHeroPresenters(hc);
            PopulateNotOwnedHeroViews();
        }

        private void PopulateOwnedHeroPresenters(HeroConfiguration hc)
        {
            var ownedHeroPairs = hc.GetOwnedHeroPairs();
            foreach (var pair in ownedHeroPairs)
            {
                var id = pair.Key;
                var heroDef = pair.Value;

                var presenter = Instantiate(heroPresenterTemplate, heroPresenterTemplate.transform.parent);
                presenter.gameObject.SetActive(true);
                presenter.onClicked.AddListener(OnHeroClicked);
                var hero = new HeroModel(hc, heroDef, id, hc.GetLevel(id));
                presenter.Present(hero);
                presenter.HeroView.SetSelected(_heroConfiguration.selectedHeroIdentifiers.Contains(pair.Key));
                _heroPresenters.Add(presenter);
            }
        }

        private void PopulateNotOwnedHeroViews()
        {
            for (int i = _heroPresenters.Count; i < _heroConfiguration.TotalHeroCount; i++)
            {
                var view = Instantiate(notOwnedHeroViewTemplate, notOwnedHeroViewTemplate.transform.parent);
                view.gameObject.SetActive(true);
                view.onClick.AddListener(OnNotOwnedHeroViewClicked);
                _notOwnedHeroViews.Add(view);
            }
        }

        private void OnDestroy()
        {
            foreach (var presenter in _heroPresenters)
            {
                presenter.onClicked.RemoveListener(OnHeroClicked);
            }
            
            foreach (var view in _notOwnedHeroViews)
            {
                view.onClick.RemoveListener(OnNotOwnedHeroViewClicked);
            }
            
            battleButton.onClick.RemoveListener(OnBattleClicked);
        }

        private void OnHeroClicked(HeroPresenter presenter)
        {
            if (_heroConfiguration.selectedHeroIdentifiers.Remove(presenter.HeroModel.Identifier))
            {
                presenter.HeroView.SetSelected(false);
                return;
            }

            if (_heroConfiguration.selectedHeroIdentifiers.Count == Constants.MaxSelectableHeroCount)
            {
                //TODO toast this here
                return;
            }
            
            _heroConfiguration.selectedHeroIdentifiers.Add(presenter.HeroModel.Identifier);
            presenter.HeroView.SetSelected(true);
        }
        
        private void OnNotOwnedHeroViewClicked()
        {
            //TODO toast this here
        }
        
        private void OnBattleClicked()
        {
            if (_heroConfiguration.selectedHeroIdentifiers.Count != Constants.MaxSelectableHeroCount)
            {
                //TODO toast this here
                return;
            }

            _heroConfiguration.SaveSelectedHeroes(); //Not necessary though convenient for the player
            SceneManager.LoadScene(GameContext.Instance.battleSceneIndex);
        }
    }
}