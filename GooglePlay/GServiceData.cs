using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class GServiceData
{
    public long points;
    public int highScore;
    public int money;
    public List<bool> openedPlayerModels;
    public List<bool> openedBackgrounds;

    public void SetUpLocalData()
    {
        points = GlobalVars.instance.GetPoints();
        highScore = GlobalVars.instance.GetHighScore();
        money = GlobalVars.instance.GetMoney();
        openedPlayerModels = GlobalVars.instance.GetListPlayers();
        openedBackgrounds = GlobalVars.instance.GetListBackgrounds();
    }

    public byte[] ToByteArray()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, this);

        return ms.ToArray();
    }

    public void SetUpByByteArray(byte[] arr)
    {
        if (arr.Length <= 0)
            return;

        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arr, 0, arr.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        GServiceData obj = (GServiceData) binForm.Deserialize(memStream);

        points = obj.points;
        highScore = obj.highScore;
        money = obj.money;
        openedPlayerModels = obj.openedPlayerModels;
        openedBackgrounds = obj.openedBackgrounds;
    }
}
