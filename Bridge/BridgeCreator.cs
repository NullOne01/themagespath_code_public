using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BridgeCreator : MonoBehaviour {
    public GameObject BridgePrefab;
    public TextMeshProUGUI ScoreLabel;

    [SerializeField]
    private float ValueStepOfCreating = 0.01f;
    [SerializeField]
    private float ValueStepOfResettingHand = 1f;
    [SerializeField]
    private float ValueStepOfDroppingBridge = 10f;
    [SerializeField]
    private float BridgeSpawnOffsetX = 0.1f;

    private bool IsCreating = false;
    public bool canCreateBridge = true;
    private Transform playerHand;

    private GameObject createdBridge;
    private PlayerMovement playerMovement;
    private LevelCreator levelCreator;

    void Start()
    {
        levelCreator = GameObject.FindGameObjectWithTag("LevelCreator").GetComponent<LevelCreator>();
        SetBridgeOnEdge();
    }

    public void SetUpPlayer(GameObject player)
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerHand = player.GetComponent<PlayerParts>().GetHand();
    }

    private IEnumerator BridgeCreatingCorountine()
    {
        createdBridge = CreateBridge();

        //Пока создаём мост
        while (IsCreating)
        {
            createdBridge.transform.localScale += new Vector3(0, ValueStepOfCreating, 0);
            //Позиция верхушки моста
            Vector3 posTopOfBridge = transform.position + new Vector3(0, SpriteUtilities.GetSpriteHeight(createdBridge), 0);
            //Поворачиваем руку мага к мосту
            TurnHandToBridge(posTopOfBridge);

            yield return null;
        }

        yield return null;
    }

    public void OnTouch()
    {
        if (!canCreateBridge)
            return;
        if (!GlobalVars.instance.HasGameStarted())
            return;

        IsCreating = true;
        StartCoroutine(BridgeCreatingCorountine());
        StopCoroutine(ResetHand());
    }

    public void OnRelease()
    {
        IsCreating = false;
        //Если мы не можем создавать мосты или у нас просто нет моста
        if ((!canCreateBridge) || (createdBridge == null))
            return;
        if (!GlobalVars.instance.HasGameStarted())
            return;

        //Возвращаем руку
        StopCoroutine(BridgeCreatingCorountine());

        //Пока маг идёт по мосту
        canCreateBridge = false;

        //Возвращаем руку
        StartCoroutine(ResetHand());

        //"Роняем" мост
        StartCoroutine(DropBridge());
    }

    //Когда игрок прошёл по мосту
    public void OnAnimationsFinish()
    {
        //Переставляем мост на край колонны
        SetBridgeOnEdge();

        createdBridge = null;
        canCreateBridge = true;
    }

    public void OnPlayerLoose(bool IsBridgeOnLeft)
    {
        //Если мост ещё не дорос до колонны, то он должен упасть
        if (IsBridgeOnLeft)
            StartCoroutine(FallBridge());
        else
            createdBridge = null;
    }

    private IEnumerator FallBridge()
    {
        //Анимация падения моста в реку (когда игрок проиграл)
        while (Mathf.Abs(createdBridge.transform.eulerAngles.z - 180) > 1)
        {
            createdBridge.transform.eulerAngles += new Vector3(0, 0, -ValueStepOfDroppingBridge);
            yield return new WaitForEndOfFrame();
        }

        createdBridge = null;
        yield return null;
    }

    private IEnumerator DropBridge()
    {
        while (Mathf.Abs(createdBridge.transform.eulerAngles.z - 270) > 1)
        {
            createdBridge.transform.eulerAngles += new Vector3(0, 0, -ValueStepOfDroppingBridge);
            yield return new WaitForEndOfFrame();
        }

        if (playerMovement.GetPlayerResult(transform.position + new Vector3(SpriteUtilities.GetSpriteHeight(createdBridge), 0, 0)) != 0)
            AudioManager.instance.Play("BridgeFall1");
        //Заставляем игрок идти на ту точку
        playerMovement.MovePlayerTo(transform.position + new Vector3(SpriteUtilities.GetSpriteHeight(createdBridge), 0, 0), this);
        yield return null;
    }

    private void TurnHandToBridge(Vector3 vector3)
    {
        float horizontalLength = vector3.x - playerHand.position.x;
        float verticalLength = vector3.y - playerHand.position.y;
        //Находим гипотенузу
        float hypoLength = Mathf.Sqrt(Mathf.Pow(horizontalLength, 2) + Mathf.Pow(verticalLength, 2));

        Vector3 angleLookAt = new Vector3(0, 0, Mathf.Max(0, Mathf.Rad2Deg * Mathf.Asin(verticalLength / hypoLength)));
        playerHand.eulerAngles = angleLookAt;
    }

    private IEnumerator ResetHand()
    {
        while (Mathf.Abs(playerHand.eulerAngles.z) >= ValueStepOfResettingHand)
        {
            float currentRotZ = playerHand.eulerAngles.z;
            Vector3 newRotation;
            if (currentRotZ < 180)
                newRotation = new Vector3(0, 0, currentRotZ - ValueStepOfResettingHand);
            else
                newRotation = new Vector3(0, 0, currentRotZ + ValueStepOfResettingHand);
            playerHand.eulerAngles = newRotation;
            yield return null;
        }

        yield return null;
    }

    private void SetBridgeOnEdge()
    {
        float newX = levelCreator.GetPrevColumn().transform.position.x + levelCreator.GetPrevColumnWidth() / 2f - SpriteUtilities.GetSpriteWidth(BridgePrefab) / 2f - BridgeSpawnOffsetX;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private GameObject CreateBridge()
    {
        GameObject newBridge = Instantiate(BridgePrefab, transform.position, Quaternion.identity, levelCreator.transform);
        Vector3 newScale = new Vector3(newBridge.transform.localScale.x, 0.1f, newBridge.transform.localScale.z);
        newBridge.transform.localScale = newScale;
        return newBridge;
    }
}
