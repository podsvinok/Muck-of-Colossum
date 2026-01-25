using System;
using Code.Gameplay.Levels;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.ResourceSystem
{
    public class ResourceHud : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image hpImage;
        private ILevelDataProvider levelData;
        private Transform camera;
        
        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }

        public void EnableCanvas()
        {
            canvas.gameObject.SetActive(true);
            camera = levelData.LocalPlayer.GetComponentInChildren<Camera>().transform;
        }
        
        public void SetHp(float hp)
        {
            hpImage.fillAmount = hp;
        }
        
        private void Update()
        {
            if (!canvas.enabled) return;
            if (levelData.LocalPlayer)
                canvas.transform.LookAt(camera);
        }
    }
}