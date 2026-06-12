using Core.Configs;
using Core.Http;
using Core.RequestQueue;
using Features.Clicker;
using Features.Dogs;
using Features.Navigation;
using Features.Navigation.Tabs;
using Features.Weather;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Navigation")]
        [SerializeField] private NavigationView m_navigationView;

        [Header("Tabs")]
        [SerializeField] private ClickerTabView m_clickerTabView;
        [SerializeField] private WeatherTabView m_weatherTabView;
        [SerializeField] private DogsTabView m_dogsTabView;
        
        [Header("Clicker")]
        [SerializeField] private ClickerConfig m_clickerConfig;
        [SerializeField] private CurrencyFlyView m_currencyFlyPrefab;
        [SerializeField] private Transform m_currencyFlyPoolParent;

        public override void InstallBindings()
        {
            Debug.Log("[SceneInstaller] InstallBindings");

            BindViews();
            BindClicker();
            BindPools();
            BindPresenters();
        }

        private void BindViews()
        {
            Container.Bind<NavigationView>().FromInstance(m_navigationView).AsSingle();

            Container.Bind<ClickerTabView>().FromInstance(m_clickerTabView).AsSingle();
            Container.Bind<WeatherTabView>().FromInstance(m_weatherTabView).AsSingle();
            Container.Bind<DogsTabView>().FromInstance(m_dogsTabView).AsSingle();
        }

        private void BindPresenters()
        {
            Container.BindInterfacesAndSelfTo<ClickerTabPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<WeatherTabPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<DogsTabPresenter>().AsSingle();

            Container
                .BindInterfacesAndSelfTo<NavigationPresenter>()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindClicker()
        {
            if (m_clickerConfig == null)
                throw new MissingReferenceException($"{nameof(ClickerConfig)} is not assigned");

            Container.Bind<ClickerConfig>().FromInstance(m_clickerConfig).AsSingle();

            Container
                .Bind<ClickerModel>()
                .AsSingle()
                .WithArguments(
                    m_clickerConfig.initialCurrency,
                    m_clickerConfig.initialEnergy);

            Container.Bind<ClickerService>().AsSingle();
        }
        
        private void BindPools()
        {
            Container
                .BindMemoryPool<CurrencyFlyView, CurrencyFlyView.Pool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(m_currencyFlyPrefab)
                .UnderTransform(m_currencyFlyPoolParent);
        }
        
        private void BindTest()
        {
            Container
                .BindInterfacesAndSelfTo<HttpClientTestPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}