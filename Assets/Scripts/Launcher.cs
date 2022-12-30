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

    private List<RoomInformation> roomBtnList = new List<RoomInformation>();
    private List<TMP_Text> playersList = new List<TMP_Text>();
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

    private void ListPlayers()
    {
        foreach (TMP_Text pl in playersList)
        {
            Destroy(pl.gameObject);
        }
        playersList.Clear();

        MainMenuNew.instance.playerName.gameObject.SetActive(false);

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            
            TMP_Text pLabel = Instantiate(MainMenuNew.instance.playerName, MainMenuNew.instance.playerName.transform.parent);
            pLabel.text = players[i].NickName;
            pLabel.gameObject.SetActive(true);

            playersList.Add(pLabel);
            

        }
    }

    public override void OnJoinedRoom()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.roomOverviewCanvas.SetActive(true);

        MainMenuNew.instance.roomName.text = $"{PhotonNetwork.CurrentRoom.Name} room list:";

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();
        ListPlayers();
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text pLabel = Instantiate(MainMenuNew.instance.playerName, MainMenuNew.instance.playerName.transform.parent);
        pLabel.text = newPlayer.NickName;
        pLabel.gameObject.SetActive(true);

        playersList.Add(pLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListPlayers();
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

    public void FindRoom()
    {
        
        MainMenuNew.instance.findRoomCanvas.SetActive(true);
    }


    public override void OnLeftRoom()
    {
        MainMenuNew.instance.CloseMenus();
        MainMenuNew.instance.firstMenu.SetActive(true);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInformation ri in roomBtnList)
        {
            Destroy(ri.gameObject);
        }
        roomBtnList.Clear();

        MainMenuNew.instance.roomInfoBtn.gameObject.SetActive(false);

        for (int i=0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomInformation rBtn = Instantiate(MainMenuNew.instance.roomInfoBtn, MainMenuNew.instance.roomInfoBtn.transform.parent);
                rBtn.PopulateRoomInfo(roomList[i]);
                rBtn.gameObject.SetActive(true);

                roomBtnList.Add(rBtn);
            }
           
        }

    }

    public void JoinRoom(RoomInfo rInfo)
    {
        PhotonNetwork.JoinRoom(rInfo.Name);
        MainMenuNew.instance.CloseMenus();

        MainMenuNew.instance.loadingText.text = "Joining Room...";
        MainMenuNew.instance.loadingMenu.SetActive(true);


    }
}
