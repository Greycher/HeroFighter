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
        [SerializeField] private DamageNumberPresenter damageNumberPresenter;
        [Tooltip("Let's say this value is -2 and the summary of the player's hero level is 7, " +
                 "enemy hero level will be minimum 5(7-2)")]
        [SerializeField] private int minEnemyHeroLevelDiff = -2;
        [Tooltip("Let's say this value is +2 and the summary of the player's hero level is 7, " +
                 "enemy hero level will be maximum 9(7+2)")]
        [SerializeField] private int maxEnemyHeroLevelDiff = +2;
        [SerializeField] private float delayBeforeTurnStarts = 0.7f;
        [SerializeField] private float delayBeforeEnemyAttack = 2f;

        private ToastPresenter _toastPresenter;
        private bool _playersTurn;
        private bool _battling;
        private bool _turnPlayed = true;

        public UnityEvent<bool> onBattleEnd = new();

        public List<HeroPresenter> AlivePlayerHeroes { get; } = new();

        private void Awake()
        {
            Assert.AreEqual(heroPresenters.Length, Constants.MaxSelectableHeroCount);
            _toastPresenter = ToastPresenter.Instance;

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
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeTurnStarts));
            _turnPlayed = false;
            if (!_playersTurn)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeEnemyAttack));
                var randomHero = AlivePlayerHeroes[Random.Range(0, AlivePlayerHeroes.Count)];
                enemyHeroPresenter.Attack(randomHero);
                EndTurn();
            }
        }
        
        private void EndTurn()
        {
            _turnPlayed = true;

            if (AlivePlayerHeroes.Count == 0)
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
                presenter.Present(hero, damageNumberPresenter);
                presenter.onClicked.AddListener(OnPlayerHeroClicked);
                presenter.onDeath.AddListener(OnHeroDied);


                var healthPresenter = Instantiate(healthPresenterPrefab, presenter.transform);
                healthPresenter.Present(hero.HealthModel);
                
                AlivePlayerHeroes.Add(presenter);
            }
        }

        private void OnPlayerHeroClicked(HeroPresenter presenter)
        {
            if (!AlivePlayerHeroes.Contains(presenter))
            {
                _toastPresenter.ToastMessage("Hero is dead!");
                return;
            }
            
            if (!_battling)
            {
                _toastPresenter.ToastMessage("Not battling at the moment!");
                return;
            }
            
            if (!_playersTurn || _turnPlayed)
            {
                _toastPresenter.ToastMessage("Wait for your turn!");
                return;
            }
            
            presenter.Attack(enemyHeroPresenter);
            EndTurn();
        }
        
        private void OnHeroDied(HeroPresenter presenter)
        {
            var removed = AlivePlayerHeroes.Remove(presenter);
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
            enemyHeroPresenter.Present(hero, damageNumberPresenter);
            
            var healthPresenter = Instantiate(healthPresenterPrefab, enemyHeroPresenter.transform);
            healthPresenter.Present(hero.HealthModel);
        }

        private void OnDestroy()
        {
            foreach (var presenter in heroPresenters)
            {
                presenter.onClicked.RemoveListener(OnPlayerHeroClicked);
                presenter.onDeath.RemoveListener(OnHeroDied);
            }
        }
    }
}