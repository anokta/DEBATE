using UnityEngine;
using System.Collections;

public class GameManagerClient : MonoBehaviour
{
    public int masterPort = 23466;
    public string masterIP = "54.174.144.122";

    public string gameTypeName = "MatchmakerLD31";

    public GameObject playerPrefab;

    public string log;


    void Awake()
    {
        MasterServer.port = masterPort;
        MasterServer.ipAddress = masterIP;
    }

    void Update()
    {
        FindObjectOfType<GUIText>().text = log;
    }

    void OnGUI()
    {
        if(GUILayout.Button("Join"))
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

    void OnConnectedToServer()
    {
        log = "Connected to server";
        Debug.Log("Connected to server");

        Network.Instantiate(playerPrefab, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity, 0);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        log = "Disconnected";

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
        log = "Could not connect to server: " + error;
        Debug.Log("Could not connect to server: " + error);
    }   
}