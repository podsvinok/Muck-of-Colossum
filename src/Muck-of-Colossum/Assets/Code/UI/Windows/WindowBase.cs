using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Windows
{
    public class WindowBase: MonoBehaviour
    {
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

        protected virtual void OnAwake()
        {
            if (!CloseButton)
                return;
            
            CloseButton.onClick.AddListener(()=> Destroy(gameObject));
        }

        public virtual void Show() => 
            gameObject.SetActive(true);

        public virtual void Hide() => 
            gameObject.SetActive(false);
        protected virtual void Initialize(){}
        protected virtual void SubscribeUpdates(){}
        protected virtual void Cleanup(){}
    }
}