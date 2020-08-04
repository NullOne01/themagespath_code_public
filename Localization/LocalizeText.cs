using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeText : MonoBehaviour {
    [SerializeField]
    private string TextID = "error";
    [SerializeField]
    private bool RemoveScriptAfter = true;

    private LocalizationManager localizationManager;

    void Start() {
        localizationManager = LocalizationManager.instance;
        Localize();
    }

    void Update() {
        Localize();
    }

    private void Localize() {
        if (localizationManager.GetIsReady()) {
            TextMeshProUGUI textPro = GetComponent<TextMeshProUGUI>();
            if (textPro != null)
                textPro.text = localizationManager.GetLocalizedValue(TextID);

            Text text = GetComponent<Text>();
            if (text != null)
                text.text = localizationManager.GetLocalizedValue(TextID);

            if (RemoveScriptAfter)
                Destroy(this);
        }
    }
}
