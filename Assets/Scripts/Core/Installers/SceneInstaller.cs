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

        public override void InstallBindings()
        {
            Debug.Log("[SceneInstaller] InstallBindings");

            BindViews();
            BindPresenters();
            
            BindTest();
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
        
        private void BindTest()
        {
            Container
                .BindInterfacesAndSelfTo<HttpClientTestPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}