using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace BallBattleAR
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameParameters parameters;
        public Transform rootContainer;
        private int currentMatch = 1;
        private float timer;
        private bool isPlayerAttacking = true;

        private int playerWins = 0;
        private int enemyWins = 0;

        public TMP_Text timerText, matchText, playerGameStateText, enemyGameStateText, gameOverText;
        public GameObject playerField, enemyField;
        public GameObject ballPrefab;
        private GameObject ballInstance;
        private EnergySystem energySystem;
        private bool matchEnded = false;
        public GameObject GameOverScreen;
        public GameObject PausedScreen;
        public ParticleSystem enemyParticle;
        public ParticleSystem playerParticle;
        public bool isGameStarted = false;
        public GameObject MazeGame;
        public GameObject Instruction;

        void Awake() { Instance = this; }

        void Start()
        {
            energySystem = FindObjectOfType<EnergySystem>();
            timer = parameters.matchTimeLimit;
        }

        void Update()
        {
            if (!isGameStarted) { return; }

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

            Bounds fieldBounds = fieldCollider.bounds;

            float marginX = fieldBounds.size.x * 0.15f;
            float marginZ = fieldBounds.size.z * 0.15f;

            float randomX = Random.Range(fieldBounds.min.x + marginX, fieldBounds.max.x - marginX);
            float randomZ = Random.Range(fieldBounds.min.z + marginZ, fieldBounds.max.z - marginZ);

            float gameBoardY = GameManager.Instance.rootContainer.position.y;

            float ballHeight = Mathf.Max(fieldBounds.max.y, gameBoardY) + 0.1f;

            Vector3 spawnPosition = new Vector3(randomX, ballHeight, randomZ);

            ballInstance = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            ballInstance.tag = "Ball";

            ballInstance.transform.SetParent(GameManager.Instance.rootContainer, true);
        }



        public void EndMatch(string resultType)
        {
            if (matchEnded) return;
            matchEnded = true;
            StartCoroutine(FindObjectOfType<ProjektSumperk.CameraShake>().ShakeCoroutine());
            Debug.Log($"EndMatch Called | Result: {resultType} | Player Attacking: {isPlayerAttacking}");
            GameObject.Find("GoalHit").GetComponent<AudioSource>().Play();

            switch (resultType)
            {
                case "AttackerWin":
                    if (isPlayerAttacking)
                    {
                        Debug.Log("Player Wins this match!");
                        playerWins++;
                        playerGameStateText.text = "PLAYER - WIN";
                        enemyGameStateText.text = "ENEMY - LOSE";
                        enemyParticle.gameObject.SetActive(true);
                        enemyParticle.Play(true);
                    }
                    else
                    {
                        Debug.Log("Enemy Wins this match!");
                        enemyWins++;
                        playerGameStateText.text = "PLAYER - LOSE";
                        enemyGameStateText.text = "ENEMY - WIN";
                        playerParticle.gameObject.SetActive(true);
                        playerParticle.Play(true);
                    }
                    break;

                case "DefenderWin":
                    if (isPlayerAttacking)
                    {
                        Debug.Log("Enemy Wins! Player Attackers Eliminated!");
                        enemyWins++;
                        playerGameStateText.text = "PLAYER - LOSE";
                        enemyGameStateText.text = "ENEMY - WIN";
                        playerParticle.gameObject.SetActive(true);
                        playerParticle.Play(true);
                    }
                    else
                    {
                        Debug.Log("Player Wins! Enemy Attackers Eliminated!");
                        playerWins++;
                        playerGameStateText.text = "PLAYER - WIN";
                        enemyGameStateText.text = "ENEMY - LOSE";
                        enemyParticle.gameObject.SetActive(true);
                        enemyParticle.Play(true);
                    }
                    break;

                case "Draw":
                    Debug.Log("Match Draw! Time Expired!");
                    playerGameStateText.text = "MATCH DRAW!";
                    enemyGameStateText.text = "MATCH DRAW!";
                    break;
            }

            currentMatch++;
            Debug.Log($"Moving to Match {currentMatch}");

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
                gameOverText.text = "PLAYER WINS THE GAME!";

                timerText.text = "";
                GameOverScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else if (enemyWins > playerWins)
            {
                playerGameStateText.text = "PLAYER LOSES!";
                enemyGameStateText.text = "ENEMY WINS THE GAME!";
                gameOverText.text = "ENEMY WINS THE GAME!";

                timerText.text = "";
                GameOverScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                playerGameStateText.text = "GAME TIED! Play Maze Runner";
                enemyGameStateText.text = "GAME TIED! Play Maze Runner";
                gameOverText.text = "GAME TIED! Play Maze Runner";

                StartMazeGame();
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            PausedScreen.SetActive(true);
            isGameStarted = false;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            PausedScreen.SetActive(false);
            isGameStarted = true;
        }

        public bool IsPlayerAttacking()
        {
            return isPlayerAttacking;
        }

        public void GameStarted()
        {
            isGameStarted = true;
            StartMatch();
        }

        public void AppExit()
        {
            Application.Quit();
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

        public void MazeGameOver(string txt)
        {
            GameOverScreen.SetActive(true);
            gameOverText.text = txt;
        }

        public void StartMazeGame()
        {
            Instruction.SetActive(true);
            isGameStarted=false;
            MazeGame.SetActive(true);
            ballInstance.SetActive(false);
        }
    }
}
