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
        [SerializeField] private LevelEndMenuView levelSuccessMenuView;
        [SerializeField] private LevelEndMenuView levelFailMenuView;
        [SerializeField] private float levelEndMenuOpenDelay = 1f;

        private readonly LevelModel _levelModel = new();

        private void Start()
        {
            battlePresenter.onBattleEnd.AddListener(OnBattleEnd);
            levelSuccessMenuView.OnContinuePressed.AddListener(OnContinuePressed);
            levelFailMenuView.OnContinuePressed.AddListener(OnContinuePressed);

            battlePresenter.StartBattle();
        }

        private void OnContinuePressed()
        {
            SceneManager.LoadScene(GameContext.Instance.heroSelectionSceneIndex);
        }

        private async void OnBattleEnd(bool playerWin)
        {
            var playCount = _levelModel.GetPlayCount() + 1;
            _levelModel.SetPlayCount(playCount);

            var hc = GameContext.Instance.heroConfiguration;
            if (playCount % hc.getNewHeroEveryXLevel == 0)
            {
                var unOwnedHeroes = hc.GetUnOwnedHeroIdentifiers();
                hc.SetHeroOwned(unOwnedHeroes[Random.Range(0, unOwnedHeroes.Count)]);
            }
            
            if (playerWin)
            {
                foreach (var hero in battlePresenter.AlivePlayerHeroes)
                {
                    var id = hero.HeroModel.Identifier;
                    hc.SetExperience(id, hc.GetExperience(id) + hc.experienceIncreasePerWin);
                }
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(levelEndMenuOpenDelay));
            
            if (playerWin)
            {
                levelSuccessMenuView.gameObject.SetActive(true);
            }
            else
            {
                levelFailMenuView.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            battlePresenter.onBattleEnd.RemoveListener(OnBattleEnd);
            levelSuccessMenuView.OnContinuePressed.RemoveListener(OnContinuePressed);
            levelFailMenuView.OnContinuePressed.RemoveListener(OnContinuePressed);
        }
    }
}