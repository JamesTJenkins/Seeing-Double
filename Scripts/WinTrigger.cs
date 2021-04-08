using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour {

    public CatsMovement cm;
    public Transform cloudObj;
    public GameObject credits;
    public MainMenu menu;
    public float height;
    public float speed = 2f;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Cat")) {
            if (collision.gameObject == cm.wc.gameObject) {
                cm.wc.velocity = Vector3.zero;
                cm.lockMovement = true;
                cm.lockSwitching = true;
                cm.pauseMenuDisable = true;
                GlobalValues.isWhiteCat = true;
                cm.wc.transform.parent = cloudObj;
                credits.SetActive(true);

                StartCoroutine(Credits());
            }
        }
    }

    private IEnumerator Credits() {
        while (height > cloudObj.position.y) {
            cloudObj.Translate(Vector3.up * speed * Time.deltaTime);

            yield return null;
        }

        float t = 0f;
        cm.cameraMovementDisable = true;

        while (t < 5f) {
            t += Time.deltaTime;
            cloudObj.Translate(Vector3.up * speed * Time.deltaTime);

            yield return null;
        }

        menu.ReturnToMainMenu(0);
    }
}
