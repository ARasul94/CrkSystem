using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(menuName = "Configs/Clicker Config")]
    public class ClickerConfig: ScriptableObject
    {
        public int m_initialEnergy = 100;
        public int m_maxEnergy = 1000;
        public int m_energyPerClick = 1;
        public int m_currencyPerClick = 1;
        public int m_autoClickIntervalSeconds = 3;
        public int m_energyRestoreIntervalSeconds = 10;
        public int m_energyRestoreAmount = 10;
    }
}