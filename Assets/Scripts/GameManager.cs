using UnityEngine;
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

    public void SpawnPlayer(int playerID)
    {
        players[playerID] = ((GameObject)Network.Instantiate(playerPrefab, getPlayerSpawnPosition(), Quaternion.identity, 1)).GetComponent<LonerController>();
    }

    public void DeletePlayer(int playerID)
    {
        Network.Destroy(players[playerID].gameObject);

        players[playerID] = null;
    }

    [RPC]
    public void PlayerMove(int playerID, int x, int y)
    {
        players[playerID].Move(x, y);
    }

    [RPC]
    public void PlayerInteract(int playerID)
    {
        players[playerID].Interact();
    }
}
