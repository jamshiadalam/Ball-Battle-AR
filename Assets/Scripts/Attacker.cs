using UnityEngine;

namespace BallBattleAR
{
    public class Attacker : MonoBehaviour
    {
        private Transform ball;
        private Transform goal;
        private Transform opponentFence;
        private Renderer attackerRenderer;
        private Collider attackerCollider;
        public bool hasBall = false;
        public bool isActive = false;
        private GameParameters parameters;

        void Start()
        {
            parameters = GameManager.Instance.parameters;

            ball = GameObject.FindGameObjectWithTag("Ball")?.transform;
            goal = FindOpponentGoal();
            opponentFence = FindOpponentFence();

            attackerRenderer = transform.GetChild(2).GetComponent<Renderer>();
            attackerCollider = GetComponent<Collider>();

            attackerRenderer.material.color = Color.gray;
            attackerCollider.enabled = false;
            Invoke(nameof(Activate), parameters.spawnTime);
        }

        void Activate()
        {
            isActive = true;
            attackerRenderer.material.color = Color.cyan;
            attackerCollider.enabled = true;
        }

        void Update()
        {
            if (!isActive) return;

            if (hasBall && goal != null)
            {
                MoveToTarget(goal.position, parameters.carryingSpeed);
                RotateTowards(goal.position);
                if (ball != null)
                {
                    ball.position = transform.position;
                }
            }
            else if (!hasBall && ball != null)
            {
                MoveToTarget(ball.position, parameters.attackerSpeed);
                RotateTowards(ball.position);
            }
            else if (!hasBall && ball == null && opponentFence != null)
            {
                MoveToTarget(opponentFence.position, parameters.attackerSpeed);
                RotateTowards(opponentFence.position);
            }

            UpdateBallIndicator();
        }

        void UpdateBallIndicator()
        {
            Attacker[] attackers = FindObjectsOfType<Attacker>();
            bool foundBallHolder = false;

            foreach (var attacker in attackers)
            {
                if (attacker.hasBall)
                {
                    if (!foundBallHolder)
                    {
                        attacker.transform.GetChild(1).gameObject.SetActive(true);
                        foundBallHolder = true;
                    }
                    else
                    {
                        attacker.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
                else
                {
                    attacker.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }

        void MoveToTarget(Vector3 target, float speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        void RotateTowards(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameters.rotationSpeed * Time.deltaTime);
            }
        }

        public void PickUpBall()
        {
            hasBall = true;
            transform.GetChild(1).gameObject.SetActive(true);
        }

        public void PassBall(Transform newTarget)
        {
            hasBall = false;
            transform.GetChild(1).gameObject.SetActive(false);

            if (ball != null && newTarget != null)
            {
                ball.position = Vector3.Lerp(ball.position, newTarget.position, parameters.ballSpeed * Time.deltaTime);

                Attacker newAttacker = newTarget.GetComponent<Attacker>();
                if (newAttacker != null)
                {
                    newAttacker.PickUpBall();
                    newAttacker.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }

        public void Deactivate()
        {
            if (!isActive) return;

            isActive = false;
            hasBall = false;
            attackerRenderer.material.color = Color.gray;
            attackerCollider.enabled = false;
            transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("Attacker is deactivated for " + parameters.attackerReactivateTime + " seconds.");

            Invoke(nameof(Activate), parameters.attackerReactivateTime);
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball") && !hasBall)
            {
                Attacker[] attackers = FindObjectsOfType<Attacker>();
                bool ballAlreadyHeld = false;

                foreach (var attacker in attackers)
                {
                    if (attacker.hasBall)
                    {
                        ballAlreadyHeld = true;
                        break;
                    }
                }

                if (!ballAlreadyHeld)
                {
                    hasBall = true;
                    Debug.Log("Attacker picked up the Ball!");
                }

                UpdateBallIndicator();
                GameObject.Find("SoldierHit").GetComponent<AudioSource>().Play();
            }

            if (other.CompareTag("Goal") && hasBall)
            {
                GameManager.Instance.EndMatch("AttackerWin");
                Destroy(gameObject);
            }

            if (other.CompareTag("Fence") && opponentFence != null && other.transform == opponentFence)
            {
                Debug.Log("Attacker reached the opponent's Fence and was destroyed.");
                Destroy(gameObject);
            }

            if (other.CompareTag("Defender") && hasBall)
            {
                Transform nearestAttacker = FindNearestAttacker();
                if (nearestAttacker != null)
                {
                    Debug.Log("Attacker was caught by a Defender! Passing Ball to nearest Attacker.");
                    GameObject.Find("SoldierHit").GetComponent<AudioSource>().Play();
                    PassBall(nearestAttacker);
                }
                else
                {
                    Debug.Log("No nearby Attacker to pass to! Defender Wins!");
                    GameManager.Instance.EndMatch("DefenderWin");
                }
                Deactivate();
            }
        }


        Transform FindNearestAttacker()
        {
            Attacker[] attackers = FindObjectsOfType<Attacker>();
            float minDistance = Mathf.Infinity;
            Transform nearest = null;

            foreach (var attacker in attackers)
            {
                if (attacker != this && attacker.isActive)
                {
                    float distance = Vector3.Distance(transform.position, attacker.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = attacker.transform;
                    }
                }
            }

            return nearest;
        }

        Transform FindOpponentGoal()
        {
            GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
            GameObject playerField = GameManager.Instance.playerField;
            GameObject enemyField = GameManager.Instance.enemyField;

            bool isPlayerAttacking = GameManager.Instance.IsPlayerAttacking();

            foreach (GameObject goal in goals)
            {
                float distanceToPlayerField = Vector3.Distance(goal.transform.position, playerField.transform.position);
                float distanceToEnemyField = Vector3.Distance(goal.transform.position, enemyField.transform.position);

                if (isPlayerAttacking && distanceToEnemyField < distanceToPlayerField)
                {
                    return goal.transform;
                }
                else if (!isPlayerAttacking && distanceToPlayerField < distanceToEnemyField)
                {
                    return goal.transform;
                }
            }

            Debug.LogError("Could not find the correct opponent Goal!");
            return null;
        }

        Transform FindOpponentFence()
        {
            GameObject[] fences = GameObject.FindGameObjectsWithTag("Fence");
            GameObject playerField = GameManager.Instance.playerField;
            GameObject enemyField = GameManager.Instance.enemyField;

            bool isPlayerAttacking = GameManager.Instance.IsPlayerAttacking();

            foreach (GameObject fence in fences)
            {
                float distanceToPlayerField = Vector3.Distance(fence.transform.position, playerField.transform.position);
                float distanceToEnemyField = Vector3.Distance(fence.transform.position, enemyField.transform.position);

                if (isPlayerAttacking && distanceToEnemyField < distanceToPlayerField)
                {
                    return fence.transform;
                }
                else if (!isPlayerAttacking && distanceToPlayerField < distanceToEnemyField)
                {
                    return fence.transform;
                }
            }

            Debug.LogError("Could not find the correct opponent Fence!");
            return null;
        }
    }
}
