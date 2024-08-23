using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PanelManager : MonoBehaviourPunCallbacks
{
    public static PanelManager Instance { get; private set; }


    public LoginPanel login;
    public MenuPanel menu;
    public LobbyPanel lobby;
    public RoomPanel room;

    private Dictionary<string, GameObject> panels;

    #region 메시지 함수

    private void Awake()
    {
        Instance = this;
        panels = new Dictionary<string, GameObject>
        {
            { "Login", login.gameObject },
            { "Menu", menu.gameObject },
            { "Lobby", lobby.gameObject },
            { "Room", room.gameObject }
        };
        PanelOpen("Login");
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void Start()
    {
        
    }


    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnEnable()
    {
        //base.OnEnable();
    }
    public override void OnDisable()
    {
        //base.OnDisable();
    }

    #endregion

    public void PanelOpen(string panelName)
    {
        foreach (var row in panels)
        {
            //if (row.Key == panelName)
            //{
            //    row.Value.SetActive(true);
            //}
            //else
            //{
            //    row.Value.SetActive(false);
            //}

            row.Value.SetActive(row.Key.Equals(panelName));
        }
    }

    public override void OnConnected()
    {
        PanelOpen("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LogManager.Log($"disconnected cause {cause}");
        PanelOpen("Login");
    }

    public override void OnJoinedLobby()
    {
        PanelOpen("Lobby");

    }
    public override void OnLeftLobby()
    {
        PanelOpen("Menu");
    }
    public override void OnJoinedRoom()
    {
        PanelOpen("Room");
    }
    public override void OnCreatedRoom()
    {
        PanelOpen("Room");
    }

    public override void OnLeftRoom()
    {
        PanelOpen("Menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        room.JoinPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        room.LeavePlayer(otherPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobby.UpdateRoomList(roomList);
    }
}
