using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Windows
{
    public class WindowBase: MonoBehaviour
    {
        [HideInInspector] public bool isOpened;
        
        private void Awake() => 
            OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() => 
            Cleanup();

        public virtual void Show()
        {
            gameObject.SetActive(true);
            isOpened = true;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isOpened = false;
        }

        protected virtual void OnAwake(){}
        protected virtual void Initialize(){}
        protected virtual void SubscribeUpdates(){}
        protected virtual void Cleanup(){}
    }
}