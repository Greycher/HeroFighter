using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class LevelEndMenuView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public UnityEvent OnContinuePressed => button.onClick;
    }
}