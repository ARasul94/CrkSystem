using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Navigation.Tabs;

namespace Features.Weather
{
    public class WeatherTabPresenter: TabPresenterBase
    {
        public override TabType tabType => TabType.WEATHER;
        
        public WeatherTabPresenter(WeatherTabView _view) : base(_view)
        {
        }
        
        public override void Show()
        {
            base.Show();

            // Позже здесь запустим weather refresh loop.
        }

        public override void Hide()
        {
            base.Hide();

            // Позже здесь отменим weather-запросы.
        }
    }
}