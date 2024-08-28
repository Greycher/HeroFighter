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
        [SerializeField] private HeroInfoPopupView heroInfoPopupView;
        [SerializeField] private Button notOwnedHeroViewTemplate;
        [SerializeField] private Button battleButton;
        
        private readonly List<HeroPresenter> _heroPresenters = new List<HeroPresenter>();
        private readonly List<Button> _notOwnedHeroViews = new List<Button>();
        
        private HeroConfiguration _heroConfiguration;
        private ToastPresenter _toastPresenter;
        private PlayerModel _playerModel;

        private void Awake()
        {
            heroPresenterTemplate.gameObject.SetActive(false);
            notOwnedHeroViewTemplate.gameObject.SetActive(false);
            _heroConfiguration = GameContext.Instance.heroConfiguration;
            _playerModel = PlayerModel.Instance;
            _toastPresenter = ToastPresenter.Instance;
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
            var heroIndices = _playerModel.GetOwnedHeroes();
            foreach (var id in heroIndices)
            {
                var presenter = Instantiate(heroPresenterTemplate, heroPresenterTemplate.transform.parent);
                presenter.gameObject.SetActive(true);
                presenter.onClicked.AddListener(OnHeroClicked);
                
                var heroDef = _heroConfiguration.heroDefinitionCollection[id];
                var hero = new HeroModel(hc, heroDef, id, _playerModel.GetLevel(id));
                
                presenter.Initialize(hero, heroInfoPopupView, null);
                presenter.HeroView.SetSelected(_playerModel.SelectedHeroIdentifiers.Contains(id));
                
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
            if (_playerModel.SelectedHeroIdentifiers.Remove(presenter.HeroModel.Identifier))
            {
                presenter.HeroView.SetSelected(false);
                return;
            }

            if (_playerModel.SelectedHeroIdentifiers.Count == Constants.MaxSelectableHeroCount)
            {
                _toastPresenter.ToastMessage("You have reached equip limit!");
                return;
            }
            
            _playerModel.SelectedHeroIdentifiers.Add(presenter.HeroModel.Identifier);
            presenter.HeroView.SetSelected(true);
        }
        
        private void OnNotOwnedHeroViewClicked()
        {
            _toastPresenter.ToastMessage($"New hero unlock every {_heroConfiguration.unlockNewHeroEveryXLevel} battle.");
        }
        
        private void OnBattleClicked()
        {
            if (_playerModel.SelectedHeroIdentifiers.Count != Constants.MaxSelectableHeroCount)
            {
                _toastPresenter.ToastMessage($"You need to equip {Constants.MaxSelectableHeroCount} hero in order to battle!");
                return;
            }

            _playerModel.SaveSelectedHeroes(); //Not necessary though convenient for the player
            SceneManager.LoadScene(GameContext.Instance.sceneConfiguration.battleSceneIndex);
        }
    }
}