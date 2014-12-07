using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    Dictionary<int, LonerController> players;

    AudioSource noise;
    public float noiseVolume;

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
        float targetVolume =  GameEventManager.CurrentState == GameEventManager.GameState.Running ? noiseVolume : 0.0f;
        if (noise.volume != targetVolume)
            noise.volume = Mathf.Lerp(noise.volume, targetVolume, Time.deltaTime);
    }

    void OnDestroy()
    {
        GameEventManager.GameMenu -= GameMenu;
        GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
    }

    void GameMenu()
    {

    }

    void GameStart()
    {

    }

    void GameOver()
    {
        LonerController[] players = FindObjectsOfType<LonerController>();
        foreach (LonerController loner in players)
            Destroy(loner.gameObject);
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

    public void SpawnPlayer(int playerID)
    {
        players[playerID] = ((GameObject)Network.Instantiate(playerPrefab, getPlayerSpawnPosition(), Quaternion.identity, playerID)).GetComponent<LonerController>();

        players[playerID].SkinColor = getSkinColor();
        players[playerID].voice = Random.Range(-5, 3);
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
