using System.Collections;
using UnityEngine;

public class WDTrigger : MonoBehaviour {

    public WingedDemonAI wd;
    public Camera cam;
    public CatsMovement cm;
    public Vector2 targetPos;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat") {
            StartCoroutine(LerpCamera());
            StartCoroutine(Delay());
        }
    }

    private IEnumerator LerpCamera() {
        cm.cameraMovementDisable = true;
        cam.orthographicSize = 7.5f;

        while (Mathf.Abs(cam.transform.parent.position.x - targetPos.x) > 0.1f && Mathf.Abs(cam.transform.parent.position.y - targetPos.y) > 0.1f) {
            cam.transform.parent.position = new Vector3(Mathf.Lerp(cam.transform.parent.position.x, targetPos.x, Time.unscaledDeltaTime * 3), Mathf.Lerp(cam.transform.parent.position.y, targetPos.y, Time.unscaledDeltaTime * 3), cam.transform.parent.position.z);

            yield return null;
        }
    }

    private IEnumerator Delay() {
        yield return new WaitForSecondsRealtime(1);
        wd.aiDisable = false;
        this.enabled = false;
    }
}
