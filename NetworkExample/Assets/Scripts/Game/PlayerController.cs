using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    // MonoBehaviourPun �ϸ� ���� �䰡 �˾Ƽ� �ֱ� ������ ������� �ʾƵ� ��
    private Rigidbody rb;
    private Animator anim;
    public Transform pointer;   // ĳ���Ͱ� �ٶ� ����
    
    public Bomb bombPrefab;
    public Transform shotPoint;
    public float shotPower = 15f;
    public float hp = 100f;
    public int shotCount = 0;
    //private PhotonView photonView;
    public float moveSpeed = 10f;
    public Text hpText;
    public Text shotText;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        pointer.gameObject.SetActive(photonView.IsMine);    //���� �����ϴ� ĳ������ Pointer�� Ȱ��ȭ

        hpText.text = hp.ToString();
        //photonView = GetComponent<PhotonView>();
    }


    private void Update()
    {
        if (false == photonView.IsMine) return;

        Move();

        if (Input.GetButtonDown("Fire1"))
        {
            shotCount++;
            shotText.text = shotCount.ToString();

            // ���ÿ����� ȣ��ǵ���
            //Fire();

            //PhotonNetwork�� RPC�� ȣ��.
            //photon Unity ����ȭ �԰� ã�ƺ��� ���� � �������ִµ� �ű⿡ � ���ԵǾ� �־ vector, Quaternion�� ���� �� ����
            photonView.RPC("Fire", RpcTarget.All, shotPoint.position, shotPoint.forward);
        }
    }

    private void FixedUpdate()
    {
        if (false == photonView.IsMine) return;
        Rotate();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(x, 0, z) * moveSpeed;
    }

    private void Rotate()
    {
        var pos = rb.position;  //�� rb�� ��ġ
        pos.y = 0;  //�������� ���� �� �����Ƿ� y����ǥ�� 0����

        var forward = pointer.position - pos;
        rb.rotation = Quaternion.LookRotation(forward, Vector3.up); //�� ��ġ���� pointer ������ �ٶ󺸵��� ��
    }

    // Bomb�� PhotonView�� ���� ��� ���ʿ��� ��Ŷ�� ��ȯ�Ǵ� ��ȿ���� �߻��ϹǷ�, Ư�� Ŭ���̾�Ʈ��
    // Fire�� ȣ���� ��� �ٸ� Ŭ���̾�Ʈ���� RPC�� ���� �Ȱ��� Fire�� ȣ���ϵ��� �ϰ����.
    // RPC�� �� �޼ҵ���� ��Ʈ����Ʈ�� ������ �ϸ� PhotonMessageInfo�� ������ �Ѵ�.
    [PunRPC]
    private void Fire(Vector3 shotPoint, Vector3 shotDirection, PhotonMessageInfo info)
    {
        // ���� �����ؼ�, ���� �ð��� �� Ŭ���̾�Ʈ �ð� ���̸�ŭ ���� ����.
        print($"fore Procedure called by {info.Sender.NickName}");
        print($"local time : {PhotonNetwork.Time}");
        print($"server time : {info.SentServerTime}");

        //                  1:35:20.5               1:35:20.3   0.2���� ������ ���� ���
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);


        var bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);
        bomb.rb.AddForce(shotDirection * shotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        // ��ź�� ��ġ���� ��ź�� �����ŭ �����ð� ���� ������ ��ġ�� ����
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    public void Hit(float damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            // ����
        }

        hpText.text = hp.ToString();
    }

    public void Heal(float amount)
    {
        hp += amount;

        if (hp > 100f) hp = 100f; // �ִ� ü���� 100���� �������� ���.

        hpText.text = hp.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //stream�� ���ؼ� hp�� shotCount�� ����ȭ
        // stream�� queue�� �����̱� ������ ���Լ���

        //�� ��
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(shotCount);
        }
        else  //���� ��
        {
            hp = (float)stream.ReceiveNext();
            shotCount = (int)stream.ReceiveNext();
            hpText.text = hp.ToString();
            shotText.text = shotCount.ToString();
        }
    }
}
