using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Configs/API Config")]
    public class ApiConfig : ScriptableObject
    {
        [Header("Weather")]
        [SerializeField] private string m_weatherForecastUrl =
            "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        [Header("Dogs")]
        [SerializeField] private string m_dogBreedsUrl =
            "https://dogapi.dog/api/v2/breeds";

        [SerializeField] private string m_dogBreedDetailsUrlFormat =
            "https://dogapi.dog/api/v2/breeds/{0}";
        
        public string weatherForecastUrl => m_weatherForecastUrl;
        public string dogBreedsUrl => m_dogBreedsUrl;
        
        public string GetDogBreedDetailsUrl(string _breedId)
        {
            return string.Format(m_dogBreedDetailsUrlFormat, _breedId);
        }
    }
}