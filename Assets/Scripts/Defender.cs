using UnityEngine;

namespace BallBattleAR
{
    public class Defender : MonoBehaviour
    {
        public float detectionRange = 5f;
        public float chaseSpeed = 4f;
        private Transform targetAttacker;
        private bool isActive = false;
        private Renderer defenderRenderer;
        private Collider defenderCollider;

        void Start()
        {
            defenderRenderer = GetComponent<Renderer>();
            defenderCollider = GetComponent<Collider>();
        }

        void Update()
        {
            if (!isActive) FindAttackerInRange();
            else if (targetAttacker != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetAttacker.position, chaseSpeed * Time.deltaTime);
            }
        }

        void FindAttackerInRange()
        {
            Collider[] attackers = Physics.OverlapSphere(transform.position, detectionRange);
            foreach (Collider col in attackers)
            {
                if (col.CompareTag("Attacker"))
                {
                    targetAttacker = col.transform;
                    isActive = true;
                    break;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attacker"))
            {
                other.GetComponent<Attacker>().Deactivate(2.5f);
                isActive = false;
                Invoke(nameof(Reactivate), 4f);
            }
        }

        void Reactivate()
        {
            isActive = true;
        }
    }
}