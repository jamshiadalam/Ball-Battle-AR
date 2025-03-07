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

                if ((playerIsAttacking && isEnemyGate) || (!playerIsAttacking && !isEnemyGate))
                {
                    Debug.Log("Goal! Attacker Wins!");
                    GameManager.Instance.EndMatch(true);
                }
                else
                {
                    Debug.Log("Goal! Defender Wins!");
                    GameManager.Instance.EndMatch(false);
                }

                Destroy(other.gameObject);
            }
        }
    }
}
