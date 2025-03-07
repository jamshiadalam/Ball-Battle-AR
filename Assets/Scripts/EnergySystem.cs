using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BallBattleAR
{
    public class EnergySystem : MonoBehaviour
    {
        public Slider playerEnergySlider;
        public Slider enemyEnergySlider;

        private float playerEnergy = 0;
        private float enemyEnergy = 0;
        private bool energyRegenActive = false;

        public GameParameters parameters;

        void Start()
        {
            energyRegenActive = true;
        }

        void Update()
        {
            if (!energyRegenActive) return;

            playerEnergy = Mathf.Min(parameters.energyBarLimit, playerEnergy + parameters.energyRegenRate * Time.deltaTime);
            enemyEnergy = Mathf.Min(parameters.energyBarLimit, enemyEnergy + parameters.energyRegenRate * Time.deltaTime);

            playerEnergySlider.value = playerEnergy / parameters.energyBarLimit;
            enemyEnergySlider.value = enemyEnergy / parameters.energyBarLimit;
        }

        public void ResetEnergy()
        {
            playerEnergy = 0;
            enemyEnergy = 0;
            playerEnergySlider.value = 0;
            enemyEnergySlider.value = 0;
            energyRegenActive = false;
            StartCoroutine(StartEnergyRegenAfterDelay(2f));
        }

        private IEnumerator StartEnergyRegenAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            energyRegenActive = true;
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
