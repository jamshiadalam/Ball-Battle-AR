using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace BallBattleAR
{
    public class MazeGenerator : MonoBehaviour
    {
        public GameObject wallPrefab;
        public Transform gameBoard;
        public int rows = 36;
        public int cols = 20;
        private int[,] mazeGrid;
        private List<GameObject> mazeWalls = new List<GameObject>();

        private float wallSizeX;
        private float wallSizeZ;

        public GameObject ballPrefab;
        public Transform PlayersGate;

        private float mazeTimer = 20f;
        private bool timerRunning = false;
        private bool ballReachedGoal = false;
        public TMP_Text timer;

        void Start()
        {
            GameManager.Instance.isGameStarted = false;
            if (wallPrefab != null)
            {
                Renderer wallRenderer = wallPrefab.GetComponent<Renderer>();
                if (wallRenderer != null)
                {
                    wallSizeX = wallRenderer.bounds.size.x;
                    wallSizeZ = wallRenderer.bounds.size.z;
                }
                else
                {
                    Debug.LogError("Wall prefab missing Renderer!");
                }
            }

            GenerateMaze();
            SpawnBallAtPlayerGate();
            StartMazeTimer();
        }

        void Update()
        {
            if (timerRunning && !ballReachedGoal)
            {
                mazeTimer -= Time.deltaTime;
                timer.text = "Time Left " + Mathf.Ceil(mazeTimer).ToString();

                if (mazeTimer <= 0)
                {
                    timerRunning = false;
                    Debug.Log("Time's Up! Enemy Wins!");
                    GameManager.Instance.EndMatch("DefenderWin");
                    GameManager.Instance.MazeGameOver("Enemy Wins!");
                }
            }
        }

        public void StartMazeTimer()
        {
            mazeTimer = 20f;
            timerRunning = true;
            ballReachedGoal = false;
        }

        public void GenerateMaze()
        {
            RemoveMaze();

            mazeGrid = new int[rows, cols];

            Bounds bounds = GetTotalFieldBounds();
            float fieldCenterX = bounds.center.x;
            float fieldCenterZ = bounds.center.z;

            float totalMazeWidth = cols * wallSizeX;
            float totalMazeHeight = rows * wallSizeZ;

            float startX = fieldCenterX - (totalMazeWidth / 2);
            float startZ = fieldCenterZ - (totalMazeHeight / 2);

            GenerateMazeGrid();
            CreatePathToEnemyGate();

            int wallCount = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (mazeGrid[row, col] == 1)
                    {
                        Vector3 position = new Vector3(startX + col * wallSizeX, 0.18f, startZ + row * wallSizeZ);
                        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                        wall.transform.SetParent(gameBoard);
                        mazeWalls.Add(wall);
                        wallCount++;
                    }
                }
            }

            Debug.Log($"Maze Generated with {wallCount} Walls!");
        }

        void GenerateMazeGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    mazeGrid[row, col] = (Random.value < 0.3f) ? 1 : 0;
                }
            }
        }

        void CreatePathToEnemyGate()
        {
            int pathCol = cols / 2;
            for (int row = rows - 1; row >= 0; row--)
            {
                mazeGrid[row, pathCol] = 0;

                if (Random.value < 0.5f && pathCol > 1)
                    pathCol--;
                else if (Random.value > 0.5f && pathCol < cols - 2)
                    pathCol++;
            }
        }

        public void RemoveMaze()
        {
            foreach (GameObject wall in mazeWalls)
            {
                Destroy(wall);
            }
            mazeWalls.Clear();
            Debug.Log("Maze Removed!");
        }

        Bounds GetTotalFieldBounds()
        {
            GameObject playerField = GameManager.Instance.playerField;
            GameObject enemyField = GameManager.Instance.enemyField;

            Collider playerCollider = playerField.GetComponent<Collider>();
            Collider enemyCollider = enemyField.GetComponent<Collider>();

            Bounds playerBounds = playerCollider.bounds;
            Bounds enemyBounds = enemyCollider.bounds;

            return new Bounds(
                (playerBounds.center + enemyBounds.center) / 2,
                playerBounds.size + enemyBounds.size
            );
        }

        public Vector3 GetPlayerGatePosition()
        {
            return PlayersGate.transform.position + new Vector3(0, -0.09f, -0.2f);
        }

        public void SpawnBallAtPlayerGate()
        {
            Vector3 spawnPos = GetPlayerGatePosition();
            Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        }
    }
}
