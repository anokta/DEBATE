using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    void Update () {
        if (Network.isClient)
        {
            // Move
            int x = (int)Input.GetAxisRaw("Horizontal");
            int y = (int)Input.GetAxisRaw("Vertical");

            if (x != 0)
            {
                networkView.RPC("PlayerMove", RPCMode.Server, int.Parse(Network.player.ToString()), x, y);
            }

            // Interact
            if (Input.GetButtonDown("Jump"))
            {
                networkView.RPC("PlayerInteract", RPCMode.Server, int.Parse(Network.player.ToString()));
            }
        }
	}
}
