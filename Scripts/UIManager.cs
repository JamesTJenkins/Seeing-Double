using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    
    public Text floorWeakUI;
    public Text SwitchButton;
    public Text gemText;

    bool uiElementActive = false;

    public void CallFromInt(int call, bool value) {
        switch (call) {
            case 0:
                FloorWeakTextActive(value);
                break;
            case 1:
                SwitchButtonTextActive(value);
                break;
            case 2:
                GemTextActive(value);
                break;
        }
    }

    public void FloorWeakTextActive(bool value) {
        // Checks current ui elements and checks if this one is being enabled
        if (uiElementActive && value)
            DisableAllUI();
        
        floorWeakUI.gameObject.SetActive(value);

        uiElementActive = value;
    }

    public void SwitchButtonTextActive(bool value) {
        if (uiElementActive && value)
            DisableAllUI();

        SwitchButton.gameObject.SetActive(value);

        uiElementActive = value;
    }

    public void GemTextActive(bool value) {
        // Should make it display this correctly
        gemText.text = "Total Gems: " + (GlobalValues.gemsCollected + 1);

        gemText.gameObject.SetActive(value);
    }

    public void DisableAllUI() {
        floorWeakUI.gameObject.SetActive(false);
        SwitchButton.gameObject.SetActive(false);
    }

    public IEnumerator Delay(int call) {
        yield return new WaitForSecondsRealtime(3);
        CallFromInt(call, false);
    }
}
