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
        private EnergySystem energySystem;
        private bool matchEnded = false;

        void Awake() { Instance = this; }

        void Start()
        {
            energySystem = FindObjectOfType<EnergySystem>();
            StartMatch();
            timer = parameters.matchTimeLimit;
        }

        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                timerText.text = "Time Left " + Mathf.Ceil(timer).ToString();
            }
            else if (!matchEnded)
            {
                Debug.Log("Timeout! Match is a Draw!");
                EndMatch("Draw");
            }
        }

        void StartMatch()
        {
            if (currentMatch > parameters.matchesPerGame)
            {
                GameOver();
                return;
            }

            matchEnded = false;
            

            isPlayerAttacking = (currentMatch % 2 != 0);

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

            matchText.text = $"Match {currentMatch}";
        }

        void SpawnBall(GameObject field)
        {
            if (ballInstance != null) Destroy(ballInstance);

            Collider fieldCollider = field.GetComponent<Collider>();
            if (fieldCollider == null)
            {
                Debug.LogError($"Field collider is missing on {field.name}");
                return;
            }

            Vector3 fieldCenter = fieldCollider.bounds.center;
            Vector3 fieldSize = fieldCollider.bounds.size;

            float minX = fieldCenter.x - (fieldSize.x / 2) + 1f;
            float maxX = fieldCenter.x + (fieldSize.x / 2) - 1f;
            float minZ = fieldCenter.z - (fieldSize.z / 2) + 1f;
            float maxZ = fieldCenter.z + (fieldSize.z / 2) - 1f;
            float ballHeight = fieldCollider.bounds.max.y + 0.15f;

            Vector3 spawnPosition = new Vector3(
                Random.Range(minX, maxX),
                ballHeight,
                Random.Range(minZ, maxZ)
            );

            ballInstance = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            ballInstance.tag = "Ball";
        }

        public void EndMatch(string resultType)
        {
            if (matchEnded) return;
            matchEnded = true;

            Debug.Log($"EndMatch Called | Result: {resultType} | Player Attacking: {isPlayerAttacking}");

            switch (resultType)
            {
                case "AttackerWin":
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
                    break;

                case "DefenderWin":
                    if (isPlayerAttacking)
                    {
                        Debug.Log("Enemy Wins! Player Attackers Eliminated!");
                        enemyWins++;
                        playerGameStateText.text = "PLAYER - LOSE";
                        enemyGameStateText.text = "ENEMY - WIN";
                    }
                    else
                    {
                        Debug.Log("Player Wins! Enemy Attackers Eliminated!");
                        playerWins++;
                        playerGameStateText.text = "PLAYER - WIN";
                        enemyGameStateText.text = "ENEMY - LOSE";
                    }
                    break;

                case "Draw":
                    Debug.Log("Match Draw! Time Expired!");
                    playerGameStateText.text = "MATCH DRAW!";
                    enemyGameStateText.text = "MATCH DRAW!";
                    break;
            }

            currentMatch++;
            Debug.Log($"➡️ Moving to Match {currentMatch}");

            energySystem.ResetEnergy();
            RemoveAll();

            Debug.Log("Waiting 2 seconds before starting next match...");
            Invoke(nameof(StartMatch), 2f);
        }

        void GameOver()
        {
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

        void RemoveAll()
        {
            GameObject[] attackers = GameObject.FindGameObjectsWithTag("Attacker");
            GameObject[] defenders = GameObject.FindGameObjectsWithTag("Defender");

            foreach (GameObject attacker in attackers)
            {
                Destroy(attacker);
            }
            foreach (GameObject defender in defenders)
            {
                Destroy(defender);
            }
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
