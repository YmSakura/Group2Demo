using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBegin : MonoBehaviour
{
    private bool isBegun = false;//是否已经开始boss战
    [SerializeField] private GameObject panel;//提示面板
    [SerializeField] private GameObject Entry;//入口墙体
    [SerializeField] private GameObject Boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isBegun)//如果未开始boss战并进入检测区域
        {
            panel.SetActive(true);//开启提示
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        panel.SetActive(false);//关闭提示
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.E)&&!isBegun)//按下E开始BOSS战
        {
            isBegun = true;//已经开始Boss战
            panel.SetActive(false);//关闭提示面板
            Entry.SetActive(true);//启用入口墙体，关闭入口
            Boss.GetComponent<Animator>().Play("start");//激活boss
            Boss.GetComponent<BossController>().OpenDirLight();
            Invoke("EnableBoss",4.5f);
        }
    }

    private void Update()
    {
        
    }

    private void EnableBoss()
    {
        Boss.GetComponent<BossController>().StartChase();
    }
}
