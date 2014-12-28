using UnityEngine;
using System.Collections;

public class SaveData
{
    public int winCount;
    public SaveData()
    {
        winCount = 0;
    }
}

public class SaveDataManager : MonoBehaviour
{
    public static SaveData data;
    public static void Save()
    {
        try
        {

            using (var sw = new System.IO.StreamWriter(Constants.filename, false, new System.Text.UTF8Encoding(false)))
            {
                var s = new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
                s.Serialize(sw, data);
            }
        }
        catch
        {
            UnityEngine.Debug.Log("Datasave failed!");
        }

    }

    public static void Load()
    {
        try
        {

            using (var sr = new System.IO.StreamReader(Constants.filename, new System.Text.UTF8Encoding(false)))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
                data = (SaveData)s.Deserialize(sr);
            }
        }
        catch
        {
            data = new SaveData();
            UnityEngine.Debug.Log("Dataload failed!");
        }

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
