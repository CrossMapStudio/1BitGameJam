using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class cameraController : MonoBehaviour
{
    public static cameraController controller { get; private set; }
    Cinemachine.CinemachineVirtualCamera camera;

    float savedFOVValue, modifiedFOVValue = 60f;
    float currentFOVValue;

    public void Awake()
    {
        controller = this;
        camera = GetComponent<CinemachineVirtualCamera>();
        savedFOVValue = camera.m_Lens.FieldOfView;
        currentFOVValue = savedFOVValue;
    }

    public void cameraShakeStart(float duration, float freq, float amp, bool zoom = false)
    {
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = freq;
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amp;

        if (zoom)
            currentFOVValue = modifiedFOVValue;

        StartCoroutine(stopShake(duration));
    }

    public IEnumerator stopShake(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;

        currentFOVValue = savedFOVValue;
    }

    public void Update()
    {
        if (camera.m_Lens.FieldOfView != currentFOVValue)
        {
            camera.m_Lens.FieldOfView = Mathf.Lerp(camera.m_Lens.FieldOfView, currentFOVValue, Time.deltaTime * 8f);
        }
    }
}
