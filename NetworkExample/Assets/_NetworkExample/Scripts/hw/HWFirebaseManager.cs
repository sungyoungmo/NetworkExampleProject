using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using Firebase.Auth;
using System;
using Firebase.Database;
using Newtonsoft.Json;
using Photon.Pun;

public class HWFirebaseManager : MonoBehaviour
{
    public static HWFirebaseManager Instance { get; private set; }

    public FirebaseApp App { get; private set; }    // ���̾�̽� �⺻ ��(�⺻ ��ɵ�)
    public FirebaseAuth Auth { get; private set; }  // ���� ��� ����

    public FirebaseDatabase DB { get; private set; }

    public bool IsInitialized { get; private set; } = false;    // ���̾�̽� ���� �ʱ�ȭ �Ǿ� ��� �������� ����

    public event Action onInit;     //���̾�̽��� �ʱ�ȭ�Ǹ� ȣ��

    public event Action<FirebaseUser> onLogin;  //�α��� �Ǵ� ȸ������ �Ŀ� ȣ��

    public event Action<string> onReceiveMessage;   //���� ������ �޽����� ������ ȣ��

    public UserData userData;

    public DatabaseReference usersRef;

    private void Awake()
    {
        Instance = this;
        onLogin += OnLogin;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializedAsync();
    }

    // �α����� �Ǿ��� �� ȣ��Ǿ��� ������
    private void OnLogin(FirebaseUser user)
    {
        var msgRef = DB.GetReference($"msg/{user.UserId}");
        msgRef.ChildAdded += RecvMsgEventHandler;
    }

    //async�� �Ⱦ� ���
    private void Initialize()
    {
        // ���̾�̽� �� �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread
        (
            (Task<DependencyStatus> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"���̾�̽� �ʱ�ȭ ���� : {task.Status}");
                }
                else if (task.IsCompleted)
                {
                    print($"���̾�̽� �ʱ�ȭ ���� : {task.Status}");

                    if (task.Result == DependencyStatus.Available)
                    {
                        App = FirebaseApp.DefaultInstance;
                        Auth = FirebaseAuth.DefaultInstance;
                        IsInitialized = true;
                    }
                }
            }
        );
    }


    //async Ű���带 ���� �񵿱� ���α׷���
    private async void InitializedAsync()
    {
        //async ���� task.Result���� �Ϸ��ϰ� �������°���
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            //���̾�̽� �ʱ�ȭ ����
            print("���̾�̽� �ʱ�ȭ ����");
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
            IsInitialized = true;
            onInit?.Invoke();
        }
        else
        {
            Debug.LogWarning($"���̾�̽� �ʱ�ȭ ����: {status}");
        }
    }

    public async void Login(string email, string pw, Action<FirebaseUser> callback = null)
    {
        var result = await Auth.SignInWithEmailAndPasswordAsync(email, pw);

        usersRef = DB.GetReference($"users/{result.User.UserId}");

        DataSnapshot userDataValues = await usersRef.GetValueAsync();

        if (userDataValues.Exists)
        {
            string json = userDataValues.GetRawJsonValue(); // ������ ��ü�� json���� ������

            var address = userDataValues.Child("address");  // ������ ���� ���۷���(������ ������)�� ������.

            if (address.Exists) print($"�ּ� : {address.GetValue(false)}");

            userData = JsonConvert.DeserializeObject<UserData>(json);

            print(json);

            
        }
        else
        {
            FBPanelManager.Instance.Dialog("�α��� ������ ������ �ֽ��ϴ�. \n �����Ϳ� �����ϼ���.");
        }

        onLogin?.Invoke(result.User);

        callback?.Invoke(result.User);

        PhotonNetwork.ConnectUsingSettings();
    }

    public async void Create(string email, string pw, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, pw);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            UserData userData = new UserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            this.userData = userData;

            onLogin?.Invoke(result.User);

            callback?.Invoke(Auth.CurrentUser);
        }
        catch (FirebaseException fe)
        {

            Debug.LogError(fe.Message);
        }
    }

    public async void UpdateUser(string name, string pw, Action callback = null)
    {
        var profile = new UserProfile()
        {
            DisplayName = name
        };

        await Auth.CurrentUser.UpdateUserProfileAsync(profile);

        if (false == string.IsNullOrEmpty(pw))
        {
            await Auth.CurrentUser.UpdatePasswordAsync(pw);
        }

        callback?.Invoke();
    }

    // ������ �� �̷��� ���� ����, ���� ���� ���´� �ƴ�
    public void Logout() => Auth.SignOut();

    //DB�� Transaction�� ������ �Լ���
    public async void UpdateCharacterClass(UserData.Class cl, Action callback = null)
    {
        // ĳ���� Ŭ���� �׸��� ������ ���۷���

        string refKey = //"characterClass";
            nameof(UserData.characterClass);

        //var classRef = usersRef.Child("characterClass"); �̰� ���
        var classRef = usersRef.Child(refKey);  //�̷��� ���� 

        await classRef.SetValueAsync((int)cl);

        userData.characterClass = cl;

        callback?.Invoke();
    }

    public async void UpdateCharacterLevel(Action callback = null)
    {
        int level = userData.level + 1;

        string refKey = nameof(UserData.level);

        var levelRef = usersRef.Child(refKey);

        await levelRef.SetValueAsync(level);

        userData.level = level;

        callback?.Invoke();
    }

    public async void UpdateCharacterAddress(string address, Action callback = null)
    {
        string refKey = nameof(UserData.address);

        var addrRef = usersRef.Child(refKey);

        await addrRef.SetValueAsync(address);

        userData.address = address;

        callback?.Invoke();
    }

    public async void UpdateUserData(string childName, object value, Action<object> callback = null)
    {
        var targetRef = usersRef.Child(childName);
        //TODO: �ߺ����� �����
    }

    //TODO: DB�� ���� �޽����� �ְ� �޴� �Լ�
    // �޽����� ���� ��
    public void SendMsg(string receiver, Message msg)
    {
        var msgRef = DB.GetReference($"msg/{receiver}");

        var msgJson = JsonConvert.SerializeObject(msg);

        msgRef.Child(msg.sender + msg.sendTime).SetRawJsonValueAsync(msgJson);
    }

    //�޽��� ���� ��
    public void RecvMsgEventHandler(object sender/*�̺�Ʈ�� ȣ���� ��ü�� ���� Key ������ ���� ��ü*/, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError);
            return;
        }
        else
        {
            var rawJson = args.Snapshot.GetRawJsonValue();

            Message msg = JsonConvert.DeserializeObject<Message>(rawJson);

            string msgString = $" ������ : {msg.sender}\n ���� : {msg.message}\n ���� �ð� : {msg.GetSendTime()}";
            onReceiveMessage?.Invoke(msgString);
        }
    }

}
