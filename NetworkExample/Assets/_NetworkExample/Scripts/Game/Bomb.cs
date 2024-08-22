using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public ParticleSystem particlePrefab;

    //[HideInInspector]
    //public Rigidbody rb;

    //[HideInInspector]
    //public Player owner;

    // �̰� �ǰԵ� ĸ��ȭ ���
    //private Rigidbody rb;
    //public Rigidbody RB { get => rb; }
    //�Ǵ�
    public Rigidbody rb { get; private set; }
    public Player owner { get; set; }

    public float expRad = 1.5f;    // explosion radius => ���� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var particle = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

        particle.Play();
        Destroy(particle, 3f); // ��� ������Ʈ Ǯ�� ���°� ����

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.1f);

        // ���� ���� ���� ������ �����ؼ� �ش� ���� �ȿ� ���� �ݶ��̴��� ���
        var contactedColliders = Physics.OverlapSphere(transform.position, expRad);

        foreach (var coll in contactedColliders)
        {
            //�ʱ��ڿ�
            //if (coll.tag.Equals("Player"))
            //{
            //    //�÷��̾�� Ÿ�� �Լ� ȣ��
            //    coll.SendMessage("Hit", 1, SendMessageOptions.RequireReceiver);
            //}

            if (coll.TryGetComponent<PlayerController>(out var player))
            {

                // local �÷��̾��� ���� ��ź�� ���� �÷��̾�� �������� ���.
                bool isMine = PhotonNetwork.LocalPlayer.ActorNumber == player.photonView.Owner.ActorNumber;
                if (isMine)
                {
                    player.Hit(1);
                }

                print($"{owner.NickName}�� ���� ��ź�� {player.photonView.Owner.NickName}���� ����");
            }
        }
    }
}
