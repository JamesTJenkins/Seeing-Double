using System.Collections;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public bool loadInProgress = false;

    // Main Menu

    public void NewGame(int scene) {
        if (loadInProgress)
            return;

        Time.timeScale = 1f;

        // Holds all gems
        GlobalValues.remainingGems = new bool[] {
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true //38 at mo
        };

        // Altar options
        GlobalValues.altarAvailability = new bool[] {
            true,
            true,
            true
        };

        // Danglers
        GlobalValues.danglers = new bool[] {
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        // Totems
        GlobalValues.totems = new bool[] {
            false,
            false,
            false,
            false,
            false,
            false
        };

        // Reset general values
        GlobalValues.scene = 0;
        GlobalValues.newScene = 0;
        GlobalValues.isWhiteCat = true;
        GlobalValues.gemsCollected = 0;
        GlobalValues.cloudCount = 0;
        // Abilities and altar reset
        GlobalValues.altarIndex = 0;
        GlobalValues.canCloudSpit = false;
        GlobalValues.canFire = false;
        // Bosses reset
        GlobalValues.sbDead = false;
        GlobalValues.wdDead = false;
        GlobalValues.tbDead = false;
        GlobalValues.rDead = false;

        StartCoroutine(LoadScene(scene));
    }

    public void LoadGame() {
        if (loadInProgress)
            return;

        Time.timeScale = 1f;

        loadInProgress = true;

        GameData data = SDBinaryFormatter.LoadData();

        if (data == null) {
            loadInProgress = false;
            return;
        }

        // Set Global
        GlobalValues.isWhiteCat = data.isWhiteCat;
        GlobalValues.canCloudSpit = data.canCloudSpit;
        GlobalValues.canFire = data.canFire;
        GlobalValues.gemsCollected = data.gemCount;
        GlobalValues.cloudCount = data.cloudCount;
        GlobalValues.remainingGems = data.gems;
        GlobalValues.altarAvailability = data.altarOptions;
        GlobalValues.altarIndex = data.altarIndex;
        GlobalValues.danglers = data.danglers;
        GlobalValues.totems = data.totems;
        GlobalValues.sbDead = data.sbDead;
        GlobalValues.wdDead = data.wdDead;
        GlobalValues.tbDead = data.tbDead;
        GlobalValues.rDead = data.rDead;

        StartCoroutine(LoadSceneWithData(data));
    }

    public void Quit() {
        if (loadInProgress)
            return;

        Application.Quit();
    }

    // Pause Menu

    public void SaveGame(CatsMovement cm) {
        if (loadInProgress)
            return;

        loadInProgress = true;

        GameData data = new GameData(cm);

        string path = Application.persistentDataPath + "/player.sd";

        _ = SaveAsync(data, path);
    }

    public void ReturnToMainMenu(int scene) {
        StartCoroutine(LoadScene(scene));
    }

    // Helper Functions

    private async Task SaveAsync(GameData data, string path) {
        await Task.Run(() => SDBinaryFormatter.SaveData(data, path));

        loadInProgress = false;
    }

    private IEnumerator LoadScene(int scene) {
        GlobalValues.newScene = scene;

        AsyncOperation loadScene = SceneManager.LoadSceneAsync(scene);

        while (!loadScene.isDone) {
            yield return null;
        }

        GlobalValues.scene = scene;
    }

    private IEnumerator LoadSceneWithData(GameData data) {
        GlobalValues.newScene = data.scene;

        // Loads Scene
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(data.scene, LoadSceneMode.Additive);

        while (!loadScene.isDone) {
            yield return null;
        }

        // Modify Scene
        Scene s = SceneManager.GetSceneAt(1);

        // Gets controller and sets up along with cat positions
        GameObject[] objects = s.GetRootGameObjects();

        for (int i = 0; i < objects.Length; i++) {
            if (objects[i].TryGetComponent(out CatsMovement cm)) {
                cm.lockSwitching = data.canSwitch;

                if (cm.wc != null)
                    cm.wc.transform.position = FloatToVec3(data.whiteCatLoc);

                if (cm.bc != null)
                    cm.bc.transform.position = FloatToVec3(data.blackCatLoc);

                break;
            }
        }

        GlobalValues.scene = data.scene;

        SceneManager.SetActiveScene(s);

        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(0);
    }

    private Vector3 FloatToVec3(float[] data) {
        return new Vector3(data[0], data[1], data[2]);
    }
}
