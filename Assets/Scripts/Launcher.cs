using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using SlimUI.ModernMenu;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

   
    // Start is called before the first frame update
    void Start()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.loadingMenu.SetActive(true);
        MainMenuNew.instance.loadingText.text = "Connecting...";

        PhotonNetwork.ConnectUsingSettings();

        Debug.Log(PhotonNetwork.NetworkClientState);
    }

    public void CreateRoom()
    {
        if(!string.IsNullOrEmpty(MainMenuNew.instance.roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 10;
            PhotonNetwork.CreateRoom(MainMenuNew.instance.roomNameInput.text, options);

            MainMenuNew.instance.CloseMenus();
            MainMenuNew.instance.loadingText.text = "Creating room...";
            MainMenuNew.instance.loadingMenu.SetActive(true);


        }
        
        
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        MainMenuNew.instance.loadingText.text = "Joining Lobby...";
    }
    public override void OnJoinedLobby()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.firstMenu.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.roomOverviewCanvas.SetActive(true);

        MainMenuNew.instance.roomName.text = $"{PhotonNetwork.CurrentRoom.Name} room list:";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.errorText.text = $"Failed to create room: {message}";
        
        MainMenuNew.instance.errorDialog.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MainMenuNew.instance.loadingText.text = "Leaving Room...";
        MainMenuNew.instance.loadingMenu.SetActive(true);
    }


    public override void OnLeftRoom()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.firstMenu.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
