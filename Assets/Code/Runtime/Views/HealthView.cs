using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Slider delayedSlider;
        [SerializeField] private float delay = 0.5f;
        [Tooltip("Sometimes health is near zero and it seems like hero is alive with zero health. This parameter prevents that.")]
        [SerializeField] private float minValueUnlessZero = 0.1f;
        [SerializeField] private float tick = 0.1f;
        [SerializeField] private float smoothness = 1f;

        private CancellationTokenSource _onDisableCancelTokenSource;
        private float _lastUpdateTime;
        private float _targetValue;
        private float _delayedTargetValue;

        private void OnEnable()
        {
            _onDisableCancelTokenSource = new CancellationTokenSource();
            UpdateHealth(_onDisableCancelTokenSource.Token);
        }

        private void OnDisable()
        {
            _onDisableCancelTokenSource.Cancel();
        }

        public void UpdateView(float percentage, bool immediate = false)
        {
            var min = Mathf.Approximately(percentage, 0) ? 0 : minValueUnlessZero;
            _targetValue = Mathf.Clamp(percentage, min, 1);
            _lastUpdateTime = Time.time;
            if (immediate)
            {
                _delayedTargetValue = slider.value = delayedSlider.value = _targetValue;
            }
        }

        private async void UpdateHealth(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(tick), cancellationToken: token).SuppressCancellationThrow();
                
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (Time.time - _lastUpdateTime > delay)
                {
                    _delayedTargetValue = _targetValue;
                }
                
                var maxDelta = smoothness * tick;
                slider.value = Mathf.MoveTowards(slider.value, _targetValue, maxDelta);
                delayedSlider.value = Mathf.MoveTowards(delayedSlider.value, _delayedTargetValue, maxDelta);
            }
        }
    }
}