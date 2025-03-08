using ProjektSumperk;
using UnityEngine;

namespace BallBattleAR
{
    public class GoalTrigger : MonoBehaviour
    {
        public bool isEnemyGate;
        public ParticleSystem enemyParticle;
        public ParticleSystem playerParticle;
        public AudioSource goalHit;

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

                if(isEnemyGate)
                {
                    enemyParticle.gameObject.SetActive(true);
                    enemyParticle.Play(true);
                } 
                else
                {
                    playerParticle.gameObject.SetActive(true);
                    playerParticle.Play(true);
                }

                goalHit.Play();
                Destroy(other.gameObject);
            }
        }
    }
}
