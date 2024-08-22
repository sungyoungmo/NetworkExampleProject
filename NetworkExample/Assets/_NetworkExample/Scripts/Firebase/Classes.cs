using System;

[Serializable]
public class UserData
{
    public enum Class   //����
    {
        Warrior,
        Wizard,
        Rogue,
        Archer
    }

    public string userId;
    public string userName;
    public int level;
    public Class characterClass;    //json�� ���� �����͸� �ְ� ���� �� intager�� ĳ������ �ؼ� Ȱ��
    public string address;

    public UserData() { }   //Json�� ������ȭ�Ͽ� ��ü�� �����ϱ� ���� �ʿ��� �⺻ ������.

    public UserData(string userId, string userName, int level, Class characterClass, string address)
    {
        this.userId = userId;
        this.userName = userName;
        this.level = level;
        this.characterClass = characterClass;
        this.address = address;
    }

    public UserData(string userId)
    {
        this.userId = userId;
        userName = "������ ����";
        level = 1;
        characterClass = Class.Warrior;
        address = "None";
    }
}

[Serializable]
public class Message
{
    public string sender;
    public string message;
    public long sendTime;

    public DateTime SendTime { get => new DateTime(sendTime); }

}