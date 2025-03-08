using UnityEngine;

namespace BallBattleAR
{
    public class MazeBallController : MonoBehaviour
    {
        private bool isDragging = false;
        private Vector3 offset;
        private Camera mainCamera;

        public float dragSpeed = 10f;
        private Rigidbody rb;

        void Start()
        {
            mainCamera = Camera.main;
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    offset = transform.position - hit.point;
                }
            }

            if (isDragging && Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                float distance;

                if (plane.Raycast(ray, out distance))
                {
                    Vector3 targetPosition = ray.GetPoint(distance) + offset;
                    targetPosition.y = transform.position.y;
                    rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Goal"))
            {
                Debug.Log("Ball Reached Enemy Gate! Player Wins!");
                GameManager.Instance.EndMatch("AttackerWin");
                GameManager.Instance.MazeGameOver("Player Wins!");
            }
        }
    }
}