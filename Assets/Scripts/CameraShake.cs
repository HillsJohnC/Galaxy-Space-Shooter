using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _elapsedTime = 0f;
    
    public IEnumerator Shake (float shakeTime, float power)
    {
        Vector3 beginPos = transform.localPosition;

        while (_elapsedTime < shakeTime)
        {
            float x = Random.Range(-.25f, .25f) * power;
            float y = Random.Range(-.25f, .25f) * power;

            transform.localPosition = new Vector3(x, y, beginPos.z);
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = beginPos;
        _elapsedTime = 0f;
    }    
}
