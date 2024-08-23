using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBGamePanelManager : MonoBehaviour
{
    public Text titleText;
    public Text nameText;
    public Text classText;
    public Text levelText;
    public Text addrText;

    public Dropdown classDropdown;
    public Button levelupButton;
    public InputField addrInput;
    public Button sendMessageButton;

    public FBReceiveMessagePopup rPopup;
    public FBSendMessagePopup sPopup;

    private void Awake()
    {
        var options = new List<Dropdown.OptionData>();  // 옵션리스트로 사용할 list 인스턴스 생성

        foreach (string clNames in Enum.GetNames(typeof(UserData.Class)))
        {
            options.Add(new Dropdown.OptionData(clNames));
        }

        classDropdown.options = options;

        classDropdown.onValueChanged.AddListener(ClassDropDownChange);
        levelupButton.onClick.AddListener(LevelUpButtonClick);
        
        // onSubmit : PC 환경에서 Enter 키를 누를 때, 모바일 환경에서는 가상 키보드의 완료 버튼을 누를 때
        // OnEndEdit : onSubmit을 포함하여, PC 환경에서 다른 곳을 클릭하는 등 포커스를 잃을 때, 모바일 환경에서 다른 곳을 터치하여 가상 키보드가 사라질 떄 등등
        addrInput.onSubmit.AddListener(AddressInputChange);
        sendMessageButton.onClick.AddListener(SendMessageButtonClick);
    }

    private void Start()
    {
        SetUserData(FirebaseManager.Instance.userData);

        FirebaseManager.Instance.onReceiveMessage += rPopup.OnReceiveMessage;
    }

    public void SetUserData(UserData data)
    {
        titleText.text = $"안녕하세요, {data.userName}";
        nameText.text = data.userName;
        classText.text = data.characterClass.ToString();
        levelText.text = data.level.ToString();
        addrText.text = data.address;
    }

    public void ClassDropDownChange(int value)
    {
        //TODO: 드롭다운으로 직업을 교체할 경우

        FirebaseManager.Instance.UpdateCharacterClass(
        (
            UserData.Class)value,
            () => 
            {
                SetUserData(FirebaseManager.Instance.userData);
            }
        );

    }

    public void LevelUpButtonClick()
    {
        //TODO: 레벨업 버튼을 클릭할 경우

        FirebaseManager.Instance.UpdateCharacterLevel(
            () => SetUserData(FirebaseManager.Instance.userData)
            );
        
    }
    public void AddressInputChange(string value)
    {
        //TODO: address에 값을 입력 후 enter 등을 누를 경우

        FirebaseManager.Instance.UpdateCharacterAddress
        (   
            value,
            () =>
            { 
                SetUserData(FirebaseManager.Instance.userData); 
            }
        );
    }

    public void SendMessageButtonClick()
    {
        sPopup.gameObject.SetActive(true);
    }
}
