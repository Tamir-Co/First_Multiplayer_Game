using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class CreateGame : NetworkBehaviour
{
    private int room_size = 2;
    private string room_name;
    private NetworkManager networkManager;

    // Start is called before the first frame update
    void Start() {
        networkManager = NetworkManager.singleton;
        //NetworkManager.singleton.start
        //if (networkManager.)D
        //NetworkManager.singleton.match
        //networkManager.matchMaker.CreateMatch("roomName", 4, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public void set_RoomName(string name)
    {
        room_name = name;
    }

    public void CreateRoom()
    {
        if(room_name != "" && room_name != null)
        {
            Debug.Log("creating room " + room_name + " for " + room_size + "players.");
            //NetworkManager
            //networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            // networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }

}