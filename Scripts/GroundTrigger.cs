using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    public CatsMovement cm;
    public bool forWhiteCat = true;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag != "Cat") {
            if (forWhiteCat) {
                cm.wcGrounded = true;
            } else {
                cm.bcGrounded = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag != "Cat") {
            if (forWhiteCat) {
                cm.wcGrounded = false;
            } else {
                cm.bcGrounded = false;
            }
        }
    }
}
