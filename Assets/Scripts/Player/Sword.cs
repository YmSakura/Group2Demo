using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Collider2D swordColl;
    public static bool succeed, hurtAble;//判断boss是否处于伤害判区， 能否造成伤害
    private Collider2D BossColl;

    // Update is called once per frame
    void Update()
    {
        if (succeed && hurtAble) //防update止多次伤害，每次造成伤害后，将可造成伤害状态关闭
        {
            if (PlayerMovement.attackTime == 1)
            {
                BossColl.GetComponent<BossController>().BeAttacked(12);
                hurtAble = false;
            }
            else if (PlayerMovement.attackTime == 2)
            {
                BossColl.GetComponent<BossController>().BeAttacked(16);
                hurtAble = false;
            }
            else if (PlayerMovement.attackTime == 3)
            {
                BossColl.GetComponent<BossController>().BeAttacked(22);
                hurtAble = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)//接触到boss时伤害判定打开
    {
        if (collision.tag == "Boss")
        {
            Debug.Log("Succeed");
            BossColl = collision;//获取boss的触发器
            succeed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)//离开boss时伤害判定关闭
    {
        if (collision.tag == "Boss")
        {
            succeed = false;
        }
    }

    public void HurtAble()//开启可伤害状态，在攻击动画开时时调用
    {
        hurtAble = true;
        Debug.Log(hurtAble);
    }
}
