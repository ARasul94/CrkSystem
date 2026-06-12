using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(menuName = "Configs/Clicker Config")]
    public class ClickerConfig: ScriptableObject
    {
        [Header("Currency")]
        [SerializeField] private int m_initialCurrency = 0;
        [SerializeField] private int m_currencyPerClick = 1;

        [Header("Energy")]
        [SerializeField] private int m_initialEnergy = 1000;
        [SerializeField] private int m_maxEnergy = 1000;
        [SerializeField] private int m_energyCostPerClick = 1;

        [Header("Auto Click")]
        [SerializeField] private float m_autoClickIntervalSeconds = 3f;

        [Header("Energy Restore")]
        [SerializeField] private float m_energyRestoreIntervalSeconds = 10f;
        [SerializeField] private int m_energyRestoreAmount = 10;

        public int initialCurrency => m_initialCurrency;
        public int currencyPerClick => m_currencyPerClick;

        public int initialEnergy => m_initialEnergy;
        public int maxEnergy => m_maxEnergy;
        public int energyCostPerClick => m_energyCostPerClick;

        public float autoClickIntervalSeconds => m_autoClickIntervalSeconds;

        public float energyRestoreIntervalSeconds => m_energyRestoreIntervalSeconds;
        public int energyRestoreAmount => m_energyRestoreAmount;
    }
}