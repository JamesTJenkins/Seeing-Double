using UnityEngine;

public class WebDagger : MonoBehaviour {

    public GameObject ui;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag != "Cat") {
            Destroy(this.gameObject);
        } else {
            ui.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
