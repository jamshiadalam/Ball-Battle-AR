﻿using UnityEngine;

namespace BallBattleAR
{
    public class GoalTrigger : MonoBehaviour
    {
        public bool isEnemyGate;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                bool playerIsAttacking = GameManager.Instance.IsPlayerAttacking();

                Debug.Log($"[GoalTrigger] Ball entered: {gameObject.name} | isEnemyGate: {isEnemyGate} | Player Attacking: {playerIsAttacking}");

                if (playerIsAttacking && isEnemyGate)
                {
                    Debug.Log("Goal! Player Wins!");
                    GameManager.Instance.EndMatch(true);
                }
                else if (!playerIsAttacking && !isEnemyGate)
                {
                    Debug.Log("Goal! Enemy Wins!");
                    GameManager.Instance.EndMatch(true);
                }
                else if (playerIsAttacking && !isEnemyGate)
                {
                    Debug.Log("Goal! Enemy Wins!");
                    GameManager.Instance.EndMatch(true);
                }
                else if (!playerIsAttacking && isEnemyGate)
                {
                    Debug.Log("Goal! Player Wins!");
                    GameManager.Instance.EndMatch(true);
                }

                Destroy(other.gameObject);
            }
        }
    }
}
