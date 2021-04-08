using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TriggerUI : MonoBehaviour
{
    public UIManager uiManager;
    public int call = 0;
    public bool disable = false;
    public bool useOnce = false;

    private bool stopDoubleTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !stopDoubleTrigger && !disable) {
            if (GlobalValues.isWhiteCat && collision.gameObject.name == "whiteCat") {
                uiManager.CallFromInt(call, true);
                stopDoubleTrigger = true;
                StartCoroutine(UIUpdate());
            } else if (!GlobalValues.isWhiteCat && collision.gameObject.name == "blackCat") {
                uiManager.CallFromInt(call, true);
                stopDoubleTrigger = true;
                StartCoroutine(UIUpdate());
            }
        }
    }

    private IEnumerator UIUpdate() {
        StartCoroutine(uiManager.Delay(call));

        yield return new WaitForSecondsRealtime(4);

        if (useOnce)
            this.gameObject.SetActive(false);

        stopDoubleTrigger = false;
    }
}
