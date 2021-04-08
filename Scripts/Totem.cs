using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour {

    public int id = 0;
    public Animator animator;
    public GameObject gem;
    public GameObject tbr;
    public GameObject tbp;

    private void Start() {
        if (GlobalValues.totems[id]) {
            animator.SetBool("Settled", true);

            if(tbr != null)
                tbr.SetActive(false);

            if (tbp != null)
                tbp.SetActive(true);

            gem.SetActive(true);

            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "BCFireball") {
            animator.SetBool("Settled", true);
            GlobalValues.totems[id] = true;

            if (tbr != null)
                tbr.SetActive(false);

            if (tbp != null)
                tbp.SetActive(true);

            gem.SetActive(true);
            Destroy(this);
        }
    }
}
