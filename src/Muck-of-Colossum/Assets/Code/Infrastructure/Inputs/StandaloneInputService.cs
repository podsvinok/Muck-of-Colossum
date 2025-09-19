using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Infrastructure.Inputs
{
    public class StandaloneInputService : IInputService
    {
        private Camera mainCamera;
        private Vector3 screenPosition;

        private Camera CameraMain
        {
            get
            {
                if(mainCamera == null && Camera.main != null)
                  mainCamera = Camera.main;
                
                return mainCamera;
            }
        }

        public Vector2 GetScreenMousePosition() => 
            CameraMain ? (Vector2) Input.mousePosition : new Vector2();

        public Vector2 GetWorldMousePosition()
        {
            if(CameraMain == null)
              return Vector2.zero;
            
            screenPosition.x = Input.mousePosition.x;
            screenPosition.y = Input.mousePosition.y;
            return CameraMain.ScreenToWorldPoint(screenPosition);
        }

        public bool HasAxisInput() => GetHorizontalAxis() != 0 || GetVerticalAxis() != 0;
        
        public float GetVerticalAxis() => Input.GetAxis("Vertical");
        public float GetHorizontalAxis() => Input.GetAxis("Horizontal");
        

        public bool GetLeftMouseButton() => 
          Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject();

        public bool GetLeftMouseButtonDown() =>
          Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
        
        public bool GetLeftMouseButtonUp() => 
          Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject();
    }
}