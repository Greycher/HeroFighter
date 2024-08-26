using System;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace HeroFighter.Runtime.Presenters
{
    public class DamageNumberPresenter : MonoBehaviour
    {
        [SerializeField] private DamageNumberEffectView damageNumberEffectPrefab;
        
        private ObjectPool<DamageNumberEffectView> _damageNumberPool;

        private void Awake()
        {
            _damageNumberPool = new(CreateFunc, ActionOnGet, ActionOnRelease);
        }

        public void Spawn(Vector2 screenPos, int number)
        {
            var view = _damageNumberPool.Get();
            view.RectTransform.anchoredPosition = screenPos;
            view.Play(number);
        }

        private DamageNumberEffectView CreateFunc()
        {
            return Instantiate(damageNumberEffectPrefab, transform);
        }
        
        private void ActionOnGet(DamageNumberEffectView effectView)
        {
            effectView.gameObject.SetActive(true);
            effectView.onFloatingAnimEnd.AddListener(OnAnimEnd);
        }

        private void OnAnimEnd(DamageNumberEffectView effectView)
        {
            _damageNumberPool.Release(effectView);
        }

        private void ActionOnRelease(DamageNumberEffectView effectView)
        {
            effectView.gameObject.SetActive(false);
            effectView.onFloatingAnimEnd.RemoveListener(OnAnimEnd);
        }
    }
}