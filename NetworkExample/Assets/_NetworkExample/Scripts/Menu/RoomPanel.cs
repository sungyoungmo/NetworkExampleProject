using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class RoomPanel : MonoBehaviourPunCallbacks
{
    public Text roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button cancelButton;
    public Dropdown diffDropdown;
    public Text diffText;

    // 방장일 경우, 플레이어들의 ready 상태를 저장할 dictionary
    private Dictionary<int, bool> playersReady;
    // 방에 들어온 모든 플레이어들이 서로를 알고 있도록 사용할 Dictionary
    public Dictionary<int, PlayerEntry> playerEnrties = new();


    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
        
        diffDropdown.ClearOptions();    // 개발하다보면 옵션을 넣어놓고 테스트했을 수도 있어서 다 밀어버리는거임
        foreach (object diff in System.Enum.GetValues(typeof(Difficulty)))
        {
            Dropdown.OptionData option = new Dropdown.OptionData(diff.ToString());
            diffDropdown.options.Add(option);
        }

        diffDropdown.onValueChanged.AddListener(DifficultyValueChange);
    }


    public override void OnDisable()
    {
        base.OnDisable();
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            //방장일 때
            playersReady = new Dictionary<int, bool>();
        }
        else
        {
            //방장이 아닐 때
        }

        // 방장이 아니면 게임 시작 버튼 및 난이도 조절창을 비활성화
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffText.gameObject.SetActive(false == PhotonNetwork.IsMasterClient);

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);

            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }

        // 방에 입장했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void JoinPlayer(Player newPlayer)
    {
        //GameObject playerName = Instantiate(playerTextPrefab, playerList, false);
        //playerName.name = newPlayer.NickName;
        //playerName.GetComponent<Text>().text = newPlayer.NickName;

        var playerEntry = Instantiate(playerTextPrefab, playerList, false).GetComponent<PlayerEntry>();

        playerEntry.player = newPlayer;
        playerEntry.playerNameText.text = newPlayer.NickName;

        var toggle = playerEntry.readyToggle;

        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            // TODO: 내 엔트리일 경우에만 토글의 onValueChaged에 이벤트 핸들링.

            toggle.onValueChanged.AddListener(ReadyToggleClick);

        }
        else
        {
            //내가 아닌 다른 플레이어의 엔트리
            toggle.gameObject.SetActive(false);
        }

        playerEnrties[newPlayer.ActorNumber] = playerEntry;

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();
        }

        SortPlayers();
    }

    public void LeavePlayer(Player gonePlayer)
    {
        GameObject leaveTarget = playerEnrties[gonePlayer.ActorNumber].gameObject;

        playerEnrties.Remove(gonePlayer.ActorNumber);

        Destroy(leaveTarget);

        SortPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Remove(gonePlayer.ActorNumber);
            CheckReady();
        }

        //Destroy(playerList.Find(gonePlayer.NickName));
    }

    public void SortPlayers()
    {
        foreach (int actorNumber in playerEnrties.Keys)
        {
            playerEnrties[actorNumber].transform.SetSiblingIndex(actorNumber);
            //SetSiblingIndex => Hierachy 상 내 부모 안에서 다른 객체 중 순서를 지정하고 싶을 때 
            //setfirstsibling lastsibling 이런건 맨 앞으로 보내냐 맨 뒤로 보내냐도 있음
        }
    }

    private async void StartButtonClick()
    {
        // 게임 시작 버튼
        // 기존의 씬 로드 방식
        // SceneManager.LoadScene("GameScene");
        

        if (PhotonNetwork.IsMasterClient)
        {
            var eyesRef = HWFirebaseManager.Instance.usersRef.Child("eyes");

            var ey = PhotonNetwork.LocalPlayer.CustomProperties["Eyes"];

            await eyesRef.SetValueAsync((int)ey);

            HWFirebaseManager.Instance.userData.eyes = (int)ey;

            // Photon을 통해 플레이어들과 씬을 동기화 하여 로드
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
    private void CancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();

        // 시간 지연으로 인해 방을 퇴장하였는데 방장의 시작 콜에 의해 씬이 넘어가는 것을 방지
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    // 내 Ready 상태가 변경될 때 Custom Properties 변경
    public void ReadyToggleClick(bool isOn)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        //PhotonNetwork의 Custom Property는 HashTable 구조 활용
        // 그러나 dotnet의 HashTable이 아닌 간소화 형태의 HashTable 클래스를 직접 제공
        Hashtable customProps = localPlayer.CustomProperties;

        customProps["Ready"] = isOn;

        localPlayer.SetCustomProperties(customProps);


        Hashtable outfitCustomProps = localPlayer.CustomProperties;

        for (int i = 0; i < playerEnrties[localPlayer.ActorNumber].eyesToggle.Length; i++)
        {
            if (playerEnrties[localPlayer.ActorNumber].eyesToggle[i].isOn)
            {
                customProps["Eyes"] = i;
                break;
            }
        }

        localPlayer.SetCustomProperties(outfitCustomProps);

        foreach (var eye in playerEnrties[localPlayer.ActorNumber].eyesToggle)
        {
            eye.interactable = !isOn;
        }

    }

    // 다른 플레이어가 ReadyToggle을 변경했을 경우 내 클라이언트에도 반영
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        playerEnrties[actorNumber].readyLabel.gameObject.SetActive(isReady);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[actorNumber] = isReady;
            CheckReady();
        }
    }

    // 방장일 경우에 다른 플레이어들이 모두 ready 상태인지 확인하여 Start 버튼 활성화 여부 결정
    private void CheckReady()
    {
        // 여러 요소 중 한개라도 false 이면 false여야 할 때
        // 즉 모든 요소가 && 연산을 해야할 때.

        // linq에서 제공하는 All 함수는 모두가 true여야 true
        bool allReady = playersReady.Values.All(x => x);

        // linq에서 제공하는 Any 함수는 하나라도 true면 true
        bool anyReady = playersReady.Values.Any(x => x);


        // 5명의 플레이어 중 3번째 플레이어의 isReady가 false, 나머진 true => false
        //bool allReady = true; // 초기 상태는 true
        //foreach (bool isReady in playersReady.Values)
        //{
        //    if (isReady)
        //    {
        //        continue;
        //    }
        //    else
        //    {
        //        allReady = false;
        //        break;
        //    }
        //}

        startButton.interactable = allReady;
    }

    private void DifficultyValueChange(int value)
    {
        if (false == PhotonNetwork.IsMasterClient) return;

        var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Diff"] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //print($"커스텀프로퍼티 변경되었습니다. {PhotonNetwork.Time}");

        if (changedProps.ContainsKey("Ready"))
        {
            SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable props)
    {

        if (props.ContainsKey("Diff"))
        {
            print($"room difficulty changed : {props["Diff"]}");
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }
    }

    public override void OnJoinedRoom()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            print($"room difficulty changed : {props["Diff"]}");
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }
    }

    

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 방장이 나갔을 때 호출되므로, 방에 참가되어 있는 상태에서 방장의 역할을 수행할 수 있도록 유효성 검사 및 조치를 할 필요가 있음
        // 예) playersReady에 Dictionary 객체 생성 등등


        //newMasterClient = PhotonNetwork.MasterClient;
        //playerEnrties[newMasterClient.ActorNumber].transform.SetSiblingIndex(actorNumber);
    }

}