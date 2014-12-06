using UnityEngine;
using System.Collections;

public class GameManagerServer : MonoBehaviour
{
    public int masterPort = 23466;
    public string masterIP = "54.174.144.122";

    public string gameTypeName = "MatchmakerLD31";
    public string gameName = "MatchmakerMain";

    public int connections = 128;
    public int port = 25005;

    int playerCount;
    bool serverInitialized;

    void Awake()
    {
        MasterServer.port = masterPort;
        MasterServer.ipAddress = masterIP;
        MasterServer.dedicatedServer = true;

        playerCount = 0;
        serverInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!serverInitialized)
        {
            Debug.Log("Attempting to initialize the server..");

            Network.Disconnect(0);
            Network.InitializeServer(connections, port, !Network.HavePublicAddress());
            
            MasterServer.RegisterHost(gameTypeName, gameName);
        }
    }

    void OnDestroy()
    {
        Network.Disconnect();

        Debug.Log("Server disconnected.");
    }

    void OnServerInitialized()
    {
        serverInitialized = true;

        Debug.Log("Server initialized successfully.");
    }
    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        serverInitialized = false; 

        Debug.Log("Could not connect to master server: " + info);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }    
    
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
    }
}