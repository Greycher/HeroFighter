using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Pool;

namespace HeroFighter.Runtime.Presenters
{
    public class DamageNumberPresenter : MonoBehaviour
    {
       [SerializeField] private DamageNumberEffectView damageNumberEffectTemplate;
        
        private ObjectPool<DamageNumberEffectView> _damageNumberPool;

        private void Awake()
        {
            damageNumberEffectTemplate.gameObject.SetActive(false);
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
            return Instantiate(damageNumberEffectTemplate, transform);
        }
        
        private void ActionOnGet(DamageNumberEffectView effectView)
        {
            effectView.gameObject.SetActive(true);
            effectView.onAnimEnd.AddListener(OnAnimEnd);
        }

        private void OnAnimEnd(DamageNumberEffectView effectView)
        {
            _damageNumberPool.Release(effectView);
        }

        private void ActionOnRelease(DamageNumberEffectView effectView)
        {
            effectView.gameObject.SetActive(false);
            effectView.onAnimEnd.RemoveListener(OnAnimEnd);
        }
    }
}