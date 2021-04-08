using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreaTransition : MonoBehaviour
{
    public int scene;
    public Image UI;
    public CatsMovement cm;
    public MainMenu menu;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !triggered) {
            triggered = true;
            menu.SaveGame(cm);
            StartCoroutine(LoadNext());
        }
    }

    private IEnumerator LoadNext() {
        GlobalValues.newScene = scene;

        int unloadIndex = SceneManager.GetActiveScene().buildIndex;

        while (UI.color.a < 0.99) {
            UI.color = new Color(UI.color.r, UI.color.g, UI.color.b, Mathf.Lerp(UI.color.a, 1, Time.unscaledDeltaTime * 2));

            yield return null;
        }

        AsyncOperation loadScene = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!loadScene.isDone) {
            yield return null;
        }

        GlobalValues.scene = scene;

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(unloadIndex);

        while (!unloadScene.isDone) {
            yield return null;
        }
    }
}
