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
    public int eyes;

    public UserData() { }   //Json�� ������ȭ�Ͽ� ��ü�� �����ϱ� ���� �ʿ��� �⺻ ������.

    public UserData(string userId, string userName, int level, Class characterClass, string address, int eyes)
    {
        this.userId = userId;
        this.userName = userName;
        this.level = level;
        this.characterClass = characterClass;
        this.address = address;
        this.eyes = eyes;
    }

    public UserData(string userId)
    {
        this.userId = userId;
        userName = "������ ����";
        level = 1;
        characterClass = Class.Warrior;
        address = "None";
        eyes = 0;
    }
}

[Serializable]
public class Message
{
    public string sender;
    public string message;
    public long sendTime;

    // ������Ƽ���� �ø��������Ǿ
    public DateTime GetSendTime() { return new DateTime(sendTime); }

}