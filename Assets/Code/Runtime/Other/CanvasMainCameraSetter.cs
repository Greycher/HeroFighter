using System;
using UnityEngine;

namespace HeroFighter.Runtime.Other
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