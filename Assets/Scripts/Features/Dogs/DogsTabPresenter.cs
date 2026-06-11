using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Navigation.Tabs;

namespace Features.Dogs
{
    public class DogsTabPresenter: TabPresenterBase
    {
        public override TabType tabType => TabType.DOGS;
        
        public DogsTabPresenter(DogsTabView _view) : base(_view)
        {
        }
        
        public override void Show()
        {
            base.Show();

            // Позже здесь загрузим список пород.
        }

        public override void Hide()
        {
            base.Hide();

            // Позже здесь отменим dog-запросы и скроем loaders.
        }
    }
}