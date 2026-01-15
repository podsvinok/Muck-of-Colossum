using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI.LoadingCurtain
{
    public class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        [SerializeField] private CanvasGroup curtain;
        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text text;
        
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

        public void SetProgressBar(float progress) => 
            progressBar.fillAmount = progress;

        public void SetLoadingStatus(string newText) => 
            text.text = newText;

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