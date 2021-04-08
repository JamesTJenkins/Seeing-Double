using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SDBinaryFormatter {

    public static void SaveData(GameData data, string path) {
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream s = new FileStream(path, FileMode.Create);

        formatter.Serialize(s, data);

        s.Close();
    }

    public static GameData LoadData() {
        string path = Application.persistentDataPath + "/player.sd";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream s = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(s) as GameData;

            s.Close();

            return data;
        } else {
            Debug.LogError("No Save File Found");
            return null;
        }
    }

}
