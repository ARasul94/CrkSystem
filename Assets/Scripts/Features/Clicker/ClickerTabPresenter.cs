using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Navigation.Tabs;

namespace Features.Clicker
{
    public class ClickerTabPresenter: TabPresenterBase
    {
        public override TabType tabType => TabType.CLICKER;
        
        public ClickerTabPresenter(ClickerTabView _view) : base(_view)
        {
        }
    }
}