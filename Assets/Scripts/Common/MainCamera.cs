using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;

    private void Start()
    {
        startPosition = this.transform.position;
    }
    public void PlayerDamage() { StartCoroutine(CameraShake(1.0f, 0.75f, 0.2f)); }

    private IEnumerator CameraShake(float shakeDuration, float shakeMagnatude, float interpolationIncrement)
    {
        Vector3 startPosition = transform.position;
        float shakeElapsed = 0f;

        while (shakeElapsed < shakeDuration)
        {
            float shakeX = Random.Range(-1f, 1f) * shakeMagnatude;
            float shakeY = Random.Range(-1f, 1f) * shakeMagnatude;
            transform.position = Vector3.Slerp( new Vector3(startPosition.x, startPosition.y, startPosition.z)
                                              , new Vector3(startPosition.x + shakeX, startPosition.y + shakeY, startPosition.z)
                                              , interpolationIncrement
                                              );
            shakeElapsed += Time.deltaTime;
            yield return null;
        }
        this.transform.position = startPosition;
    }
}
