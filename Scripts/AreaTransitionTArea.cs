using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class AreaTransitionTArea : MonoBehaviour
{
    public int scene;
    public Image UI;
    public CatsMovement cm;
    public MainMenu menu;
    public Vector3 wcPos;
    public Vector3 bcPos;

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
        
        // Modify Scene
        Scene s = SceneManager.GetSceneAt(1);

        // Gets controller and sets up along with cat positions
        GameObject[] objects = s.GetRootGameObjects();

        for (int i = 0; i < objects.Length; i++) {
            if (objects[i].TryGetComponent(out CatsMovement cm)) {
                cm.lockSwitching = false;

                if (cm.wc != null)
                    cm.wc.transform.position = wcPos;

                if (cm.bc != null)
                    cm.bc.transform.position = bcPos;

                break;
            }
        }

        GlobalValues.scene = scene;

        SceneManager.SetActiveScene(s);

        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(unloadIndex);

        while (!unloadScene.isDone) {
            yield return null;
        }
    }
}
