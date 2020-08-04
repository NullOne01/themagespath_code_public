using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GServiceSaver
{
    public static GServiceData gServiceData;
    private const string STRING_FILE_NAME = "PlayerProgress";
    private bool isLoading;


    public void MergeCloudAndLocal(GServiceData serviceData)
    {
        if (serviceData == null)
            Debug.LogError("GServiceData is null");

        //Впервые запускаем игру?
        if (GlobalVars.instance.GetIsFirstTime()) {
            GlobalVars.instance.RemoveIsFirstTime();
            //Если заходим впервые, то можем без проблем загрузить облачное сохранение в локальное пространство. Дальше этого делать не нужно
            if (serviceData.points > GlobalVars.instance.GetPoints()) {
                GlobalVars.instance.SetUpServiceData(serviceData);
                Debug.Log("Successfully merged with cloud data!");
            }
        } else {
            if (GlobalVars.instance.GetPoints() >= serviceData.points) {
                serviceData.SetUpLocalData();
                Debug.Log("Successfully merged with local data!");
            }
        }
    }

    public void LoadData()
    {
        isLoading = true;
        OpenSavedGame(STRING_FILE_NAME);
    }

    public void SaveData()
    {
        isLoading = false;
        OpenSavedGame(STRING_FILE_NAME);
    }

    public void OpenSavedGame(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood, OnSavedGameOpened);
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success) {
            if (isLoading) {
                LoadGameData(game);
            } else {
                MergeCloudAndLocal(gServiceData);
                SaveGame(game, gServiceData.ToByteArray());
            }
        } else {
            Debug.LogError("Failed to open saved game");
        }
    }

    void SaveGame(ISavedGameMetadata game, byte[] savedData)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate updatedMetadata = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedDescription("Saved game at " + DateTime.Now)
            .Build();

        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success) {
            Debug.Log("Game has been saved");
        } else {
            Debug.LogError("Failed to save game");
        }
    }

    void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success) {
            if (gServiceData == null)
                gServiceData = new GServiceData();
            gServiceData.SetUpByByteArray(data);
            MergeCloudAndLocal(gServiceData);
            Debug.Log("Success: saved game is loaded!");
        } else {
            Debug.LogError("Failed to read game data");
        }
    }
}
