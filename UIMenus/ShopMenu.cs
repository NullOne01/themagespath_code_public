using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public Canvas shopCanvas;
    public TextMeshProUGUI costLabel;
    public TextMeshProUGUI playerNameLabel;
    public TextMeshProUGUI buyButtonLabel;
    public Button buyButton;
    public Transform contentPlane;
    public TextMeshProUGUI moneyLabel;
    public GameObject playerChoosePrefab;
    public GameObject backgroundChoosePrefab;

    public Material blurMaterial;

    private const string KEY_UNKNOWN = "unknown";
    private const string STRING_HIRE = "Hire";
    private const string STRING_BUY = "Buy";
    private const string STRING_MOVE = "Move";
    private const string PREFIX_MONEY_LABEL = "<sprite index=0> ";

    private PlayerSpawner playerSpawner;
    private BackgroundMovement backgroundMovement;

    private bool isPlayerShop = true;
    private int currentPlayerID = 0;
    private int currentBackgroundID = 0;

    void Start() {
        playerSpawner = GameObject.FindGameObjectWithTag("PlayerCreator").GetComponent<PlayerSpawner>();
        backgroundMovement = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundMovement>();

        SetVisibility(false);
    }

    void Update() {
        if (shopCanvas.enabled)
            if (Input.GetKeyDown(KeyCode.Escape))
                OnReturnButton();
    }

    public void OnSwitchButton() {
        isPlayerShop = !isPlayerShop;
        SetPlayerBackgroundUI();
    }

    public void OnReturnButton() {
        SetVisibility(false);
        GetComponent<StartMenu>().SetVisibility(true);

        //Выбираем выбранного игрока и выбранный фон
        playerSpawner.SpawnPlayer(GlobalVars.instance.GetCurrentPlayer());
        backgroundMovement.SetBackground(GlobalVars.instance.GetCurrentBackground());
    }
    /// <summary>
    /// При выборе картинки выбора игрока снизу экрана
    /// </summary>
    /// <param name="playerID">ID игрока, на которого нажали</param>
    public void OnSelectPlayer(int playerID) {
        playerSpawner.SpawnPlayer(playerID);
        currentPlayerID = playerID;

        SetPlayerBackgroundUI();
    }

    /// <summary>
    /// При выборе картинки выбора заднего фона снизу экрана
    /// </summary>
    /// <param name="bgID">ID заднего фона, на который нажали</param>
    public void OnSelectBackground(int bgID) {
        if (IsBackgroundBought(bgID)) {
            backgroundMovement.SetBackground(bgID);
        }
        currentBackgroundID = bgID;

        SetPlayerBackgroundUI();
    }

    public void OnBuyButton() {
        if (isPlayerShop) {
            //Если мы можем нанять этого персонажа
            if (IsPlayerBought(currentPlayerID)) {
                GlobalVars.instance.SetCurrentPlayer(currentPlayerID); //То спокойно нанимаем его
                playerSpawner.SpawnPlayer(currentPlayerID);
                AudioManager.instance.PlayRandom("PutOn");
            } else { //Если мы собираемся её купить
                if (GlobalVars.instance.GetMoney() >= playerSpawner.GetCost(currentPlayerID)) { //Если хватает денег
                    GlobalVars.instance.SetPlayerEnabled(currentPlayerID); //Делаем этого персонажа доступным
                    GlobalVars.instance.RemoveMoney(playerSpawner.GetCost(currentPlayerID)); //Отбираем деньги за покупку
                    playerSpawner.SpawnPlayer(currentPlayerID);
                    AudioManager.instance.PlayRandom("Coin");
                }
            }
        } else {
            //Если мы ставим этот background
            if (IsBackgroundBought(currentBackgroundID)) {
                GlobalVars.instance.SetCurrentBackground(currentBackgroundID); //То спокойно ставим его
                AudioManager.instance.PlayRandom("PutOn");
            } else { //Если мы собираемся её купить
                if (GlobalVars.instance.GetMoney() >= backgroundMovement.GetCost(currentBackgroundID)) { //Если хватает денег
                    GlobalVars.instance.SetBackgroundEnabled(currentBackgroundID); //Делаем этого персонажа доступным
                    GlobalVars.instance.RemoveMoney(backgroundMovement.GetCost(currentBackgroundID)); //Отбираем деньги за покупку
                    AudioManager.instance.PlayRandom("Coin");
                }
            }
        }

        SetPlayerBackgroundUI();
    }

    private void SetPlayerBackgroundUI() {
        ClearChildren(contentPlane);
        SetCurrentMoneyLabel();
        //Если мы выбираем игрока, а не background
        if (isPlayerShop) {
            FillListPlayer();
            //Если эта модель игрока куплена
            if (IsPlayerBought(currentPlayerID)) {
                ChangeLabelButton(STRING_HIRE); //Заменяем label в кнопке на "Нанять" вместо "Купить"
                TurnCostText(false);              //Выключаем цену
                buyButton.interactable = (GlobalVars.instance.GetCurrentPlayer() != currentPlayerID); //Если модель уже нанята, то кнопка не активна
            } else { //Если эта модель игрока ещё не куплена
                ChangeLabelButton(STRING_BUY);
                SetUpCost(playerSpawner.GetCost(currentPlayerID));
            }
            SetPlayerNameLabel(currentPlayerID);
        } else {
            FillListBackground();
            //Если этот background куплен
            if (IsBackgroundBought(currentBackgroundID)) {
                ChangeLabelButton(STRING_MOVE); //Заменяем label в кнопке на "Переместиться" вместо "Купить"
                TurnCostText(false);              //Выключаем цену
                buyButton.interactable = (GlobalVars.instance.GetCurrentBackground() != currentBackgroundID); //Если background уже выбран, то кнопка не активна
            } else { //Если этот background ещё не куплен
                ChangeLabelButton(STRING_BUY);
                SetUpCost(backgroundMovement.GetCost(currentBackgroundID));
            }
            SetBackgroundNameLabel(currentBackgroundID);
        }
    }

    private void SetUpCost(int cost) {
        SetCostLabel(cost);

        TurnCostText(true);
        //Если денег не достаточно, то делаем надпись красной. Выключаем кнопку
        if (GlobalVars.instance.GetMoney() < cost) {
            costLabel.color = new Color(255, 69, 69); //#FF4545
            buyButton.interactable = false;
        } else {
            costLabel.color = new Color(255, 255, 255);
            buyButton.interactable = true;
        }
    }

    private void FillListPlayer() {
        for (int i = 0; i < playerSpawner.players.Count; i++) {
            GameObject newObject = Instantiate(playerChoosePrefab, contentPlane);
            if (!IsPlayerBought(i)) {
                newObject.GetComponent<Image>().sprite = playerSpawner.players[i].playerSprite;
                //newObject.GetComponent<Image>().color = new Color(0, 0, 0);
                newObject.GetComponent<Image>().material = blurMaterial;
            } else {
                newObject.GetComponent<Image>().sprite = playerSpawner.players[i].playerSprite;
            }
            newObject.GetComponent<Image>().enabled = true;

            //Нужно создавать новый instance переменной, чтобы передать её как параметр
            int _i = i;
            AddOnPointerDown(newObject, (e) => OnSelectPlayer(_i));

            newObject.GetComponent<Button>().enabled = true;
        }
    }

    private void FillListBackground() {
        for (int i = 0; i < backgroundMovement.backgrounds.Count; i++) {
            GameObject newObject = Instantiate(backgroundChoosePrefab, contentPlane);
            newObject.GetComponent<Image>().sprite = backgroundMovement.backgrounds[i].bgSprite;
            if (!IsBackgroundBought(i))
                newObject.GetComponent<Image>().material = blurMaterial;

            newObject.GetComponent<Image>().enabled = true;

            int _i = i;
            AddOnPointerDown(newObject, (e) => OnSelectBackground(_i));

            newObject.GetComponent<Button>().enabled = true;
        }
    }

    private void AddOnPointerDown(GameObject button, UnityEngine.Events.UnityAction<BaseEventData> myEvent) {
        EventTrigger trigger = button.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener(myEvent);
        trigger.triggers.Add(pointerDown);
    }

    private void SetCurrentMoneyLabel() {
        SetMoneyLabel(moneyLabel);
    }

    public void SetMoneyLabel(TextMeshProUGUI moneyLabel) {
        moneyLabel.text = PREFIX_MONEY_LABEL + GlobalVars.instance.GetMoney();
    }

    private void ClearChildren(Transform parent) {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    private void ChangeLabelButton(string textID) {
        buyButtonLabel.text = LocalizationManager.instance.GetLocalizedValue(textID);
    }

    private void TurnCostText(bool isEnabled) {
        costLabel.enabled = isEnabled;
    }

    private bool IsPlayerBought(int playerID) {
        return GlobalVars.instance.GetListPlayers()[playerID];
    }

    private bool IsBackgroundBought(int bgID) {
        return GlobalVars.instance.GetListBackgrounds()[bgID];
    }

    private void SetCostLabel(int cost) {
        costLabel.text = PREFIX_MONEY_LABEL + cost;
    }

    private void SetPlayerNameLabel(int playerID) {
        //Если игрок куплен, то заменяем название. Иначе неизвестно
        if (GlobalVars.instance.GetListPlayers()[playerID])
            playerNameLabel.text = playerSpawner.GetName(playerID);
        else
            playerNameLabel.text = GetUnknownName();
    }

    private void SetBackgroundNameLabel(int bgID) {
        //Если фон куплен, то заменяем название. Иначе неизвестно
        if (GlobalVars.instance.GetListBackgrounds()[bgID])
            playerNameLabel.text = backgroundMovement.GetName(bgID);
        else
            playerNameLabel.text = GetUnknownName();
    }

    /// <summary>
    /// Просто получаем текст "Неизвестно"
    /// </summary>
    /// <returns></returns>
    private string GetUnknownName() {
        return LocalizationManager.instance.GetLocalizedValue(KEY_UNKNOWN);
    }

    public void SetVisibility(bool isEnabled) {
        shopCanvas.enabled = isEnabled;
        if (isEnabled) {
            currentPlayerID = GlobalVars.instance.GetCurrentPlayer();
            currentBackgroundID = GlobalVars.instance.GetCurrentBackground();
            SetPlayerBackgroundUI();
        }
    }
}
