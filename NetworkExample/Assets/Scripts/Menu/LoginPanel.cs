using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public InputField idInput;
    public Button loginButton;


    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }
    private void OnEnable()
    {
        idInput.interactable = true;
        loginButton.interactable = true;
    }


    private void Start()
    {
        idInput.text = $"Player {Random.Range(100, 1000)}";

        
    }

    public void OnLoginButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = idInput.text;
        PhotonNetwork.ConnectUsingSettings();
        //PanelManager.Instance.PanelOpen("Menu");
        // 버튼과 인풋필드 비활성화 시키고 로그인 메시지 또는 아이콘 출력
        idInput.interactable = false;
        loginButton.interactable = false;
        
    }

}
