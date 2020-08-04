using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float StepX = 0.2f;
    public float StepFall = 0.2f;

    private BridgeCreator bridgeCreator;
    private LevelCreator levelCreator;
    private PlayerStatus playerStatus;
    private BackgroundMovement backgroundMovement;
    private JustCloud cloud;

    enum PlayerResults { Fall, Nice, Great, Perfection }

    void Start()
    {
        levelCreator = GameObject.FindGameObjectWithTag("LevelCreator").GetComponent<LevelCreator>();
        playerStatus = Camera.main.GetComponent<PlayerStatus>();
        backgroundMovement = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundMovement>();
        cloud = GameObject.Find("CloudPlane").GetComponent<JustCloud>();
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public void MovePlayerTo(Vector3 posToGo, BridgeCreator bridgeCreator)
    {
        this.bridgeCreator = bridgeCreator;

        int result = GetPlayerResult(posToGo);
        //Если игрок не упал, то пусть идёт на платформу
        if (result != (int)PlayerResults.Fall) {
            playerStatus.OnResultGet(result);
            StartCoroutine(CorMovePlayerTo(levelCreator.GetCurrentColumn().transform.position));
        } else {
            if (Random.Range(1, 101) <= 4)
                AudioManager.instance.Play("NyVotMenyaUbut"); //Пасхалка
            StartCoroutine(FallPlayerFrom(posToGo));
        }
    }

    public int GetPlayerResult(Vector3 posToGo)
    {
        float posX = posToGo.x;
        float minAllowX = levelCreator.GetCurrentColumn().transform.position.x - levelCreator.GetCurrentColumnWidth() / 2;
        float maxAllowX = levelCreator.GetCurrentColumn().transform.position.x + levelCreator.GetCurrentColumnWidth() / 2;

        //Если мост не упал на колонну
        if ((posX < minAllowX) || (posX > maxAllowX))
            return (int)PlayerResults.Fall;

        float percentOfAim = Mathf.InverseLerp(minAllowX, maxAllowX, posX);

        //При ниже 20% или выше 80% выдаём игроку Nice 
        if ((percentOfAim <= 0.2f) || (1 - percentOfAim <= 0.2f))
            return (int)PlayerResults.Nice;
        //При ниже 40% или выше 60% выдаём игроку Great
        if ((percentOfAim <= 0.4f) || (1 - percentOfAim <= 0.4f))
            return (int)PlayerResults.Great;

        //Иначе Perfection
        return (int)PlayerResults.Perfection;
    }

    private bool IsBridgeOnLeft(Vector3 posToGo)
    {
        float posX = posToGo.x;
        float minAllowX = levelCreator.GetCurrentColumn().transform.position.x - levelCreator.GetCurrentColumnWidth() / 2;

        return posX < minAllowX;
    }

    private IEnumerator FallPlayerFrom(Vector3 posToGo)
    {
        //Пока не дошли до точки
        while (transform.position.x < posToGo.x) {
            transform.position += new Vector3(StepX, 0, 0);
            yield return null;
        }

        //Скидываем мост в пропасть
        bridgeCreator.OnPlayerLoose(IsBridgeOnLeft(posToGo));

        //Скидываем игрок в пропасть
        while (transform.position.y > -10) {
            transform.position -= new Vector3(0, StepFall, 0);
            yield return null;
        }

        playerStatus.KillPlayer();

        yield return null;
    }

    private IEnumerator CorMovePlayerTo(Vector3 posToGo)
    {
        float distanceToPass = posToGo.x - transform.position.x;

        //Пока не дошли до точки
        while (transform.position.x < posToGo.x) {
            transform.position += new Vector3(StepX, 0, 0);
            yield return null;
        }

        //Когда дошли, начинаем смещать уровень
        StartCoroutine(ReturnPositions(distanceToPass));

        yield return null;
    }

    private IEnumerator ReturnPositions(float distanceToPass)
    {
        //Создаём следующую колонну
        playerStatus.AddPoint();
        levelCreator.CreateNextColumn(distanceToPass);

        //Создаём иллюзию перемещения
        while (distanceToPass > 0) {
            backgroundMovement.MoveOnDistance(StepX);
            cloud.OnMovementStart();
            //Перемещаем предметы назад
            foreach (GameObject gObject in GameObject.FindGameObjectsWithTag("Movable"))
                //Если объект уехал далеко за карту, то удаляем его
                if (gObject.transform.position.x <= -8)
                    Destroy(gObject);
                else
                    gObject.transform.position -= new Vector3(StepX, 0, 0);
            GameObject.FindGameObjectWithTag("Player").transform.position -= new Vector3(StepX, 0, 0);
            distanceToPass -= StepX;
            yield return null;
        }

        OnAnimationsFinish();

        yield return null;
    }

    //Когда уровень сместился
    private void OnAnimationsFinish()
    {
        bridgeCreator.OnAnimationsFinish();
        cloud.OnMovementEnd();
    }
}
