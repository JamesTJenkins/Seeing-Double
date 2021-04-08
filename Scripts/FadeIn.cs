using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public int currentScene = 0;
    public Image UI;
    public Text locationText;

    void Start() {
        UI.color = new Color(UI.color.r, UI.color.g, UI.color.b, 1);
        locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, 0);
        StartCoroutine(Fade());
        StartCoroutine(LocationText());
    }

    private IEnumerator Fade() {
        while (UI.color.a > 0.01) {
            UI.color = new Color(UI.color.r, UI.color.g, UI.color.b, Mathf.Lerp(UI.color.a, 0, Time.unscaledDeltaTime / 2));

            if (GlobalValues.newScene != currentScene)
                break;

            yield return null;
        }

        if (GlobalValues.newScene != currentScene)
            UI.color = new Color(UI.color.r, UI.color.g, UI.color.b, 0);
    }

    private IEnumerator LocationText() {
        if (GlobalValues.newScene != currentScene)
            yield return new WaitForSeconds(1);

        while (locationText.color.a < 0.99) {
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, Mathf.Lerp(locationText.color.a, 1, Time.unscaledDeltaTime * 2));

            if (GlobalValues.newScene != currentScene)
                break;

            yield return null;
        }

        locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, 1);

        if (GlobalValues.newScene != currentScene)
            yield return new WaitForSeconds(1);

        while (locationText.color.a > 0.01) {
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, Mathf.Lerp(locationText.color.a, 0, Time.unscaledDeltaTime * 2));

            if (GlobalValues.newScene != currentScene)
                break;

            yield return null;
        }

        locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, 0);
    }
}
