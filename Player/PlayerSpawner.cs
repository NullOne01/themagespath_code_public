using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public BridgeCreator bridgeCreator;

    public List<PlayerObject> players;
    private PlayerObject currentPlayer;

    void Start()
    {
        SpawnCurrentPlayer();
    }

    public void SpawnPlayer(int id)
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        currentPlayer = players[id];

        GameObject playerObject = InstantiatePlayer();

        if ((!GlobalVars.instance.GetListPlayers()[id]) && (id != GlobalVars.instance.GetCurrentPlayer()) )
            playerObject.GetComponent<PlayerParts>().TurnIntoBlur();

        bridgeCreator.SetUpPlayer(playerObject);
    }

    public void SpawnCurrentPlayer()
    {
        SpawnPlayer(GlobalVars.instance.GetCurrentPlayer());
    }

    public int GetCost(int playerID)
    {
        return players[playerID].cost;
    }

    public string GetName(int playerID)
    {
        return LocalizationManager.instance.GetLocalizedValue(players[playerID].playerName);
    }


    private GameObject InstantiatePlayer()
    {
        GameObject newPlayer = Instantiate(currentPlayer.playerPrefab, transform.position, transform.rotation);
        newPlayer.tag = "Player";
        newPlayer.AddComponent<PlayerMovement>().StepFall = currentPlayer.StepFall;
        newPlayer.GetComponent<PlayerMovement>().StepX = currentPlayer.StepX;
        return newPlayer;
    }
}
