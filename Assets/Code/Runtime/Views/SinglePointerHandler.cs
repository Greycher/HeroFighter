using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public abstract class SinglePointerHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const int c_nonPointer = int.MinValue;
        private int _pointerId = c_nonPointer;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerId != c_nonPointer) 
            {
                return;
            }

            _pointerId = eventData.pointerId;
            OnSinglePointerDown(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) 
            {
                return;
            }
            
            OnSinglePointerDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) 
            {
                return;
            }
            
            _pointerId = c_nonPointer;
            OnSinglePointerUp(eventData);
        }

        protected abstract void OnSinglePointerDown(PointerEventData eventData);
        protected abstract void OnSinglePointerDrag(PointerEventData eventData);
        protected abstract void OnSinglePointerUp(PointerEventData eventData);
    }
}