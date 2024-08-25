using UnityEngine;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Slider delayedSlider;
        [SerializeField] private float delay = 0.5f;

        public void UpdateView(float percentage)
        {
            slider.value = percentage;
            delayedSlider.value = percentage;
        }
    }
}