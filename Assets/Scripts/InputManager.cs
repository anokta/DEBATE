// ----------------------------------------------------------------------
//   DEBATE - Ludum Dare 31 Compo Entry
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// -----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    void Update () {
        if (Network.isClient)
        {
            int playerID = int.Parse(Network.player.ToString());

            // Move
            int x = (int)Input.GetAxisRaw("Horizontal");
            if (x != 0)
            {
                networkView.RPC("PlayerMove", RPCMode.Server, playerID, x);
            }

            // Interact
            if (Input.GetButtonDown("Jump"))
            {
                networkView.RPC("PlayerInteract", RPCMode.Server, playerID);
            }
        }
	}
}
