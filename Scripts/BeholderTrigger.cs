using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeholderTrigger : MonoBehaviour {

    public BeholderAI bai;
    public GameObject arenaBlock;
    public GameObject arenaBlockRemoved;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat") {
            arenaBlock.SetActive(true);
            arenaBlockRemoved.SetActive(false);
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay() {
        yield return new WaitForSecondsRealtime(1);
        bai.aiDisable = false;
        Destroy(this.gameObject);
    }
}
