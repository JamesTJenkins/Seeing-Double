using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    public GameObject ui;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Cat")) {
            ui.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
