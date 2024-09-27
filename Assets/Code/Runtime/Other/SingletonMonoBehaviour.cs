using UnityEngine;

namespace HeroFighter.Runtime
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindAnyObjectByType<T>();
                }

                if (!_instance)
                {
                    _instance = new GameObject(nameof(T)).AddComponent<T>();
                }
                
                return _instance;
            }
        }

        private static T _instance;

        protected virtual void Awake()
        {
            if (_instance)
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}