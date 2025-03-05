using UnityEngine;

namespace BallBattleAR
{
    public class Defender : MonoBehaviour
    {
        private float normalSpeed;
        private float returnSpeed;
        private float detectionRange;
        private float spawnTime;
        private float inactivateTime;
        private int energyCost;

        private Transform originalPosition;
        private Transform targetAttacker;
        private bool isActive = false;
        private Renderer defenderRenderer;
        private Collider defenderCollider;

        void Start()
        {
            var parameters = GameManager.Instance.parameters;

            normalSpeed = parameters.defenderSpeed * Time.deltaTime;
            returnSpeed = parameters.returnSpeed * Time.deltaTime;
            spawnTime = parameters.spawnTime;
            inactivateTime = parameters.defenderReactivateTime;
            energyCost = (int)parameters.defenderEnergyCost;
            detectionRange = parameters.detectionRange * GameManager.Instance.GetBattlefieldWidth();

            originalPosition = transform;
            defenderRenderer = GetComponent<Renderer>();
            defenderCollider = GetComponent<Collider>();

            defenderRenderer.material.color = Color.gray;
            defenderCollider.enabled = false;
            Invoke(nameof(Activate), spawnTime);
        }

        void Activate()
        {
            isActive = true;
            defenderRenderer.material.color = Color.red;
            defenderCollider.enabled = true;
        }

        void Update()
        {
            if (!isActive) return;

            FindAttackerInRange();

            if (targetAttacker != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetAttacker.position, normalSpeed);
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
                    break;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attacker") && other.GetComponent<Attacker>().hasBall)
            {
                other.GetComponent<Attacker>().Deactivate();
                isActive = false;
                defenderRenderer.material.color = Color.gray;
                defenderCollider.enabled = false;
                Invoke(nameof(ReturnToPosition), inactivateTime);
            }
        }

        void ReturnToPosition()
        {
            isActive = true;
            transform.position = Vector3.MoveTowards(transform.position, originalPosition.position, returnSpeed);
        }
    }
}