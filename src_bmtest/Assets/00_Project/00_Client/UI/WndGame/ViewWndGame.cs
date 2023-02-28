using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewWndGame : MonoBehaviour
{
    [Header("Ui:")]
    //Parent for prefabs list view
    public Transform UiContentBusinessList;
    public Transform UiPrefabViewBusiness;
    public TMP_Text TextWalletMoneyCount;

    [Header("Exit")]
    public GameObject PopupExit;
    public Button ButtonExit;
    public Button ButtonExitSave;
    public Button ButtonExitClean;
    public Button ButtonBack;
}
