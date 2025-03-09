using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BallBattleAR
{
    public class CameraPermissionUI : MonoBehaviour
    {
        public GameObject PermissionPanel;
        public Slider toggle;

        private void Start()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                toggle.value = 0;
            }
            else
            {
                toggle.value = 1;
                PermissionPanel.SetActive(false);
            }
        }

        IEnumerator StartCamera()
        {
            FindWebCams();

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.Log("Camera found");
                SceneManager.LoadScene("AR");
            }
            else
            {
                Debug.Log("Camera not found");
            }
        }

        void FindWebCams()
        {
            foreach (var device in WebCamTexture.devices)
            {
                Debug.Log("Name: " + device.name);
            }
        }

        public void Approve()
        {
            StartCoroutine(StartCamera());
        }

        public void Deny()
        {
            PermissionPanel.SetActive(false);
        }

        public void ToggleAR()
        {
            if (toggle.value == 0)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                PermissionPanel.SetActive(true);
            }
        }
    }
}