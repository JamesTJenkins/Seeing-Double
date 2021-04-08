using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickup : MonoBehaviour
{
    public int id = 0;
    public TriggerUI tUI;

    public void Start() {
        if (!GlobalValues.remainingGems[id]) {
            tUI.disable = true;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat") {
            GlobalValues.gemsCollected += 1;
            tUI.useOnce = true;
            GlobalValues.remainingGems[id] = false;
            Destroy(this.gameObject);
        }
    }
}
