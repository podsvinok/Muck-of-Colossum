using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Windows
{
    public class WindowsBase: MonoBehaviour
    {
        //TODO
        [SerializeField] protected Button CloseButton;
        
        private void Awake() => 
            OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() => 
            Cleanup();

        protected virtual void OnAwake() => 
            CloseButton?.onClick.AddListener(()=> Destroy(gameObject));

        protected virtual void Initialize(){}
        protected virtual void SubscribeUpdates(){}
        protected virtual void Cleanup(){}
    }
}