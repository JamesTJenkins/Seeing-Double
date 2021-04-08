using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReptillianTrigger : MonoBehaviour {

    public ReptillianAI rep;
    public Camera cam;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !GlobalValues.rDead) {
            cam.orthographicSize = 8f;
            rep.aiDisable = false;
            Destroy(this);
        }
    }
}
