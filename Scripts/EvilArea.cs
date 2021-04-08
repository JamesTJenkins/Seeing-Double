using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilArea : MonoBehaviour
{
    public CatsMovement cm;

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject == cm.wc.gameObject) {
            cm.wc.velocity = new Vector2(0f, cm.wc.velocity.y);

            if (cm.wc.transform.position.x > this.transform.position.x) {
                cm.FearWhiteCat(false);
            } else {
                cm.FearWhiteCat(true);
            }
        }
    }
}
