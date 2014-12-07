using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    public int masterPort = 23466;
    public string masterIP = "54.174.144.122";

    public string gameTypeName = "MatchmakerLD31";
    public string gameName = "MatchmakerMain";

    public int connections = 128;
    public int port = 25005;

    public bool isHost = false;

    int playerCount;
    bool serverInitialized;

    GameManager gameManager;

    void Awake()
    {
        MasterServer.port = masterPort;
        MasterServer.ipAddress = masterIP;
        MasterServer.dedicatedServer = true;

        playerCount = 0;
        serverInitialized = false;

        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if(isHost)
        {
            if (!serverInitialized)
            {
                Debug.Log("[HOST] Attempting to initialize the server..");

                Network.Disconnect(0);
                Network.InitializeServer(connections, port, !Network.HavePublicAddress());

                MasterServer.RegisterHost(gameTypeName, gameName);
            }
        }
    }


    void OnGUI()
    {
        if (!isHost)
        {
            if (GUILayout.Button("Join"))
            {
                MasterServer.RequestHostList(gameTypeName);
            }

            HostData[] data = MasterServer.PollHostList();

            // Go through all the hosts in the host list
            foreach (HostData element in data)
            {
                GUILayout.BeginHorizontal();
                var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
                GUILayout.Label(name);
                GUILayout.Space(5);
                string hostInfo;
                hostInfo = "[";

                foreach (string host in element.ip)
                    hostInfo = hostInfo + host + ":" + element.port + " ";
                hostInfo = hostInfo + "]";
                GUILayout.Label(hostInfo);
                GUILayout.Space(5);
                GUILayout.Label(element.comment);
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Connect"))
                {
                    // Connect to HostData struct, internally the correct method is used (GUID when using NAT).
                    Network.Connect(element);
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    void OnDestroy()
    {
        Network.Disconnect();

        Debug.Log("Disconnected.");
    }
    void OnServerInitialized()
    {
        serverInitialized = true;

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
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("[HOST] Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);

        gameManager.SpawnPlayer(int.Parse(player.ToString()));
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
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
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }
}