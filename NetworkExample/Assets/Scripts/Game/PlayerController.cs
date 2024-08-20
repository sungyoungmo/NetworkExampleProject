using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    // MonoBehaviourPun 하면 포톤 뷰가 알아서 있기 때문에 사용하지 않아도 됨
    private Rigidbody rb;
    private Animator anim;
    public Transform pointer;   // 캐릭터가 바라볼 방향
    
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
        pointer.gameObject.SetActive(photonView.IsMine);    //내가 조종하는 캐릭터의 Pointer만 활성화

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

            // 로컬에서만 호출되도록
            //Fire();

            //PhotonNetwork의 RPC를 호출.
            //photon Unity 직렬화 규격 찾아보면 나옴 몇개 정해져있는데 거기에 몇개 포함되어 있어서 vector, Quaternion를 보낼 수 있음
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
        var pos = rb.position;  //내 rb의 위치
        pos.y = 0;  //고저차가 있을 수 있으므로 y축좌표를 0으로

        var forward = pointer.position - pos;
        rb.rotation = Quaternion.LookRotation(forward, Vector3.up); //내 위치에서 pointer 쪽으로 바라보도록 함
    }

    // Bomb에 PhotonView가 붙을 경우 불필요한 패킷이 교환되는 비효율이 발생하므로, 특정 클라이언트가
    // Fire를 호출할 경우 다른 클라이언트에게 RPC를 통해 똑같이 Fire를 호출하도록 하고싶음.
    // RPC를 한 메소드들은 어트리부트를 가져야 하며 PhotonMessageInfo를 가져야 한다.
    [PunRPC]
    private void Fire(Vector3 shotPoint, Vector3 shotDirection, PhotonMessageInfo info)
    {
        // 지연 보상해서, 서버 시간과 내 클라이언트 시간 차이만큼 값을 보정.
        print($"fore Procedure called by {info.Sender.NickName}");
        print($"local time : {PhotonNetwork.Time}");
        print($"server time : {info.SentServerTime}");

        //                  1:35:20.5               1:35:20.3   0.2초의 지연이 있을 경우
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);


        var bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);
        bomb.rb.AddForce(shotDirection * shotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        // 폭탄의 위치에서 폭탄의 운동량만큼 지연시간 동안 진행한 위치로 보정
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    public void Hit(float damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            // 죽음
        }

        hpText.text = hp.ToString();
    }

    public void Heal(float amount)
    {
        hp += amount;

        if (hp > 100f) hp = 100f; // 최대 체력을 100으로 설정했을 경우.

        hpText.text = hp.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //stream을 통해서 hp와 shotCount만 동기화
        // stream은 queue의 형태이기 떄문에 선입선출

        //쓸 때
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(shotCount);
        }
        else  //읽을 때
        {
            hp = (float)stream.ReceiveNext();
            shotCount = (int)stream.ReceiveNext();
            hpText.text = hp.ToString();
            shotText.text = shotCount.ToString();
        }
    }
}
