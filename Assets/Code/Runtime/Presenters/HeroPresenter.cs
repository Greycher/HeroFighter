using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroPresenter : MonoBehaviour
    {
        [SerializeField] private HeroView heroView;
        [SerializeField] private bool logHealth;
        
        private HeroConfiguration _heroConfiguration;
        private HeroModel _heroModel;
        private HeroInfoPopupView _heroInfoPopupView;
        private DamageNumberPresenter _damageNumberPresenter;
        private CancellationTokenSource _holdInputCancelTokenSource;

        public UnityEvent<HeroPresenter> onClicked = new();
        public UnityEvent<HeroPresenter> onDeath = new();
        private bool _onHoldInputDetected;

        public HeroModel HeroModel => _heroModel;
        public HeroView HeroView => heroView;

        private void Awake()
        {
            _heroConfiguration = GameContext.Instance.heroConfiguration;
        }

        public void Initialize(HeroModel heroModel, HeroInfoPopupView heroInfoPopupView, [CanBeNull]DamageNumberPresenter damageNumberPresenter)
        {
            _heroModel = heroModel;
            _heroInfoPopupView = heroInfoPopupView;
            _damageNumberPresenter = damageNumberPresenter;
            heroModel.HealthModel.LogHealth = logHealth;
            heroView.UpdateView(heroModel.HeroDefinition.name);
            _heroModel.onDied.RemoveListener(OnDeath);
            _heroModel.onDied.AddListener(OnDeath);
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

        private void OnDestroy()
        {
            _heroInfoPopupView.onCloseBtnClicked.RemoveListener(OnInfoPopupCloseClicked);
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
            await UniTask.Delay(TimeSpan.FromSeconds(delay), 
                cancellationToken: token).SuppressCancellationThrow();
            
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
            _heroInfoPopupView.onCloseBtnClicked.AddListener(OnInfoPopupCloseClicked);
            var def = HeroModel.HeroDefinition;
            var xpLimit = _heroConfiguration.experiencePerLevel;
            var xp = PlayerModel.Instance.GetExperience(HeroModel.Identifier) % xpLimit;
            _heroInfoPopupView.UpdateView(HeroView.ScreenPos, def.name, HeroModel.Level + 1, HeroModel.AttackPower, xp, xpLimit);
            _heroInfoPopupView.gameObject.SetActive(true);
        }

        private void OnInfoPopupCloseClicked(HeroInfoPopupView view)
        {
            view.onCloseBtnClicked.RemoveListener(OnInfoPopupCloseClicked);
            view.gameObject.SetActive(false);
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

        public void Attack(HeroPresenter heroPresenter)
        {
            var damage = HeroModel.Damage(heroPresenter.HeroModel);
            HeroView.OnAttack();
            if (_damageNumberPresenter)
            {
                _damageNumberPresenter.Spawn(heroPresenter.HeroView.DamageNumberSpawnPoint, damage);
            }
        }
    }
}