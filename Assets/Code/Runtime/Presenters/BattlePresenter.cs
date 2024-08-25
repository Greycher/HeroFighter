using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace HeroFighter.Runtime.Presenters
{
    public class BattlePresenter : MonoBehaviour
    {
        [SerializeField] private HeroPresenter[] heroPresenters;
        [FormerlySerializedAs("enemyHeroView")] [SerializeField] private HeroPresenter enemyHeroPresenter;
        [SerializeField] private HealthPresenter healthPresenterPrefab;
        [SerializeField] private TurnIndicatorView turnIndicatorView;
        [Tooltip("Let's say this value is -2 and the summary of the player's hero level is 7, " +
                 "enemy hero level will be minimum 5(7-2)")]
        [SerializeField] private int minEnemyHeroLevelDiff = -2;
        [Tooltip("Let's say this value is +2 and the summary of the player's hero level is 7, " +
                 "enemy hero level will be maximum 9(7+2)")]
        [SerializeField] private int maxEnemyHeroLevelDiff = +2;
        [SerializeField] private float delayBeforeTurnStarts = 0.7f;
        [SerializeField] private float delayBeforeEnemyAttack = 2f;

        private readonly List<HeroModel> _alivePlayerHeroes = new();
        private bool _playersTurn;
        private bool _battling;
        private bool _turnPlayed = true;

        public UnityEvent<bool> onBattleEnd = new();

        private void Awake()
        {
            Assert.AreEqual(heroPresenters.Length, Constants.MaxSelectableHeroCount);
            var hc = GameContext.Instance.heroConfiguration;
            PreparePlayerHeroes(hc);
            var totalLvl = CalculateTotalPlayerHeroLevel();
            var enemyHeroLvl = Random.Range(totalLvl + minEnemyHeroLevelDiff, totalLvl + maxEnemyHeroLevelDiff);
            PrepareEnemyHero(hc, enemyHeroLvl);
        }

        public void StartBattle()
        {
            _playersTurn = Random.value > 0.5f;
            _battling = true;
            StartTurnAsync();
        }
        
        private async void StartTurnAsync()
        {
            turnIndicatorView.OnTurnStarted(_playersTurn);
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeTurnStarts), DelayType.Realtime, 
                PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            _turnPlayed = false;
            if (!_playersTurn)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeEnemyAttack), DelayType.Realtime, 
                    PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                var randomHero = _alivePlayerHeroes[Random.Range(0, _alivePlayerHeroes.Count)];
                enemyHeroPresenter.Attack(randomHero);
                EndTurn();
            }
        }
        
        private void EndTurn()
        {
            _turnPlayed = true;

            if (_alivePlayerHeroes.Count == 0)
            {
                EndBattle(false);
            }
            else if (enemyHeroPresenter.HeroModel.Died)
            {
                EndBattle(true);
            }
            else
            {
                _playersTurn = !_playersTurn;
                StartTurnAsync();
            }
        }
        
        private void EndBattle(bool playerWin)
        {
            _battling = false;
            onBattleEnd.Invoke(playerWin);
            Debug.Log($"Battle over, player win: {playerWin}");
        }

        private void PreparePlayerHeroes(HeroConfiguration hc)
        {
            for (int i = 0; i < Constants.MaxSelectableHeroCount; i++)
            {
                var id = hc.selectedHeroIdentifiers[i];
                var heroDef = hc.heroDefinitionCollection[id];
                var hero = new HeroModel(hc, heroDef, id, hc.GetLevel(id));
                var presenter = heroPresenters[i];
                presenter.Present(hero);
                presenter.onClicked.AddListener(OnPlayerHeroClicked);
                presenter.onDied.AddListener(OnHeroDied);


                var healthPresenter = Instantiate(healthPresenterPrefab, presenter.transform);
                healthPresenter.Present(hero.healthModel);
                
                _alivePlayerHeroes.Add(hero);
            }
        }

        private void OnPlayerHeroClicked(HeroPresenter presenter)
        {
            if (!_battling)
            {
                //TODO toast here
                return;
            }
            
            if (_turnPlayed)
            {
                //TODO toast here
                return;
            }

            if (!_playersTurn)
            {
                //TODO toast here
                return;
            }

            presenter.Attack(enemyHeroPresenter.HeroModel);
            EndTurn();
        }
        
        private void OnHeroDied(HeroPresenter presenter)
        {
            UnSubscribeHeroPresenterEvents(presenter);
            presenter.HeroView.Interactable = false;
            var removed = _alivePlayerHeroes.Remove(presenter.HeroModel);
            Assert.IsTrue(removed);
        }

        private int CalculateTotalPlayerHeroLevel()
        {
            var totalLvl = 0;
            foreach (var hero in heroPresenters)
            {
                totalLvl += hero.HeroModel.Level;
            }

            return totalLvl;
        }

        private void PrepareEnemyHero(HeroConfiguration hc, int level)
        {
            var ids = hc.heroDefinitionCollection.Keys;
            var id = ids.ElementAt(Random.Range(0, ids.Count));
            var def = hc.heroDefinitionCollection[id];
            var hero = new HeroModel(hc, def, id, level);
            enemyHeroPresenter.Present(hero);
            
            var healthPresenter = Instantiate(healthPresenterPrefab, enemyHeroPresenter.transform);
            healthPresenter.Present(hero.healthModel);
        }

        private void OnDestroy()
        {
            foreach (var presenter in heroPresenters)
            {
                UnSubscribeHeroPresenterEvents(presenter);
            }
        }

        private void UnSubscribeHeroPresenterEvents(HeroPresenter presenter)
        {
            presenter.onClicked.RemoveListener(OnPlayerHeroClicked);
            presenter.onDied.RemoveListener(OnHeroDied);
        }
    }
}