using UnityEngine;

namespace BallBattleAR
{
    public class Ball : MonoBehaviour
    {
        private Transform carrier;
        public float passSpeed = 5f;

        void Update()
        {
            if (carrier != null)
                transform.position = Vector3.Lerp(transform.position, carrier.position, Time.deltaTime * passSpeed);
        }

        public void SetCarrier(Transform newCarrier)
        {
            carrier = newCarrier;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Goal"))
            {
                Debug.Log("GOAL SCORED!");
                Destroy(gameObject);
            }
        }
    }
}
