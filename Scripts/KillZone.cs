using UnityEngine;

public class KillZone : MonoBehaviour {
    public GameObject ui;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "Cat") {
            ui.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
