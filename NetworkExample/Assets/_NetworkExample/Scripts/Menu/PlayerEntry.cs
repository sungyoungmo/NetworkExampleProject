using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    public Text playerNameText;
    public Toggle readyToggle;
    public GameObject readyLabel;

    public Player player;
    public bool IsMine => player == PhotonNetwork.LocalPlayer;

    public Toggle[] eyesToggle;

    private void Awake()
    {
        //readyToggle.onValueChanged.AddListener(ReadyToggleClick);
        //readyToggle.isOn = false; => onValueChaged�� ȣ���
        readyToggle.SetIsOnWithoutNotify(false);    // => isOn���� ���������� onValueChaged�� ȣ����� ����

        foreach (Toggle toggle in eyesToggle)
        {
            toggle.SetIsOnWithoutNotify(false);
        }
        eyesToggle[0].isOn = true;

    }

    private void ReadyToggleClick(bool isOn)
    {
        // Ŀ���� ������Ƽ�� isOn�� �߰��ϴ� ������ �ۼ����� ���
    }
}
