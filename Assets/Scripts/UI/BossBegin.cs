using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBegin : MonoBehaviour
{
    
    [SerializeField] private GameObject panel;//提示面板


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!BattleBegin.isBegun)//如果未开始boss战并进入检测区域
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
        
    }

    private void Update()
    {
        
    }

    
}
