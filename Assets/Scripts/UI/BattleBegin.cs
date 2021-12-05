using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBegin : MonoBehaviour
{
    public static bool isBegun = false;//是否已经开始boss战
    [SerializeField] private GameObject Entry;//入口墙体
    [SerializeField] private GameObject Boss;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&!isBegun)//按下E开始BOSS战
        {
            isBegun = true;//已经开始Boss战
            Entry.SetActive(true);//启用入口墙体，关闭入口
            Boss.GetComponent<Animator>().Play("start");//激活boss
            Boss.GetComponent<BossController>().OpenDirLight();
            Invoke("EnableBoss",4.5f);
            SoundManager.Sound.MusicPlay();
            gameObject.SetActive(false);//关闭提示面板
        }
    }
    private void EnableBoss()
    {
        Boss.GetComponent<BossController>().StartChase();
    }
}
