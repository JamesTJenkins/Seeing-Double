using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class CloudCinematic : MonoBehaviour
{
    [Header ("Settings")]
    public CatsMovement cm;
    public AudioSource source;
    public Transform largeEvilA;
    public Transform largeEvilB;
    public float targetHeight;
    public float risingSpeed = 2f;
    public GameObject env;
    public Image UI;
    public ParticleSystem ps;

    [Header ("Camera Shake")]
    public CameraShake cs;
    public float duration = 4f;
    public float magnitude = 2f;

    private bool noLoadPls = false;
    private AsyncOperation load = null;

    private void Start() {
        source.mute = true;
        source.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Cat" && !noLoadPls) {
            cm.DisablePauseMenu();
            cm.lockMovement = true;
            cm.UpdateWhiteCatAnimation(1, true, false);
            StartCoroutine(DelayCamMovementDisable());
            StartCoroutine(cs.Shake(duration, magnitude));
            StartCoroutine(Fade());
            LargeEvilMovement();
            noLoadPls = true;
        }
    }

    private void LargeEvilMovement() {
        GlobalValues.newScene = 2;

        load = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        load.allowSceneActivation = false;

        load.completed += CloudCinematic_completed;
    }

    private void CloudCinematic_completed(AsyncOperation obj) {
        GlobalValues.scene = 2;

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

        SceneManager.UnloadSceneAsync(1);
    }

    private IEnumerator Fade() {
        ps.Play();
        source.mute = false;
        source.Play();

        while ((Mathf.Abs(largeEvilA.position.y) - Mathf.Abs(targetHeight)) > 0.2) {
            largeEvilA.transform.position = new Vector3(largeEvilA.position.x, Mathf.Lerp(largeEvilA.position.y, targetHeight, Time.unscaledDeltaTime * risingSpeed), largeEvilA.position.z);
            largeEvilB.transform.position = new Vector3(largeEvilB.position.x, Mathf.Lerp(largeEvilB.position.y, targetHeight, Time.unscaledDeltaTime * risingSpeed), largeEvilB.position.z);

            yield return null;
        }

        source.Stop();
        env.SetActive(false);

        while (UI.color.a < 0.99) {
            UI.color = new Color(UI.color.r, UI.color.g, UI.color.b, Mathf.Lerp(UI.color.a, 1, Time.unscaledDeltaTime));

            yield return null;
        }

        yield return new WaitForSeconds(1);

        load.allowSceneActivation = true;
    }

    private IEnumerator DelayCamMovementDisable() {
        yield return new WaitForSeconds(2);

        cm.cameraMovementDisable = true;
    }
}
