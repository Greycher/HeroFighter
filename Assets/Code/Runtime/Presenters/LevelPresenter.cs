using System;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace HeroFighter.Runtime.Presenters
{
    public class LevelPresenter : MonoBehaviour
    {
        [SerializeField] private BattlePresenter battlePresenter;
        [SerializeField] private LevelEndMenuView levelEndMenuView;
        [SerializeField] private float levelEndMenuOpenDelay = 1f;

        private readonly LevelModel _levelModel = new();

        private void Start()
        {
            battlePresenter.onBattleEnd.AddListener(OnBattleEnd);
            levelEndMenuView.OnContinuePressed.AddListener(OnContinuePressed);

            battlePresenter.StartBattle();
        }
        
        private void OnDestroy()
        {
            battlePresenter.onBattleEnd.RemoveListener(OnBattleEnd);
            levelEndMenuView.OnContinuePressed.RemoveListener(OnContinuePressed);
        }

        private void OnContinuePressed()
        {
            SceneManager.LoadScene(GameContext.Instance.sceneConfiguration.heroSelectionSceneIndex);
        }

        private async void OnBattleEnd(bool playerWin)
        {
            var playCount = _levelModel.GetPlayCount() + 1;
            _levelModel.SetPlayCount(playCount);

            var playerModel = PlayerModel.Instance;
            var hc = GameContext.Instance.heroConfiguration;
            if (playCount % hc.unlockNewHeroEveryXLevel == 0 
                && TryUnlockNewHero(playerModel, hc, out var heroName))
            {
                levelEndMenuView.EnableNewHeroView(heroName);
            }
            
            if (playerWin)
            {
                foreach (var hero in battlePresenter.AlivePlayerHeroes)
                {
                    AddExperienceToHeroAndUpdateLevelEndUI(hero, playerModel, hc);
                }
            }

            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(levelEndMenuOpenDelay), 
                cancellationToken: token).SuppressCancellationThrow();

            if (token.IsCancellationRequested)
            {
                return;
            }
            
            levelEndMenuView.gameObject.SetActive(true);
            levelEndMenuView.UpdateView(playerWin);
        }
        
        private bool TryUnlockNewHero(PlayerModel playerModel, HeroConfiguration hc, out string heroName)
        {
            var unOwnedHeroes = playerModel.GetUnownedHeroes();
            if (unOwnedHeroes.Count > 0)
            {
                var rndHeroId = unOwnedHeroes[Random.Range(0, unOwnedHeroes.Count)];
                playerModel.SetHeroOwned(rndHeroId);
                heroName = hc.heroDefinitionCollection[rndHeroId].name;
                return true;
            }

            heroName = default;
            return false;
        }

        private void AddExperienceToHeroAndUpdateLevelEndUI(HeroPresenter hero, PlayerModel playerModel, HeroConfiguration hc)
        {
            var beforeHeroModel = hero.HeroModel;
            var id = beforeHeroModel.Identifier;
            var beforeXP = playerModel.GetExperience(id);
            var afterXP = beforeXP + hc.experienceIncreasePerWin;
            playerModel.SetExperience(id, afterXP);

            var def = hc.heroDefinitionCollection[id];
            var afterHeroModel = new HeroModel(hc, def, id, playerModel.GetLevel(id));

            var view = levelEndMenuView.GetHeroStatUpView(def.name);
            if (beforeHeroModel.Level != afterHeroModel.Level)
            {
                view.AddLevelIncrease(beforeHeroModel.Level + 1, afterHeroModel.Level + 1);
                view.AddHealthIncrease(beforeHeroModel.HealthModel.InitialHealth, afterHeroModel.HealthModel.InitialHealth);
                view.AddAttackPowerIncrease(beforeHeroModel.AttackPower, afterHeroModel.AttackPower);
            }
            else
            {
                view.AddExperienceIncrease(beforeXP % hc.experiencePerLevel, afterXP % hc.experiencePerLevel);
            }
        }
    }
}