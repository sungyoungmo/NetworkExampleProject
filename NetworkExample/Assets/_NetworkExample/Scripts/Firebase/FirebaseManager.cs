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

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseApp App { get; private set; }    // 파이어베이스 기본 앱(기본 기능들)
    public FirebaseAuth Auth { get; private set; }  // 인증 기능 전용

    public FirebaseDatabase DB { get; private set; }

    public bool IsInitialized { get; private set; } = false;    // 파이어베이스 앱이 초기화 되어 사용 가능한지 여부

    public event Action onInit;     //파이어베이스가 초기화되면 호출

    public UserData userData;

    public DatabaseReference usersRef;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializedAsync();
    }

    //async를 안쓸 경우
    private void Initialize()
    {
        // 파이어베이스 앱 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread
        (
            (Task<DependencyStatus> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"파이어베이스 초기화 실패 : {task.Status}");
                }
                else if (task.IsCompleted)
                {
                    print($"파이어베이스 초기화 성공 : {task.Status}");

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


    //async 키워드를 통해 비동기 프로그래밍
    private async void InitializedAsync()
    {
        //async 쓰면 task.Result까지 완료하고 가져오는거임
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            //파이어베이스 초기화 성공
            print("파이어베이스 초기화 성공");
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
            IsInitialized = true;
            onInit?.Invoke();
        }
        else
        {
            Debug.LogWarning($"파이어베이스 초기화 실패: {status}");
        }
    }

    public async void Login(string email, string pw, Action<FirebaseUser> callback = null)
    {
        var result = await Auth.SignInWithEmailAndPasswordAsync(email, pw);

        usersRef = DB.GetReference($"users/{result.User.UserId}");

        DataSnapshot userDataValues = await usersRef.GetValueAsync();

        if (userDataValues.Exists)
        {
            string json = userDataValues.GetRawJsonValue(); // 데이터 전체를 json으로 가져옴
            var address = userDataValues.Child("address");  // 데이터 하위 레퍼런스(데이터 스냅샷)을 가져옴.
            if (address.Exists) print($"주소 : {address.GetValue(false)}");

            userData = JsonConvert.DeserializeObject<UserData>(json);
            print(json);
        }
        else
        {
            FBPanelManager.Instance.Dialog("로그인 정보에 문제가 있습니다. \n 고객센터에 문의하세요.");
        }
        

        callback?.Invoke(result.User);
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

    // 한줄일 때 이렇게 생략 가능, 많이 쓰는 형태는 아님
    public void Logout() => Auth.SignOut();
}
