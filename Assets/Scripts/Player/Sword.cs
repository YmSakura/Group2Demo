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
        if (succeed && hurtAble) //防止多次伤害
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boss")
        {
            Debug.Log("Succeed");
            BossColl = collision;
            succeed = true;
            /*if (PlayerMovement.attackTime == 1)
            {
                collision.GetComponent<BossController>().BeAttacked(12);
            }
            else if (PlayerMovement.attackTime == 2)
            {
                collision.GetComponent<BossController>().BeAttacked(16);
            }
            else if (PlayerMovement.attackTime == 3)
            {
                collision.GetComponent<BossController>().BeAttacked(22);
            }*/
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Boss")
        {
            succeed = false;
        }
    }

    public void HurtAble()
    {
        hurtAble = true;
        Debug.Log(hurtAble);
    }
}
