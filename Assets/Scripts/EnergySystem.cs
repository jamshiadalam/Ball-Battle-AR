using UnityEngine;
using UnityEngine.UI;

namespace BallBattleAR
{
    public class EnergySystem : MonoBehaviour
    {
        public Slider playerEnergySlider;
        public Slider enemyEnergySlider;

        private float playerEnergy = 0;
        private float enemyEnergy = 0;

        public GameParameters parameters;

        void Start()
        {
            playerEnergy = 0;
            enemyEnergy = 0;
        }

        void Update()
        {
            playerEnergy = Mathf.Min(parameters.energyBarLimit, playerEnergy + parameters.energyRegenRate * Time.deltaTime);
            enemyEnergy = Mathf.Min(parameters.energyBarLimit, enemyEnergy + parameters.energyRegenRate * Time.deltaTime);

            playerEnergySlider.value = playerEnergy / parameters.energyBarLimit;
            enemyEnergySlider.value = enemyEnergy / parameters.energyBarLimit;
        }

        public bool CanSpawn(bool isPlayer, float cost)
        {
            return isPlayer ? playerEnergy >= cost : enemyEnergy >= cost;
        }

        public void SpendEnergy(bool isPlayer, float cost)
        {
            if (isPlayer && playerEnergy >= cost)
                playerEnergy -= cost;
            else if (!isPlayer && enemyEnergy >= cost)
                enemyEnergy -= cost;
        }
    }
}