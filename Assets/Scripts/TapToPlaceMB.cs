using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

namespace BallBattleAR
{
    [RequireComponent(typeof(ARRaycastManager))]
    public class TapToPlaceMB : MonoBehaviour
    {
        [SerializeField] ARSession m_Session;
        public GameObject objectToPlace;
        private GameObject spawnedObject;
        private Vector2 touchPosition;
        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        bool isMove = false;
        bool isRotate = false;
        bool isScale = false;
        bool isToggle = false;
        public Sprite close, menu;
        public Image menuButton;
        public GameObject menuPanel;

        static List<ARRaycastHit> hits = new List<ARRaycastHit>();


        private void Awake()
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            m_Session.enabled = true;
        }

        void Start()
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
        }

        private bool isTouchOverUI = false;

        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        isTouchOverUI = true;
                        return;
                    }
                    else
                    {
                        isTouchOverUI = false;
                    }
                }

                if (isTouchOverUI)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        isTouchOverUI = false;
                    }
                    return;
                }

                if (isRotate && spawnedObject != null)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 deltaPosition = touch.deltaPosition;
                        float rotationSpeed = 0.2f;
                        spawnedObject.transform.Rotate(0, -deltaPosition.x * rotationSpeed, 0);
                    }
                }

                if (isScale && spawnedObject != null)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 deltaPosition = touch.deltaPosition;
                        float scaleSpeed = 0.01f;

                        float scaleChange = deltaPosition.y * scaleSpeed;

                        Vector3 newScale = spawnedObject.transform.localScale + new Vector3(scaleChange, scaleChange, scaleChange);
                        newScale = Vector3.Max(newScale, new Vector3(0.3f, 0.3f, 0.3f));
                        newScale = Vector3.Min(newScale, new Vector3(1f, 1f, 1f));

                        spawnedObject.transform.localScale = newScale;
                    }
                }
            }

            if (arRaycastManager == null)
            {
                Debug.LogError("arRaycastManager is not initialized. Assign it in the Inspector.");
                return;
            }

            if (hits == null)
            {
                hits = new List<ARRaycastHit>();
            }

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;

                if (spawnedObject == null)
                {
                    if (objectToPlace == null)
                    {
                        return;
                    }

                    spawnedObject = Instantiate(objectToPlace, hitPose.position, objectToPlace.transform.rotation);

                    foreach (var plane in arPlaneManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                    arPlaneManager.enabled = false;
                }
                else
                {
                    if (isMove)
                    {
                        Debug.Log("Dragging object.");
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
            else
            {
                Debug.Log("No AR plane hit detected.");
            }
        }


        private bool TryGetTouchPosition(out Vector2 touchPosition)
        {
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        

        public void Move()
        {
            if (spawnedObject == null)
            {
                return;
            }

            isMove = true;
            isRotate = false;
            isScale = false;
        }

        public void Rotate()
        {
            if (spawnedObject == null)
            {
                return;
            }

            isMove = false;
            isRotate = true;
            isScale = false;
        }

        public void Scale()
        {
            if (spawnedObject == null)
            {
                return;
            }

            isMove = false;
            isRotate = false;
            isScale = true;
        }

        public void ResetTransform()
        {
            isToggle = !isToggle;
            isMove = false;
            isRotate = false;
            isScale = false;
            

            if (isToggle)
            {
                menuButton.sprite = close;
                menuPanel.SetActive(true);
                GameManager.Instance.isGameStarted = false;
            }
            else
            {
                menuButton.sprite = menu;
                menuPanel.SetActive(false);
                GameManager.Instance.isGameStarted = true;
            }
        }
    }
}