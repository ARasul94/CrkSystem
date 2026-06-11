namespace Features.Navigation.Presenters
{
    public interface ITabPresenter
    {
        TabType tabType { get; }
        void Show();
        void Hide();
    }
}