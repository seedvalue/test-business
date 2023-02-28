
using UnityEngine;
[CreateAssetMenu(menuName = "Create business config")]

public class BusinessCfg : ScriptableObject
{
    //Уникальный ID, для сохранения состояния бизнеса
    //Можно было и по имени, но если имя сменится,тогда сохранение не подтянется
    public int ID;
    //В конфиг можно добавить много бизнесов,
    //но можно отключать включать на выбор
    public bool IsShowInList;
    //При старте игры игрок имеет 1 бизнес,
    //но галочкой можно изменить стартовое состояние,
    //например иметь 2 бизнеса по необхожимости со старта.
    public bool IsOwnAtStartGame;
    [Header("Business")]
    public string BusinessName;
    public float EarnDelay;
    public int BasePrice;
    public int BaseEarning;
    
    [Header("Upgrade 1")]
    public string UpgradeFirstName;
    public int UpgradeFirstPrice;
    public int UpgradeFirstEarnМultiplier;
    
    [Header("Upgrade 2")]
    public string UpgradeSecondName;
    public int UpgradeSecondPrice;
    public int UpgradeSecondEarnМultiplier;
}
