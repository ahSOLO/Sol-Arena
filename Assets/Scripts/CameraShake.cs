using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Variables
    private CinemachineBasicMultiChannelPerlin noiseBMCP;
    private float shakeTimer;
    public static CameraShake cS;
    [SerializeField] private GameObject mainCamera;
    private CinemachineVirtualCamera cVC;
    
    // Start is called before the first frame update
    void Start()
    {
        cS = this; // assign static variable
        cVC = GetComponent<CinemachineVirtualCamera>();
        noiseBMCP = cVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer < 0.15f) noiseBMCP.m_AmplitudeGain = 0.4f; // Reduce camera shake with 0.15 seconds remaining in timer
        }
        if (shakeTimer <= 0f)
        {
            cVC.Follow.rotation = Quaternion.identity; // Reset rotation of camera
            mainCamera.transform.rotation = Quaternion.identity; // Reset rotation of camera
            noiseBMCP.enabled = false; // Disable Camera Shake
        }
    }

    public void ShakeCamera(float intensity, float duration) // Camera shake with assigned intensity and duration, called in PlayerController's Attack function.
    {
        noiseBMCP.enabled = true;
        noiseBMCP.m_AmplitudeGain = intensity;
        shakeTimer = duration;
    }
}
