using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude) {
        float t = 0.0f;
        Vector3 oriPos = this.transform.localPosition;

        while (t < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            this.transform.localPosition = new Vector3(x, y, oriPos.z);

            t += Time.unscaledDeltaTime;

            yield return null;
        }

        this.transform.localPosition = oriPos;
    }
}
