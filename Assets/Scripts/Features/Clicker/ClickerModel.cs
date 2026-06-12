using System;
using UniRx;

namespace Features.Clicker
{
    public class ClickerModel : IDisposable
    {
        private readonly ReactiveProperty<int> m_currency;
        private readonly ReactiveProperty<int> m_energy;

        public IReadOnlyReactiveProperty<int> currency => m_currency;
        public IReadOnlyReactiveProperty<int> energy => m_energy;

        public int currentCurrency => m_currency.Value;
        public int currentEnergy => m_energy.Value;

        public ClickerModel(int _initialCurrency, int _initialEnergy)
        {
            m_currency = new ReactiveProperty<int>(_initialCurrency);
            m_energy = new ReactiveProperty<int>(_initialEnergy);
        }

        public void SetCurrency(int _value)
        {
            m_currency.Value = _value;
        }

        public void SetEnergy(int _value)
        {
            m_energy.Value = _value;
        }

        public void Dispose()
        {
            m_currency.Dispose();
            m_energy.Dispose();
        }
    }
}