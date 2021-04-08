using System.Collections;
using UnityEngine;

public class SBTrigger : MonoBehaviour
{
    public SpiderBossAI sb;
    public GameObject arenaBlock;
    public GameObject arenaBlockRemoved;
    public Camera cam;
    public CatsMovement cm;
    public Vector2 targetPos;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !GlobalValues.sbDead) {
            arenaBlock.SetActive(true);
            arenaBlockRemoved.SetActive(false);
            StartCoroutine(LerpCamera());
            StartCoroutine(Delay());
        }
    }

    private IEnumerator LerpCamera() {
        cm.cameraMovementDisable = true;
        cam.orthographicSize = 5.5f;

        while(Mathf.Abs(cam.transform.parent.position.x - targetPos.x) > 0.1f && Mathf.Abs(cam.transform.parent.position.y - targetPos.y) > 0.1f) {
            cam.transform.parent.position = new Vector3(Mathf.Lerp(cam.transform.parent.position.x, targetPos.x, Time.unscaledDeltaTime * 3), Mathf.Lerp(cam.transform.parent.position.y, targetPos.y, Time.unscaledDeltaTime * 3), cam.transform.parent.position.z);

            yield return null;
        }
    }

    private IEnumerator Delay() {
        yield return new WaitForSecondsRealtime(1);
        sb.aiDisable = false;
        Destroy(this.gameObject);
    }
}
