using UnityEngine;

public class Altar : MonoBehaviour
{
    public CatsMovement cm;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Cat") {
            cm.byAltar = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Cat") {
            cm.byAltar = false;
        }
    }
}
