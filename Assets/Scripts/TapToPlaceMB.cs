using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

        //transparencySlider.onValueChanged.AddListener(AdjustTransparency);

    }

    // Track if touch started over UI
    private bool isTouchOverUI = false;

    void Update()
    {
        if (spawnedObject != null)
            return;

        // Ensure touch position is valid
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        // Check if the touch is over a UI element
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Debug.Log("Touch started over a UI element. Ignoring.");
                    isTouchOverUI = true; // Mark the touch as starting on a UI element
                    return;
                }
                else
                {
                    Debug.Log("Touch started NOT over UI. Proceeding.");
                    isTouchOverUI = false; // Reset flag if touch starts elsewhere
                }
            }

            // If the touch started over a UI element, ignore it until the touch ends
            if (isTouchOverUI)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("Touch ended. Resetting UI touch flag.");
                    isTouchOverUI = false; // Reset flag when touch ends
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
                    float scaleSpeed = 0.01f; // Adjust this for faster or slower scaling

                    // Calculate the new scale based on vertical swipe direction
                    float scaleChange = deltaPosition.y * scaleSpeed;

                    // Apply the scale change and clamp between 0.5 and 2
                    Vector3 newScale = spawnedObject.transform.localScale + new Vector3(scaleChange, scaleChange, scaleChange);
                    newScale = Vector3.Max(newScale, new Vector3(0.5f, 0.5f, 0.5f)); // Min scale
                    newScale = Vector3.Min(newScale, new Vector3(2f, 2f, 2f));       // Max scale

                    spawnedObject.transform.localScale = newScale;
                }
            }
        }

        // Check if arRaycastManager is initialized
        if (arRaycastManager == null)
        {
            Debug.LogError("arRaycastManager is not initialized. Assign it in the Inspector.");
            return;
        }

        // Perform the raycast and ensure hits is initialized
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
                    Debug.LogError("objectToPlace is not assigned. Assign a prefab in the Inspector.");
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

    public void MovePanel()
    {
        isMove = true;
        isRotate = false;
        isScale = false;
    }

    public void RotatePanel()
    {
        isMove = false;
        isRotate = true;
        isScale = false;
    }

    public void ScalePanel()
    {
        isMove = false;
        isRotate = false;
        isScale = true;
    }
}