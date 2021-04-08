using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData {

    // Player Location Data

    public int scene;
    public bool isWhiteCat;
    public bool canSwitch;
    public float[] whiteCatLoc;
    public float[] blackCatLoc;

    // Bosses

    public bool sbDead;
    public bool wdDead;
    public bool tbDead;
    public bool rDead;

    // Abilities

    public bool canCloudSpit;
    public bool canFire;

    // Gems

    public int gemCount;
    public bool[] gems;

    // Alter

    public bool[] altarOptions;
    public int altarIndex;

    // Danglers

    public bool[] danglers;

    // Totems

    public bool[] totems;

    // Clouds

    public int cloudCount;

    public GameData(CatsMovement cm) {
        scene = SceneManager.GetActiveScene().buildIndex;
        isWhiteCat = GlobalValues.isWhiteCat;
        canCloudSpit = GlobalValues.canCloudSpit;
        canFire = GlobalValues.canFire;
        gemCount = GlobalValues.gemsCollected;
        gems = GlobalValues.remainingGems;
        altarOptions = GlobalValues.altarAvailability;
        altarIndex = GlobalValues.altarIndex;
        canSwitch = cm.lockSwitching;
        danglers = GlobalValues.danglers;
        totems = GlobalValues.totems;
        cloudCount = GlobalValues.cloudCount;

        sbDead = GlobalValues.sbDead;
        wdDead = GlobalValues.wdDead;
        tbDead = GlobalValues.tbDead;
        rDead = GlobalValues.rDead;

        if (cm.wc != null) {
            float[] newVec = new float[3];

            newVec[0] = cm.wc.transform.position.x;
            newVec[1] = cm.wc.transform.position.y;
            newVec[2] = cm.wc.transform.position.z; 
            
            whiteCatLoc = newVec;
        }

        if (cm.bc != null) {
            float[] newVec = new float[3];

            newVec[0] = cm.bc.transform.position.x;
            newVec[1] = cm.bc.transform.position.y;
            newVec[2] = cm.bc.transform.position.z;

            blackCatLoc = newVec;
        }
    }
}
