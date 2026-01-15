using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class HoverableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image hoverElement;

        private void OnEnable() => 
            OnPointerExit(null);

        public void OnPointerEnter(PointerEventData eventData) => 
            hoverElement.color = Color.white;
        
        public void OnPointerExit(PointerEventData eventData) => 
            hoverElement.color = Color.clear;
    }
}