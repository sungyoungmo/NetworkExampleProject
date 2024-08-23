using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Text playerName;

    public InputField playerNameInput;
    public Button playerNameChangeButton;
    // 방 메뉴와 방 생성 메뉴를 구분해야 함.

    [Header("Main Menu")]
    public RectTransform mainMenuPanel;
    public Button createRoomButton;
    public Button findRoomButton;
    public Button randomRoomButton;
    public Button logoutButton;

    [Header("Create Room Menu")]
    public RectTransform createRoomPanel;
    public InputField roomNameInput;
    public InputField playerNumInput;
    public Button createButton;
    public Button cancelButton;

    private void Awake()
    {
        playerNameChangeButton.onClick.AddListener(PlayerNameChangeButtonClick);
        createRoomButton.onClick.AddListener(CreateRoomButtonClick);
        findRoomButton.onClick.AddListener(FindRoomButtonClick);
        randomRoomButton.onClick.AddListener(RandomRoonButtonClick);
        logoutButton.onClick.AddListener(LogoutButtonClick);
        createButton.onClick.AddListener(CreateButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
    }

    private void OnEnable()
    {
        playerName.text = $"안녕하세요. {HWFirebaseManager.Instance.usersRef.Child("userName").ToString()}";
        mainMenuPanel.gameObject.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
    }

    private void PlayerNameChangeButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        playerName.text = $"안녕하세요. {PhotonNetwork.LocalPlayer.NickName}";
    }

    private void CreateRoomButtonClick()    //방 생성 버튼
    {
        mainMenuPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(true);
    }

    private void FindRoomButtonClick() // 방 목록을 받아오기 위해 로비에 입장
    {
        print(1);
        PhotonNetwork.JoinLobby();
    }

    private void RandomRoonButtonClick()
    {
        RoomOptions option = new() 
        {
            MaxPlayers = 8,
        };
        string roomName = $"Random Room {Random.Range(100, 1000)}";
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:option, roomName: roomName);
    }

    private void LogoutButtonClick()
    {
        mainMenuPanel.gameObject.SetActive(false);
        PhotonNetwork.Disconnect();
    }

    private void CreateButtonClick()
    {
        string roomName = roomNameInput.text;

        int maxPlayer;
        

        if (int.TryParse(playerNumInput.text, out maxPlayer))
        {
            
        }
        else
        {
            maxPlayer = 8;
        }

        //유효성 검사 이렇게 함.

        if (string.IsNullOrEmpty(roomName))
        {
            // 같은 방 번호가 있을 수 있으므로 사실 좀 더 섬세한 유효성 검사가 필요
            roomName = $"Room{Random.Range(0, 1000)}";
        }

        if (maxPlayer <= 0)
        {
            maxPlayer = 8;
        }


        PhotonNetwork.CreateRoom
            (
                roomName,
                new RoomOptions()
                {
                    MaxPlayers = maxPlayer,
                }
            );
    }

    private void CancelButtonClick() // 방만들기 패널의 취소 버튼
    {
        mainMenuPanel.gameObject.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
    }
}
