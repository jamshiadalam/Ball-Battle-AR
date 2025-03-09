using UnityEngine;

namespace BallBattleAR
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject attackerPrefab;
        public GameObject defenderPrefab;
        public GameObject playerField;
        public GameObject enemyField;
        public EnergySystem energySystem;

        void Update()
        {
            if (!GameManager.Instance.isGameStarted) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == playerField || hit.collider.gameObject == enemyField)
                    {
                        SpawnSoldier(hit.point, hit.collider.gameObject);
                    }
                }
            }
        }

        void SpawnSoldier(Vector3 spawnPosition, GameObject clickedField)
        {
            bool isPlayerAttacking = GameManager.Instance.IsPlayerAttacking();
            bool isPlayerSide = clickedField == playerField;
            bool isEnemySide = clickedField == enemyField;

            GameObject soldierPrefab;
            int energyCost;
            bool isPlayerSpawning;

            if ((isPlayerAttacking && isPlayerSide) || (!isPlayerAttacking && isEnemySide))
            {
                soldierPrefab = attackerPrefab;
                energyCost = GameManager.Instance.parameters.attackerEnergyCost;
                isPlayerSpawning = isPlayerAttacking;
            }
            else
            {
                soldierPrefab = defenderPrefab;
                energyCost = GameManager.Instance.parameters.defenderEnergyCost;
                isPlayerSpawning = !isPlayerAttacking;
            }

            if (energySystem.CanSpawn(isPlayerSpawning, energyCost))
            {
                float fieldHeight = clickedField.GetComponent<Collider>().bounds.max.y;
                spawnPosition.y = fieldHeight + 0.1f;

                if(isEnemySide)
                {
                    Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
                } else
                {
                    Instantiate(soldierPrefab, spawnPosition, Quaternion.Euler(0,-180,0));
                }

                
                energySystem.SpendEnergy(isPlayerSpawning, energyCost);
            }
            else
            {
                Debug.Log("Not enough energy to spawn!");
            }
        }
    }
}
