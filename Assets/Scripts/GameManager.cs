using UnityEngine;
using TMPro;

namespace BallBattleAR
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameParameters parameters;

        private int currentMatch = 1;
        private float timer;
        private bool isPlayerAttacking = true;

        private int playerWins = 0;
        private int enemyWins = 0;

        public TMP_Text timerText, matchText, playerGameStateText, enemyGameStateText;
        public GameObject playerField, enemyField;
        public GameObject ballPrefab;
        private GameObject ballInstance;

        void Awake() { Instance = this; }

        void Start()
        {
            StartMatch();
        }

        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timerText.text = Mathf.Ceil(timer).ToString();
            }
            else
            {
                EndMatch(false);
            }
        }

        void StartMatch()
        {
            if (currentMatch > parameters.matchesPerGame)
            {
                GameOver();
                return;
            }

            timer = parameters.matchTimeLimit;

            if (isPlayerAttacking)
            {
                playerGameStateText.text = "PLAYER - ATTACKING";
                enemyGameStateText.text = "ENEMY - DEFENDING";
                SpawnBall(playerField);
            }
            else
            {
                playerGameStateText.text = "PLAYER - DEFENDING";
                enemyGameStateText.text = "ENEMY - ATTACKING";
                SpawnBall(enemyField);
            }

            matchText.text = "Match " + currentMatch;
            currentMatch++;
        }

        void SpawnBall(GameObject field)
        {
            if (ballInstance != null) Destroy(ballInstance);

            Collider fieldCollider = field.GetComponent<Collider>();
            if (fieldCollider == null)
            {
                Debug.LogError("Field collider is missing on " + field.name);
                return;
            }

            Vector3 fieldCenter = fieldCollider.bounds.center;
            Vector3 fieldSize = fieldCollider.bounds.size;

            float minX = fieldCenter.x - (fieldSize.x / 2) + 1f;
            float maxX = fieldCenter.x + (fieldSize.x / 2) - 1f;
            float minZ = fieldCenter.z - (fieldSize.z / 2) + 1f;
            float maxZ = fieldCenter.z + (fieldSize.z / 2) - 1f;

            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            float ballHeight = fieldCollider.bounds.max.y + 0.15f;

            Vector3 spawnPosition = new Vector3(randomX, ballHeight, randomZ);

            ballInstance = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            ballInstance.tag = "Ball";

            Debug.Log($"Ball Spawned at: {spawnPosition}");
        }

        public void EndMatch(bool attackerWon)
        {
            Debug.Log($"EndMatch Called | Attacker Won: {attackerWon} | Player Attacking: {isPlayerAttacking}");

            if (attackerWon)
            {
                if (isPlayerAttacking)
                {
                    Debug.Log("Player Wins this match!");
                    playerWins++;
                    playerGameStateText.text = "PLAYER - WIN";
                    enemyGameStateText.text = "ENEMY - LOSE";
                }
                else
                {
                    Debug.Log("Enemy Wins this match!");
                    enemyWins++;
                    playerGameStateText.text = "PLAYER - LOSE";
                    enemyGameStateText.text = "ENEMY - WIN";
                }
            }
            else
            {
                Debug.Log("Match Draw!");
                playerGameStateText.text = "MATCH DRAW!";
                enemyGameStateText.text = "MATCH DRAW!";
            }

            isPlayerAttacking = !isPlayerAttacking;

            Invoke(nameof(StartMatch), 2f);
        }

        void GameOver()
        {
            Debug.Log($"Game Over | Player Wins: {playerWins} | Enemy Wins: {enemyWins}");

            if (playerWins > enemyWins)
            {
                playerGameStateText.text = "PLAYER WINS THE GAME!";
                enemyGameStateText.text = "ENEMY LOSES!";
            }
            else if (enemyWins > playerWins)
            {
                playerGameStateText.text = "PLAYER LOSES!";
                enemyGameStateText.text = "ENEMY WINS THE GAME!";
            }
            else
            {
                playerGameStateText.text = "GAME TIED! EXTRA ROUND?";
                enemyGameStateText.text = "GAME TIED!";
            }
        }

        public bool IsPlayerAttacking()
        {
            return isPlayerAttacking;
        }

        public float GetBattlefieldWidth()
        {
            if (playerField == null || enemyField == null)
            {
                Debug.LogError("playerField or enemyField is not assigned in GameManager!");
                return 10f;
            }

            float playerWidth = playerField.GetComponent<Collider>().bounds.size.x;
            float enemyWidth = enemyField.GetComponent<Collider>().bounds.size.x;

            return playerWidth + enemyWidth;
        }
    }
}