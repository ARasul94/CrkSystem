namespace Features.Weather.Models
{
    public struct WeatherViewData
    {
        public readonly int m_temperature;
        public readonly string m_temperatureUnit;
        public readonly string m_shortForecast;
        public readonly string m_iconUrl;

        public WeatherViewData(
            int _temperature,
            string _temperatureUnit,
            string _shortForecast,
            string _iconUrl)
        {
            m_temperature = _temperature;
            m_temperatureUnit = _temperatureUnit;
            m_shortForecast = _shortForecast;
            m_iconUrl = _iconUrl;
        }

        public string GetFormattedText()
        {
            return $"Сегодня - {m_temperature}{m_temperatureUnit}";
        }
    }
}