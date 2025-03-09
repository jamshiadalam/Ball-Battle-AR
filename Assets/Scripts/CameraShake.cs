using System.Collections;
using UnityEngine;

namespace ProjektSumperk
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Parameters")]
        [Tooltip("Duration of the camera shake in seconds")]
        public float shakeDuration = 1.0f;

        [Tooltip("Intensity of the camera shake")]
        public float shakeIntensity = 0.7f;

        [Tooltip("Frequency of the shake")]
        public float shakeFrequency = 2.0f;

        [Tooltip("Smoothness of the shake motion")]
        public float shakeSmoothness = 15.0f;

        [Header("Amplitude Multipliers")]
        [Tooltip("Amplitude multiplier for positional shake")]
        public Vector3 positionAmplitude = Vector3.one;

        [Tooltip("Amplitude multiplier for rotational shake")]
        public Vector3 rotationAmplitude = Vector3.one;

        [Header("Advanced Settings")]
        [Tooltip("Randomize the shake intensity slightly")]
        public bool randomizeIntensity = true;

        [Header("Shake Type")]
        [Tooltip("Choose between continuous shake or single-duration shake")]
        public ShakeType shakeType = ShakeType.SingleDuration;

        public enum ShakeType
        {
            SingleDuration,
            Continuous
        }

        private Transform cameraTransform;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool isShaking = false;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            // Check for the space button press
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (shakeType == ShakeType.SingleDuration && !isShaking)
                {
                    // Trigger the camera shake effect for a single duration
                    StartCoroutine(ShakeCoroutine());
                }
                else if (shakeType == ShakeType.Continuous)
                {
                    // Toggle continuous shake on/off
                    isShaking = !isShaking;

                    if (isShaking)
                    {
                        // Start continuous shake
                        StartCoroutine(ContinuousShakeCoroutine());
                    }
                    else
                    {
                        // Stop continuous shake and reset camera position
                        StopAllCoroutines();
                        cameraTransform.localPosition = originalPosition;
                    }
                }
            }
        }

        public IEnumerator ShakeCoroutine()
        {
            originalPosition = cameraTransform.localPosition;
            originalRotation = cameraTransform.localRotation;

            float elapsed = 0.0f;

            while (elapsed < shakeDuration)
            {
                float x = Mathf.PerlinNoise(0, elapsed * shakeFrequency) * 2 - 1;
                float y = Mathf.PerlinNoise(1, elapsed * shakeFrequency) * 2 - 1;
                float z = Mathf.PerlinNoise(2, elapsed * shakeFrequency) * 2 - 1;

                float intensity = shakeIntensity;
                if (randomizeIntensity)
                {
                    intensity *= Random.Range(0.9f, 1.1f);
                }

                Vector3 shakeOffset = new Vector3(x, y, z) * intensity;
                Vector3 positionShake = Vector3.Scale(shakeOffset, positionAmplitude);
                Vector3 rotationShake = Vector3.Scale(shakeOffset, rotationAmplitude);

                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalPosition + positionShake, Time.deltaTime * shakeSmoothness);
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, originalRotation * Quaternion.Euler(rotationShake), Time.deltaTime * shakeSmoothness);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset camera position and rotation
            cameraTransform.localPosition = originalPosition;
            cameraTransform.localRotation = originalRotation;
        }

        private IEnumerator ContinuousShakeCoroutine()
        {
            originalPosition = cameraTransform.localPosition;
            originalRotation = cameraTransform.localRotation;

            while (isShaking)
            {
                float x = Mathf.PerlinNoise(0, Time.time * shakeFrequency) * 2 - 1;
                float y = Mathf.PerlinNoise(1, Time.time * shakeFrequency) * 2 - 1;
                float z = Mathf.PerlinNoise(2, Time.time * shakeFrequency) * 2 - 1;

                float intensity = shakeIntensity;
                if (randomizeIntensity)
                {
                    intensity *= Random.Range(0.9f, 1.1f);
                }

                Vector3 shakeOffset = new Vector3(x, y, z) * intensity;
                Vector3 positionShake = Vector3.Scale(shakeOffset, positionAmplitude);
                Vector3 rotationShake = Vector3.Scale(shakeOffset, rotationAmplitude);

                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalPosition + positionShake, Time.deltaTime * shakeSmoothness);
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, originalRotation * Quaternion.Euler(rotationShake), Time.deltaTime * shakeSmoothness);

                yield return null;
            }
        }
    }
}