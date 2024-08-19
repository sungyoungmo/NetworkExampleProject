using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomPanel : MonoBehaviour
{
    public Text roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button cancelButton;

    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
    }


    private void OnDisable()
    {
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnEnable()
    {
        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);
        }
    }

    public void JoinPlayer(Player newPlayer)
    {
        GameObject playerName = Instantiate(playerTextPrefab, playerList, false);
        playerName.name = newPlayer.NickName;
        playerName.GetComponent<Text>().text = newPlayer.NickName;
    }

    public void LeavePlayer(Player gonePlayer)
    {
        GameObject leaveTarget = playerList.Find(gonePlayer.NickName).gameObject;

        Destroy(leaveTarget);
        //Destroy(playerList.Find(gonePlayer.NickName));
    }

    private void StartButtonClick()
    {
        // 게임 시작 버튼
    }
    private void CancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }
}
