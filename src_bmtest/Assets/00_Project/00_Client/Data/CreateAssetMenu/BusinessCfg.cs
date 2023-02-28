
using UnityEngine;
[CreateAssetMenu(menuName = "Create business config")]

public class BusinessCfg : ScriptableObject
{
    //���������� ID, ��� ���������� ��������� �������
    //����� ���� � �� �����, �� ���� ��� ��������,����� ���������� �� ����������
    public int ID;
    //� ������ ����� �������� ����� ��������,
    //�� ����� ��������� �������� �� �����
    public bool IsShowInList;
    //��� ������ ���� ����� ����� 1 ������,
    //�� �������� ����� �������� ��������� ���������,
    //�������� ����� 2 ������� �� ������������� �� ������.
    public bool IsOwnAtStartGame;
    [Header("Business")]
    public string BusinessName;
    public float EarnDelay;
    public int BasePrice;
    public int BaseEarning;
    
    [Header("Upgrade 1")]
    public string UpgradeFirstName;
    public int UpgradeFirstPrice;
    public int UpgradeFirstEarn�ultiplier;
    
    [Header("Upgrade 2")]
    public string UpgradeSecondName;
    public int UpgradeSecondPrice;
    public int UpgradeSecondEarn�ultiplier;
}
