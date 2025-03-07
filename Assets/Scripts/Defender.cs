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
                Debug.Log($"Defender chasing Attacker: {targetAttacker.name}");
            }
        }

        void FindAttackerInRange()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
            foreach (Collider col in colliders)
            {
                Attacker attacker = col.GetComponent<Attacker>();
                if (attacker != null && attacker.hasBall && attacker.isActive)
                {
                    targetAttacker = attacker.transform;
                    Debug.Log($"Defender locked onto Attacker: {targetAttacker.name}");
                    return;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attacker"))
            {
                Attacker attacker = other.GetComponent<Attacker>();
                if (attacker != null && attacker.hasBall)
                {
                    Debug.Log($"Defender caught Attacker: {attacker.name}");
                    attacker.Deactivate();

                    isActive = false;
                    defenderRenderer.material.color = Color.gray;
                    defenderCollider.enabled = false;
                    targetAttacker = null;

                    Invoke(nameof(ReturnToPosition), inactivateTime);
                }
            }
        }

        void ReturnToPosition()
        {
            StartCoroutine(MoveBackToOriginal());
        }

        System.Collections.IEnumerator MoveBackToOriginal()
        {
            while (Vector3.Distance(transform.position, originalPosition.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition.position, returnSpeed);
                yield return null;
            }

            isActive = true;
            defenderRenderer.material.color = Color.red;
            defenderCollider.enabled = true;
            Debug.Log("Defender is back in position and reactivated.");
        }
    }
}