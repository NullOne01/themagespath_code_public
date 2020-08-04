using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {
    public static LocalizationManager instance;

    private const string localFileEn = "local_en"; //.json
    private const string localFileRu = "local_ru";

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        SetLocalization();
        DontDestroyOnLoad(gameObject);
    }

    private void SetLocalization() {
        switch (Application.systemLanguage) {
            case SystemLanguage.Russian: LoadLocalizedText(localFileRu); break;
            case SystemLanguage.English: LoadLocalizedText(localFileEn); break;
            default: LoadLocalizedText(localFileEn); break;
        }
    }

    public void LoadLocalizedText(string fileName) {
        localizedText = new Dictionary<string, string>();
        TextAsset textFile = Resources.Load<TextAsset>("Text/" + fileName);

        if (textFile.text.Length > 0) {
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(textFile.text);

            for (int i = 0; i < loadedData.items.Length; i++) {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        } else {
            Debug.LogError("Cannot find file!");
        }

        isReady = true;
    }

    public string GetLocalizedValue(string key) {
        string result = missingTextString;
        if (localizedText.ContainsKey(key)) {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsReady() {
        return isReady;
    }

}