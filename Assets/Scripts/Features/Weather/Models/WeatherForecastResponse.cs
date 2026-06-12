using System;

namespace Features.Weather.Models
{
    [Serializable]
    public class WeatherForecastResponse
    {
        public WeatherProperties properties;
    }
    
    [Serializable]
    public class WeatherProperties
    {
        public WeatherPeriod[] periods;
    }
    
    [Serializable]
    public sealed class WeatherPeriod
    {
        public int number;
        public string name;
        public int temperature;
        public string temperatureUnit;
        public string icon;
        public string shortForecast;
    }
}