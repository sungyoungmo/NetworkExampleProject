using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ϲ����� ���¿����� ���� �ö���� �ʰ�, ������� ���� �κ� �ǳ� �ٰ� ���� ���� ������ ��� Ȱ��ȭ
public class PhotonDebuger : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.NickName = $"Tester {Random.Range(1,10)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // �׽�Ʈ�� ���� ������ �Ǹ� �׽�Ʈ�� ���� ����
        RoomOptions testRoomOptions = new RoomOptions()
        {
            IsVisible = false,
            MaxPlayers = 10
        };
        PhotonNetwork.JoinOrCreateRoom("TestRoom",testRoomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        GameObject.Find("Canvas/DebugText").GetComponent<UnityEngine.UI.Text>().text = PhotonNetwork.CurrentRoom.Name;
        TestManager.debugReady = true;
    }
}
