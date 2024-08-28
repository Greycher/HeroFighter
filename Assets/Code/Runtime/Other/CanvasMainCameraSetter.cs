using UnityEngine;

namespace HeroFighter.Runtime
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasMainCameraSetter : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }
}