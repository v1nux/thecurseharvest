using UnityEngine;
using System.Collections;

public class CameraFollowShake : MonoBehaviour
{
    public static CameraFollowShake Instance;

    private Vector3 originalLocalPos;

    void Awake()
    {
        Instance = this;
        originalLocalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.15f, float strength = 0.15f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            transform.localPosition = originalLocalPos + new Vector3(x, y, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
    }
}