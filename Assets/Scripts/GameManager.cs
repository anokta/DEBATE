﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    Dictionary<int, LonerController> players;

    void Awake()
    {
        players = new Dictionary<int, LonerController>();
    }

    Vector3 getPlayerSpawnPosition()
    {
        return new Vector3(Random.Range(-5, 5), 0, 0);
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

    public void SpawnPlayer(NetworkPlayer player)
    {
        networkView.RPC("SpawnPlayer", player, player);
        players[int.Parse(player.ToString())] = ((GameObject)Network.Instantiate(playerPrefab, getPlayerSpawnPosition(), Quaternion.identity, 1)).GetComponent<LonerController>();
            
        players[int.Parse(player.ToString())].SkinColor = getSkinColor();
    }

    public void DeletePlayer(int playerID)
    {
        Network.Destroy(players[playerID].gameObject);

        players[playerID] = null;
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