// ----------------------------------------------------------------------
//   DEBATE - Ludum Dare 31 Compo Entry
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// -----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    public static int PlayerCount;

    public int masterPort = 23466;
    public string masterIP = "54.174.144.122";

    public int facilitatorPort = 50005;

    public string gameTypeName = "MatchmakerLD31";
    public string gameName = "MatchmakerMain";

    public int connections = 128;
    public int port = 25005;

    public bool isHost = false;

    public float waitInterval = 5.0f;
    float elapsed;

    public bool serverInitialized;

    GameManager gameManager;

    void Awake()
    {
        MasterServer.port = masterPort;
        MasterServer.ipAddress = masterIP;
        MasterServer.dedicatedServer = true;

        Network.natFacilitatorPort = facilitatorPort;
        Network.natFacilitatorIP = masterIP;

        PlayerCount = 0;

        gameManager = GetComponent<GameManager>();

        GameEventManager.GameMenu += GameMenu;
    }

    void Update()
    {
        if (((!Network.isServer && isHost) || !serverInitialized) && Time.time - elapsed > waitInterval)
        {
            Debug.Log("Connecting..");

            elapsed = Time.time;

            if (isHost)
            {
                Debug.Log("[HOST] Attempting to initialize the server..");

                Network.Disconnect(0);
                Network.InitializeServer(connections, port, !Network.HavePublicAddress());

                MasterServer.RegisterHost(gameTypeName, gameName);
            }
            else if(!serverInitialized)
            {
                MasterServer.RequestHostList(gameTypeName);

                HostData[] data = MasterServer.PollHostList();
                foreach (HostData element in data)
                {
                    Debug.Log("Attemting to connect to the server..");

                    if (element.gameName.Equals(gameName))
                    {
                        Network.Connect(element);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        GameEventManager.GameMenu -= GameMenu;

        Network.Disconnect();

        Debug.Log("Disconnected.");
    }

    void GameMenu()
    {
        serverInitialized = false;
        elapsed = Time.time;
    }

    void OnServerInitialized()
    {
        serverInitialized = true;
        PlayerCount = 0;

        Debug.Log("[HOST] Server initialized successfully.");
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        serverInitialized = false;

        Debug.Log("[HOST] Could not connect to master server: " + info);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("[HOST] Clean up after player " + player);

        int playerID = int.Parse(player.ToString());

        Network.RemoveRPCsInGroup(playerID);

        gameManager.DeletePlayer(playerID);

        PlayerCount--;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        PlayerCount++;

        Debug.Log("[HOST] Player " + PlayerCount + " connected from " + player.ipAddress + ":" + player.port);

        gameManager.SpawnPlayer(int.Parse(player.ToString()));
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");

        serverInitialized = true;

        GameEventManager.TriggerGameStart();
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");

        GameEventManager.TriggerGameOver();
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);

        serverInitialized = false;
    }
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        stream.Serialize(ref PlayerCount);
    }
}