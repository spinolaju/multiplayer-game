using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomInformation : MonoBehaviour
{
    public static RoomInformation instance;
    public TMP_Text btnNameText;
    public TMP_Text btnPlayersNoText;
    private RoomInfo roomInfo;

    private void Awake()
    {
        instance = this;
    }


    public void PopulateRoomInfo(RoomInfo info)
    {
        roomInfo = info;
        btnNameText.text = $"{roomInfo.Name}";
        btnPlayersNoText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
    }

    public void OpenRoom()
    {
        Launcher.instance.JoinRoom(roomInfo);
    }
}
