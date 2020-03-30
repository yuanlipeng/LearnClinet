using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public Button btnOpen1;
    public Button btnOpen2;
    public Button btnOpen3;

    public Button btnClose1;
    public Button btnClose2;
    public Button btnClose3;

    void Start()
    {
        ViewSetting.Init();
        ViewMgr.Instance.Init();
        BattleMgr.Instance.Open();

        btnOpen1.onClick.AddListener(OnClickOpenBtn1);
        btnOpen2.onClick.AddListener(OnClickOpenBtn2);
        btnOpen3.onClick.AddListener(OnClickOpenBtn3);

        btnClose1.onClick.AddListener(OnClickCloseBtn1);
        btnClose2.onClick.AddListener(OnClickCloseBtn2);
        btnClose3.onClick.AddListener(OnClickCloseBtn3);
    }

    private void OnClickOpenBtn1()
    {
        ViewMgr.Instance.Open("View_1");
    }

    private void OnClickOpenBtn2()
    {
        ViewMgr.Instance.Open("View_2");
    }

    private void OnClickOpenBtn3()
    {
        ViewMgr.Instance.Open("View_3");
    }

    private void OnClickCloseBtn1()
    {
        ViewMgr.Instance.Close("View_1");
    }

    private void OnClickCloseBtn2()
    {
        ViewMgr.Instance.Close("View_2");
    }

    private void OnClickCloseBtn3()
    {
        ViewMgr.Instance.Close("View_3");
    }
}
