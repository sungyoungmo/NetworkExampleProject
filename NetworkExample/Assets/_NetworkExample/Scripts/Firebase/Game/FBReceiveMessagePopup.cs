using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBReceiveMessagePopup : MonoBehaviour
{
    public Text msgText;

    //private void Start()
    //{
    //    FirebaseManager.Instance.onReceiveMessage += OnReceiveMessage;
    //}

    //������ ������ �޽����� ������ �� ȣ��
    public void OnReceiveMessage(string msg)
    {
        msgText.text = msg;
        gameObject.SetActive(true);
    }
}
