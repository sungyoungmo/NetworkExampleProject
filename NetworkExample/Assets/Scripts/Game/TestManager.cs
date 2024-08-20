using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Transform startPositions;




    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(NormalStart());
        }
        else
        {
            // ������ �� ���� �� ���� ������ �ǳ� �پ����Ƿ�, �ڵ����� ����׷뿡 �����Ŵ
            StartCoroutine(DebugStart());
        }
    }

    private IEnumerator NormalStart()
    {
        // PhotonNetwork�� ��� �÷��̾��� �ε� ���¸� �Ǵ��Ͽ� �ѹ����� �ؾ� �ϴµ�, ���� �׷� ����� �����Ǿ� ���� �����Ƿ�, 1�� ��� �� ���� ���� ���� ����
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);

        // Resources ����� �ϴ� ��� �޸𸮿� �ø��� ������ ���򿡴� ����õ�ε� ������ ���� ����̶� Resources ��� ���� ��������
        //GameObject playerPrefab = Resources.Load<GameObject>("Player");
        //Instantiate(playerPrefab, startPositions.GetChild(0).position, Quaternion.identity);

        // ���ӿ� ������ �濡�� �ο��� �� ��ȣ.
        // Ȱ���ϱ� ���ؼ��� ���� ���� PlayerNumbering ������Ʈ�� �����ؾ� ��
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPos = startPositions.GetChild(playerNumber);

        GameObject playerObject = PhotonNetwork.Instantiate("Player", playerPos.position, playerPos.rotation);

        playerObject.name = $"Player {playerNumber}";
    }

    public static bool debugReady = false;

    private IEnumerator DebugStart()
    {
        // ����� ������ Start ����
        gameObject.AddComponent<PhotonDebuger>();

        yield return new WaitUntil(()=> debugReady);

        StartCoroutine(NormalStart());
    }
}
