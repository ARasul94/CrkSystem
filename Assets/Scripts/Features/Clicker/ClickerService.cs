using Core.Configs;
using UnityEngine;

namespace Features.Clicker
{
    public class ClickerService
    {
        private readonly ClickerModel m_model;
        private readonly ClickerConfig m_config;

        public ClickerService(
            ClickerModel _model,
            ClickerConfig _config)
        {
            m_model = _model;
            m_config = _config;
        }

        public bool TryClick()
        {
            if (m_model.currentEnergy < m_config.energyCostPerClick)
            {
                Debug.Log("[ClickerService] Not enough energy");
                return false;
            }

            m_model.SetEnergy(m_model.currentEnergy - m_config.energyCostPerClick);
            m_model.SetCurrency(m_model.currentCurrency + m_config.currencyPerClick);

            return true;
        }

        public void RestoreEnergy()
        {
            var newEnergy = Mathf.Min(
                m_model.currentEnergy + m_config.energyRestoreAmount,
                m_config.maxEnergy);

            m_model.SetEnergy(newEnergy);
        }
    }
}