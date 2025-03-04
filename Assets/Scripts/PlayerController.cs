using UnityEngine;

namespace BallBattleAR
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject attackerPrefab, defenderPrefab;
        public Transform gameBoard;
        public GameObject playerField, enemyField;
        public EnergySystem energySystem;
        public GameParameters parameters;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    bool isPlayerTurn = GameManager.Instance.IsPlayerAttacking();
                    bool clickedOnPlayerField = hit.transform == playerField.transform;
                    bool clickedOnEnemyField = hit.transform == enemyField.transform;

                    if (isPlayerTurn && clickedOnPlayerField)
                    {
                        SpawnSoldier(attackerPrefab, hit.point, parameters.attackerEnergyCost, true);
                    }
                    else if (!isPlayerTurn && clickedOnEnemyField)
                    {
                        SpawnSoldier(defenderPrefab, hit.point, parameters.defenderEnergyCost, false);
                    }
                }
            }
        }

        void SpawnSoldier(GameObject prefab, Vector3 position, float cost, bool isPlayer)
        {
            if (energySystem.CanSpawn(isPlayer, cost))
            {
                Instantiate(prefab, position, Quaternion.identity);
                energySystem.SpendEnergy(isPlayer, cost);
            }
        }
    }

}