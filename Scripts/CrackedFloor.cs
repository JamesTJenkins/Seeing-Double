using UnityEngine;

public class CrackedFloor : MonoBehaviour
{
    public GameObject target;
    public TriggerUI tUI;
    public UIManager uiManager;

    private bool catOnFloor = false;

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Cat") {
            if (catOnFloor) {
                uiManager.FloorWeakTextActive(false);
                tUI.disable = true;
                target.SetActive(false);
            } else {
                catOnFloor = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Cat") {
            catOnFloor = false;
        }
    }
}
