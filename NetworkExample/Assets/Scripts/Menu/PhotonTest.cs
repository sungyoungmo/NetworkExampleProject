using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PhotonTest : MonoBehaviour
{
    private ClientState photonState = 0; // cache 용도로 활용하기 때문에 private로 막아둔다.

    private void Update()
    {
        if (PhotonNetwork.NetworkClientState != photonState)
        {
            LogManager.Log($"state changed : {PhotonNetwork.NetworkClientState}");
            photonState = PhotonNetwork.NetworkClientState;
        }
    }

}
