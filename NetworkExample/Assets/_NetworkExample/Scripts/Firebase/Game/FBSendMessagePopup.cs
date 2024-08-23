using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBSendMessagePopup : MonoBehaviour
{
    public InputField toInput;
    public InputField msgInput;
    public Button sendButton;

    private void Awake()
    {
        sendButton.onClick.AddListener(SendButtonClick);
    }

    public void SendButtonClick()
    {
        Message msg = new Message()
        {
            sender = FirebaseManager.Instance.Auth.CurrentUser.UserId,  // ������: ��
            message = msgInput.text,    //�޽��� ����
            sendTime = DateTime.Now.Ticks   // ���� �ð�
        
        };

        //TODO: �޽��� ����
        FirebaseManager.Instance.SendMsg(toInput.text, msg);
    }
}
