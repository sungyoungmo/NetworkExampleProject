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
        // ��ư�� ��ǲ�ʵ� ��Ȱ��ȭ ��Ű�� �α��� �޽��� �Ǵ� ������ ���
        idInput.interactable = false;
        loginButton.interactable = false;
        
    }

}
