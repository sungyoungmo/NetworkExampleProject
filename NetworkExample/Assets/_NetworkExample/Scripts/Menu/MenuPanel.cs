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
    // �� �޴��� �� ���� �޴��� �����ؾ� ��.

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
        playerName.text = $"�ȳ��ϼ���. {HWFirebaseManager.Instance.usersRef.Child("userName").ToString()}";
        mainMenuPanel.gameObject.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
    }

    private void PlayerNameChangeButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        playerName.text = $"�ȳ��ϼ���. {PhotonNetwork.LocalPlayer.NickName}";
    }

    private void CreateRoomButtonClick()    //�� ���� ��ư
    {
        mainMenuPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(true);
    }

    private void FindRoomButtonClick() // �� ����� �޾ƿ��� ���� �κ� ����
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

        //��ȿ�� �˻� �̷��� ��.

        if (string.IsNullOrEmpty(roomName))
        {
            // ���� �� ��ȣ�� ���� �� �����Ƿ� ��� �� �� ������ ��ȿ�� �˻簡 �ʿ�
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

    private void CancelButtonClick() // �游��� �г��� ��� ��ư
    {
        mainMenuPanel.gameObject.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
    }
}
