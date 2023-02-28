
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewUiOneBusiness : MonoBehaviour
{
    [Header("Business")]
    public TMP_Text TextBusinessName;
    public Slider SliderProgressEarning;

    public TMP_Text TextLevelVal;
    public TMP_Text TextEarnVal;

    //Button level up
    public TMP_Text TextLevelUpPrice;

    [Header("Upgrade 1")]
    public TMP_Text TextUpgradeFirstName;
    public TMP_Text TextUpgradeFirstEarnÌultiplie;
    public TMP_Text TextUpgradeFirstPriceVal;

    [Header("Upgrade 2")]
    public TMP_Text TextUpgradeSecondName;
    public TMP_Text TextUpgradeSecondEarnÌultiplie;
    public TMP_Text TextUpgradeSecondPriceVal;

    [Header("Buttons")]
    public Button ButtonLevelUp;
    public Button ButtonUpgradeFirst;
    public Button ButtonUpgradeSecond;
}
