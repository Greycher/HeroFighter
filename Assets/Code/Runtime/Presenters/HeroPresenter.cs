using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroPresenter : MonoBehaviour
    {
        [SerializeField] private HeroView heroView;
        [SerializeField] private bool logHealth;
        
        private HeroModel _heroModel;
        private HeroConfiguration _heroConfiguration;
        private CancellationTokenSource _holdInputCancelTokenSource;
        private DamageNumberPresenter _damageNumberPresenter;

        public UnityEvent<HeroPresenter> onClicked = new();
        public UnityEvent<HeroPresenter> onDeath = new();
        private bool _onHoldInputDetected;
        
        public HeroModel HeroModel => _heroModel;
        public HeroView HeroView => heroView;

        private void Awake()
        {
            _heroConfiguration = GameContext.Instance.heroConfiguration;
        }

        public void Present(HeroModel heroModel, DamageNumberPresenter damageNumberPresenter)
        {
            _heroModel = heroModel;
            _damageNumberPresenter = damageNumberPresenter;
            heroModel.HealthModel.LogHealth = logHealth;
            heroView.UpdateView(heroModel.HeroDefinition.name);
            _heroModel?.onDied.RemoveListener(OnDeath);
            _heroModel?.onDied.AddListener(OnDeath);
        }

        private void OnEnable()
        {
            heroView.onPointerDown.AddListener(OnPointerDown);
            if (_heroConfiguration.cancelHoldInputOnDragOutside)
            {
                heroView.onPointerDrag.AddListener(OnPointerDrag);
            }
            heroView.onPointerUp.AddListener(OnPointerUp);
            _heroModel?.onDied.RemoveListener(OnDeath);
            _heroModel?.onDied.AddListener(OnDeath);
        }

        private void OnDisable()
        {
            _holdInputCancelTokenSource?.Cancel();
            heroView.onPointerDown.RemoveListener(OnPointerDown);
            heroView.onPointerDrag.RemoveListener(OnPointerDrag);
            heroView.onPointerUp.RemoveListener(OnPointerUp);
            _heroModel?.onDied.RemoveListener(OnDeath);
        }
        
        private void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Middle)
            {
                return;
            }
            
            _onHoldInputDetected = false;
            _holdInputCancelTokenSource = new CancellationTokenSource();
            ExecuteHoldInputAfterDelayAsync(_heroConfiguration.heroInfoInputHoldDuration, _holdInputCancelTokenSource.Token);
        }
        
        private void OnPointerDrag(PointerEventData eventData, bool inside)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Middle)
            {
                return;
            }
            
            if (!inside && _heroConfiguration.cancelHoldInputOnDragOutside)
            {
                _holdInputCancelTokenSource?.Cancel();
            }
        }

        private void OnPointerUp(PointerEventData eventData, bool insideRect)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Middle)
            {
                return;
            }
            
            _holdInputCancelTokenSource.Cancel();
            if (insideRect && !_onHoldInputDetected)
            {
                OnClick();
            }
        }
        
        private async void ExecuteHoldInputAfterDelayAsync(float delay, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), false, 
                PlayerLoopTiming.Update, token).SuppressCancellationThrow();
            
            if (token.IsCancellationRequested)
            {
                return;
            }
            
            OnHold();
        }
        
        private void OnHold()
        {
            _onHoldInputDetected = true;
            SpawnInfoPopup();
        }

        private void SpawnInfoPopup()
        {
            var view = Instantiate(_heroConfiguration.heroInfoPopupViewPrefab);
            view.OnCloseBtnClicked.AddListener(OnInfoPopupCloseClicked);
            var def = HeroModel.HeroDefinition;
            var xpLimit = _heroConfiguration.experiencePerLevel;
            var xp = _heroConfiguration.GetExperience(HeroModel.Identifier) % xpLimit;
            view.UpdateView(HeroView.ScreenPos, def.name, HeroModel.Level + 1, HeroModel.AttackPower, xp, xpLimit);
        }

        private void OnInfoPopupCloseClicked(HeroInfoPopupView view)
        {
            view.OnCloseBtnClicked.RemoveListener(OnInfoPopupCloseClicked);
            Destroy(view.gameObject);
        }

        private void OnClick()
        {
            onClicked.Invoke(this);
        }

        private void OnDeath()
        {
            heroView.OnDeath();
            onDeath.Invoke(this);
        }

        public void Attack(HeroPresenter randomHero)
        {
            var damage = HeroModel.Damage(randomHero.HeroModel);
            HeroView.OnAttack();
            if (_damageNumberPresenter)
            {
                _damageNumberPresenter.Spawn(randomHero.HeroView.DamageNumberSpawnPoint, damage);
            }
        }
    }
}