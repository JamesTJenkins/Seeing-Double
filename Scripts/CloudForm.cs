using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudForm : MonoBehaviour {

    public GameObject[] clouds;
    public GameObject cloudCollider;
    public int cloudRegained = 0;
    public int maxClouds = 0;

    private void Start() {
        cloudRegained = GlobalValues.cloudCount;
        maxClouds = clouds.Length;
        for (int i = 0; i < cloudRegained; i++) {
            clouds[i].SetActive(true);
        }

        if (cloudRegained >= maxClouds)
            cloudCollider.SetActive(true);
    }

    public void AddCloud() {
        clouds[cloudRegained].SetActive(true);
        cloudRegained++;
        GlobalValues.cloudCount = cloudRegained;

        if (cloudRegained == maxClouds)
            cloudCollider.SetActive(true);
    }
}
