// ----------------------------------------------------------------------
//   DEBATE - Ludum Dare 31 Compo Entry
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// -----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogManager : MonoBehaviour {

    public GUISkin guiSkin;
    public int capacity;

    public static List<string> Logs;

	void Awake () {
        Logs = new List<string>();
	}

    void OnGUI()
    {
        if (GameEventManager.CurrentState == GameEventManager.GameState.Running)
        {
            GUI.skin = guiSkin;

            GUILayout.BeginArea(new Rect(0.575f * Screen.width, 0.025f * Screen.height, 0.4f * Screen.width, 0.5f * Screen.height));

            GUI.skin.label.alignment = TextAnchor.UpperRight;
            foreach (string log in Logs)
                GUILayout.Label(log);

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0.025f * Screen.width, 0.025f * Screen.height, 0.25f * Screen.width, 0.25f * Screen.height));

            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            string playersLog = NetworkManager.PlayerCount + " candidates are currently debating..";
            if (NetworkManager.PlayerCount <= 1) playersLog = "You are currently debating by yourself..";
            GUILayout.Label(playersLog);
            
            GUILayout.EndArea();
        }
    }

    [RPC]
    public void AddLog(string log)
    {
        if(Logs.Count >= capacity)
        {
            Logs.RemoveAt(0);
        }

        Logs.Add(log);
    }
}
