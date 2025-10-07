using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        [SerializeField] private CanvasGroup curtain;
        
        private const float FadeInWaitingSeconds = 0.03f;

        private WaitForSeconds fadeInWaitForSeconds;
        
        public void Show()
        {
            gameObject.SetActive(this);
            curtain.alpha = 1;
        }

        public void Hide()
        {
            fadeInWaitForSeconds = new WaitForSeconds(FadeInWaitingSeconds);
            StartCoroutine(DoFadeIn());
        }

        private IEnumerator DoFadeIn()
        {
            while (curtain.alpha > 0)
                curtain.alpha -= FadeInWaitingSeconds;
            yield return fadeInWaitForSeconds;
            
            gameObject.SetActive(false);
        }
        
        public class Factory : PlaceholderFactory<string, UniTask<LoadingCurtain>>
        {
        }
    }
}