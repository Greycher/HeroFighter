using System;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFighter.Runtime.Presenters
{
    public class LevelPresenter : MonoBehaviour
    {
        [SerializeField] private BattlePresenter battlePresenter;
        [SerializeField] private LevelEndMenuView levelSuccessMenuView;
        [SerializeField] private LevelEndMenuView levelFailMenuView;
        [SerializeField] private float levelEndMenuOpenDelay = 1f;

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
            await UniTask.Delay(TimeSpan.FromSeconds(levelEndMenuOpenDelay));
            if (playerWin)
            {
                var hc = GameContext.Instance.heroConfiguration;
                foreach (var hero in battlePresenter.AlivePlayerHeroes)
                {
                    var id = hero.Identifier;
                    hc.SetExperience(id, hc.GetExperience(id) + hc.experienceIncreasePerWin);
                }
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