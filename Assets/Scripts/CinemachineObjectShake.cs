using UnityEngine;
using Cinemachine;
using System.Collections;

public class CinemachineObjectShake : MonoBehaviour
{
    public static CinemachineObjectShake Instance;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;
    private Vector3 originalOffset;
    private Coroutine shakeRoutine;

    void Awake()
    {
        Instance = this;

        vcam = GetComponent<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (transposer != null)
                originalOffset = transposer.m_TrackedObjectOffset;
        }
    }

    public void Shake(float duration = 0.2f, float strength = 0.3f)
    {
        if (transposer == null)
        {
            Debug.LogWarning("No Cinemachine Framing Transposer found.");
            return;
        }

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            transposer.m_TrackedObjectOffset = originalOffset + new Vector3(x, y, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        transposer.m_TrackedObjectOffset = originalOffset;
    }
}