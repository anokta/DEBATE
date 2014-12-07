using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    //GameManager gameManager;

    //void Awake()
    //{
    //    gameManager = GetComponent<GameManager>();
    //}

    void Update () {
        if (Network.isClient)
        {
            // Move
            int x = (int)Input.GetAxisRaw("Horizontal");
            //int y = (int)Input.GetAxisRaw("Vertical");

            int playerID = int.Parse(Network.player.ToString());
            
            if (x != 0)
            {
                networkView.RPC("PlayerMove", RPCMode.Server, playerID, x);
                //gameManager.PlayerMove(playerID, x);
            }

            // Interact
            if (Input.GetButtonDown("Jump"))
            {
                networkView.RPC("PlayerInteract", RPCMode.Server, int.Parse(Network.player.ToString()));
                //gameManager.PlayerInteract(playerID);
            }
        }
	}
}
