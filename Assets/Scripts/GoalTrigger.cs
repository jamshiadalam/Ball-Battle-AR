using UnityEngine;

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

                Debug.Log($"Ball entered: {gameObject.name} | isEnemyGate: {isEnemyGate} | Player Attacking: {playerIsAttacking}");

                if ((playerIsAttacking && isEnemyGate) || (!playerIsAttacking && !isEnemyGate))
                {
                    Debug.Log("Goal! Attacker Wins!");
                    GameManager.Instance.EndMatch("AttackerWin");
                }
                else
                {
                    Debug.Log("Goal! Opponent Wins!");
                    GameManager.Instance.EndMatch("DefenderWin");
                }

                Destroy(other.gameObject);
            }
        }
    }
}
