using System.Collections;
using UnityEngine;

public class DanglerTrigger : MonoBehaviour
{
    public int id = 0;
    public Animator dangler;
    public GameObject lightGO;
    public GameObject toBeRemoved;
    public GameObject toBePlaced;

    private void Start() {
        if (GlobalValues.danglers[id]) {
            dangler.SetBool("triggered", true);
            toBeRemoved.SetActive(false);
            toBePlaced.SetActive(true);
            StartCoroutine(Lerping());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cloud") {
            GlobalValues.danglers[id] = true;
            dangler.SetBool("triggered", true);
            toBeRemoved.SetActive(false);
            toBePlaced.SetActive(true);
            StartCoroutine(Lerping());
        }
    }

    private IEnumerator Lerping() {
        Vector2 v = new Vector2(lightGO.transform.position.x, lightGO.transform.position.y + 0.5f);

        while (Vector2.Distance(lightGO.transform.position, v) < 0.05f) {
            Mathf.Lerp(lightGO.transform.position.y, v.y, Time.deltaTime);

            yield return null;
        }
    }
}
