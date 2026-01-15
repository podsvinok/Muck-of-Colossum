namespace Code.UI.LoadingCurtain
{
    public interface ILoadingCurtain
    {
        public void Show();
        public void Hide();
        public void SetProgressBar(float progress);
        public void SetLoadingStatus(string newText);
    }
}