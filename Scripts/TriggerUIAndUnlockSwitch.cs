using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TriggerUIAndUnlockSwitch : MonoBehaviour
{
    public UIManager uiManager;
    public CatsMovement cm;

    private bool stopDoubleTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !stopDoubleTrigger) {
            uiManager.SwitchButtonTextActive(true);
            stopDoubleTrigger = true;
            cm.lockSwitching = false;
            StartCoroutine(UIUpdate());
        }
    }

    private IEnumerator UIUpdate() {
        StartCoroutine(uiManager.Delay(1));

        yield return new WaitForSecondsRealtime(4);

        Destroy(this.gameObject);
    }
}
