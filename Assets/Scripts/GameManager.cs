﻿// ----------------------------------------------------------------------
//   DEBATE - Ludum Dare 31 Compo Entry
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    Dictionary<int, LonerController> players;

    AudioSource noise;
    public float noiseVolume;
    float targetVolume;

    void Awake()
    {
        players = new Dictionary<int, LonerController>();
        noise = GetComponent<AudioSource>();

        GameEventManager.GameMenu += GameMenu;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
    }

    void Start()
    {
        GameEventManager.TriggerGameMenu();
    }

    void Update()
    {
        if (!Network.isServer)
        {
            switch(GameEventManager.CurrentState)
            {
                case GameEventManager.GameState.InMenu:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        GameEventManager.TriggerGameQuit();
                    }
                    break;

                case GameEventManager.GameState.Running:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Network.Disconnect();

                        GameEventManager.TriggerGameOver();
                    }
                    break;
                case GameEventManager.GameState.Over:
                    if (!Network.isClient && 1.0f - GUIManager.currentAlphaOver < 0.01f && Input.GetKeyDown(KeyCode.Space))
                    {
                        GameEventManager.TriggerGameMenu();
                    }
                    
                    break;
            }

            if(GameEventManager.CurrentState != GameEventManager.GameState.Running)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0.0f, 1.0f, -5.0f), 2.0f * Time.deltaTime);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 15.0f, 3.0f * Time.deltaTime);
            }

            if (noise.volume != targetVolume)
                noise.volume = Mathf.Lerp(noise.volume, targetVolume, Time.deltaTime);
        }
    }

    void OnDestroy()
    {
        GameEventManager.GameMenu -= GameMenu;
        GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
    }

    void GameMenu()
    {
        targetVolume = 0.0f;
    }

    void GameStart()
    {
        targetVolume = noiseVolume;
    }

    void GameOver()
    {
        LonerController[] players = FindObjectsOfType<LonerController>();
        foreach (LonerController loner in players)
            Destroy(loner.gameObject);

        LogManager.Logs.Clear();
    }

    Vector3 getPlayerSpawnPosition()
    {
        return new Vector3(Mathf.Min(15.0f, Mathf.Max(-15.0f, Random.Range(-5.0f * NetworkManager.PlayerCount, 5.0f * NetworkManager.PlayerCount))), 2.0f, 0.0f);
    }

    Color getSkinColor()
    {
        Color skinColor = Color.white;
        //float factor = Random.Range(-3.0f, 3.0f);

        skinColor.r = Random.Range(0.5f, 1.0f);     //(224.3f + 9.6f * factor) / 255.0f;
        skinColor.g = Random.Range(0.5f, 0.8f);     //(193.1f + 17.0f * factor) / 255.0f;
        skinColor.b = Random.Range(0.25f, 0.75f);   //(177.6f + 21.0f * factor) / 255.0f;

        return skinColor;
    }

    public void SpawnPlayer(int playerID)
    {
        players[playerID] = ((GameObject)Network.Instantiate(playerPrefab, getPlayerSpawnPosition(), Quaternion.identity, playerID)).GetComponent<LonerController>();
        players[playerID].PlayerID = playerID;
        players[playerID].SkinColor = getSkinColor();
        players[playerID].voice = Random.Range(-5, 3);

        networkView.RPC("AddLog", RPCMode.All, "Candidate #" + playerID + " joined the DEBATE");
    }

    public void DeletePlayer(int playerID)
    {
        Network.Destroy(players[playerID].gameObject);

        players[playerID] = null;

        networkView.RPC("AddLog", RPCMode.All, "Candidate #" + playerID + " left the DEBATE");
    }

    [RPC]
    public void PlayerMove(int playerID, int x)
    {
        players[playerID].Move(x);
    }

    [RPC]
    public void PlayerInteract(int playerID)
    {
        players[playerID].Interact();
    }
}
