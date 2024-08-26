using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HeroFighter.Runtime.Presenters
{
    public class HeroPresenter : MonoBehaviour
    {
        [SerializeField] private HeroView heroView;
        [SerializeField] private bool logHealth;
        
        private HeroModel _heroModel;
        private HeroConfiguration _heroConfiguration;
        private CancellationTokenSource _holdInputCancelTokenSource;

        public UnityEvent<HeroPresenter> onClicked = new();
        public UnityEvent<HeroPresenter> onDied = new();
        private bool _onHoldInputDetected;
        public HeroModel HeroModel => _heroModel;
        public HeroView HeroView => heroView;

        private void Awake()
        {
            _heroConfiguration = GameContext.Instance.heroConfiguration;
        }

        public void Present(HeroModel heroModel)
        {
            _heroModel = heroModel;
            heroModel.healthModel.LogHealth = logHealth;
            heroView.UpdateView(heroModel.heroDefinition.name);
            _heroModel?.onDied.RemoveListener(OnDied);
            _heroModel?.onDied.AddListener(OnDied);
        }

        private void OnEnable()
        {
            heroView.onPointerDown.AddListener(OnPointerDown);
            if (_heroConfiguration.cancelHoldInputOnDragOutside)
            {
                heroView.onPointerDrag.AddListener(OnPointerDrag);
            }
            heroView.onPointerUp.AddListener(OnPointerUp);
            _heroModel?.onDied.RemoveListener(OnDied);
            _heroModel?.onDied.AddListener(OnDied);
        }

        private void OnDisable()
        {
            _holdInputCancelTokenSource?.Cancel();
            heroView.onPointerDown.RemoveListener(OnPointerDown);
            heroView.onPointerDrag.RemoveListener(OnPointerDrag);
            heroView.onPointerUp.RemoveListener(OnPointerUp);
            _heroModel?.onDied.RemoveListener(OnDied);
        }
        
        private void OnPointerDown(PointerEventData eventData)
        {
            _onHoldInputDetected = false;
            _holdInputCancelTokenSource = new CancellationTokenSource();
            ExecuteHoldInputAfterDelayAsync(_heroConfiguration.heroInfoInputHoldDuration, _holdInputCancelTokenSource.Token);
        }
        
        private void OnPointerDrag(PointerEventData arg0, bool arg1)
        {
            if (_heroConfiguration.cancelHoldInputOnDragOutside)
            {
                _holdInputCancelTokenSource.Cancel();
            }
        }

        private void OnPointerUp(PointerEventData eventData, bool insideRect)
        {
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
            var pos = HeroView.transform.position;
            var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            var def = HeroModel.heroDefinition;
            var xpLimit = _heroConfiguration.experiencePerLevel;
            var xp = _heroConfiguration.GetExperience(HeroModel.Identifier) % xpLimit;
            view.UpdateView(screenPos, def.name, HeroModel.Level, def.attackPower, xp, xpLimit);
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

        private void OnDied()
        {
            onDied.Invoke(this);
        }

        public void Attack(HeroModel randomHero)
        {
            HeroModel.Damage(randomHero);
            HeroView.OnAttack();
        }
    }
}