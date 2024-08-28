using System.Collections.Generic;
using HeroFighter.Runtime.Views;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace HeroFighter.Runtime.Presenters
{
    [RequireComponent(typeof(Canvas))]
    public class ToastPresenter : SingletonMonoBehaviour<ToastPresenter>
    {
        [SerializeField] private ToastView toastViewTemplate;
        
        private ObjectPool<ToastView> _toastPool;
        private readonly HashSet<int> _messageHashSet = new();

        protected override void Awake()
        {
            base.Awake();
            _toastPool = new ObjectPool<ToastView>(CreateFunc, ActionOnGet, ActionOnRelease);
            toastViewTemplate.gameObject.SetActive(false);
        }

        public void ToastMessage(string message)
        {
            var msgHash = message.GetHashCode();
            if (_messageHashSet.Contains(msgHash))
            {
                return;
            }

            var view = _toastPool.Get();
            view.ToastMessage(message);
            _messageHashSet.Add(msgHash);
        }
        
        private ToastView CreateFunc()
        {
            return Instantiate(toastViewTemplate, toastViewTemplate.transform.parent);
        }
        
        private void ActionOnGet(ToastView view)
        {
            view.gameObject.SetActive(true);
            view.onAnimationEnd.AddListener(OnAnimEnd);
        }

        private void ActionOnRelease(ToastView view)
        {
            view.gameObject.SetActive(false);
            view.onAnimationEnd.RemoveListener(OnAnimEnd);
        }
        
        private void OnAnimEnd(ToastView effectView)
        {
            var removed = _messageHashSet.Remove(effectView.Message.GetHashCode());
            Assert.IsTrue(removed);
            _toastPool.Release(effectView);
        }
    }
}