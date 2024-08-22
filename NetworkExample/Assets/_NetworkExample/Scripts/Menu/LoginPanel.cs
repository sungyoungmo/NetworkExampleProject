using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;

    public Button createButton;
    public Button loginButton;


    private void Awake()
    {
        //loginButton.onClick.AddListener(PhotonConnectButtonClick);

        createButton.onClick.AddListener(CreateButtonClick);
        loginButton.onClick.AddListener(LoginButtonClick);
    }
    private void OnEnable()
    {
        idInput.interactable = true;
        loginButton.interactable = true;
    }


    private IEnumerator Start()
    {
        //idInput.text = $"Player {Random.Range(100, 1000)}";

        idInput.interactable = false;
        pwInput.interactable = false;
        createButton.interactable = false;
        loginButton.interactable = false;

        yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);

        idInput.interactable = true;
        pwInput.interactable = true;
        createButton.interactable = true;
        loginButton.interactable = true;
    }

    public void CreateButtonClick()
    {
        createButton.interactable = false;
        FirebaseManager.Instance.Create(idInput.text, pwInput.text,
            (user) =>
            {
                print("ȸ�� ���� ����");
                createButton.interactable = true;
            }
         );

    }

    public void LoginButtonClick()
    {
        loginButton.interactable = false;
        FirebaseManager.Instance.Login(idInput.text, pwInput.text,
            (user) =>
            {
                loginButton.interactable = true;
            }
        );
    }

    public void PhotonConnectButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = idInput.text;
        PhotonNetwork.ConnectUsingSettings();
        //PanelManager.Instance.PanelOpen("Menu");
        // ��ư�� ��ǲ�ʵ� ��Ȱ��ȭ ��Ű�� �α��� �޽��� �Ǵ� ������ ���
        idInput.interactable = false;
        loginButton.interactable = false;

    }

}
