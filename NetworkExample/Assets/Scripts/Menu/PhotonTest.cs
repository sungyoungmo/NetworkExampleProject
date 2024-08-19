using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PhotonTest : MonoBehaviour
{
    private ClientState photonState = 0; // cache �뵵�� Ȱ���ϱ� ������ private�� ���Ƶд�.

    private void Update()
    {
        if (PhotonNetwork.NetworkClientState != photonState)
        {
            LogManager.Log($"state changed : {PhotonNetwork.NetworkClientState}");
            photonState = PhotonNetwork.NetworkClientState;
        }
    }

}
