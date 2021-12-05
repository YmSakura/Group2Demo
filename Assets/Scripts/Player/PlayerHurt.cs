using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] private float healthTimer, healthTimerSet = 5, collapseTimer, collapseTimerSet;
    [SerializeField] private int healthSet = 120, healthIncrease = 1;
    public static int health;
    public GameObject boss,player,pumpkin;
    public GameObject DeathPanel;//死亡面板

    private Rigidbody2D rb;
    //public bool isHurt;

    // Start is called before the first frame update
    void Awake()
    {
        healthTimer = healthTimerSet;
        health = healthSet;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        /*if (PlayerMovement.rollLock)
        {
            if (isHurt)
            {
                health -= PlayerMovement.getDamage;
                PlayerMovement.anim.SetBool("isHurt", isHurt);
            }
        }
        else
        {
            
        }*/
        Curing();
        DeadJudge();
    }

    //血量回复
    void Curing()
    {
        if (health < healthSet)//血量回复计时器
        {
            if (healthTimer != 0)
            {
                healthTimer -= Time.deltaTime;
                if (healthTimer <= 0)
                {
                    healthTimer = healthTimerSet;
                    health += healthIncrease;
                }
            }
        }
    }

    //击退击飞
    public IEnumerator Repeled()
    {
        GameObject.Find("PLAYER0").GetComponent<PlayerMovement>().enabled = false;//禁止行动
        PlayerMovement.rb.velocity *= -1;//速度朝反方向击退
        Debug.Log("人物被击退");
        yield return new WaitForSeconds(0.3f);
        ResetMovement();
    }

    //眩晕
    public IEnumerator Collapsing(float CollapseTime)
    {
        rb.velocity=Vector2.zero;
        GameObject.Find("PLAYER0").GetComponent<PlayerMovement>().enabled = false;//禁止行动
        Debug.Log("人物被眩晕");
        yield return new WaitForSeconds(CollapseTime);
        ResetMovement();
    }

    //重置人物移动脚本
    private void ResetMovement()
    {
        Debug.Log("Recover");
        GameObject.Find("PLAYER0").GetComponent<PlayerMovement>().enabled = true;//恢复行动
    }

    //重置受伤状态
    /*public void RemoveHurt()
    {
        //isHurt = false;
        PlayerMovement.getDamage = 0;
        //PlayerMovement.anim.SetBool("isHurt", false);
    }*/

    //死亡条件判断
    void DeadJudge()
    {
        if (health <= 0)
        {
            PlayerMovement.anim.Play("die");
            //PlayerMovement.anim.SetBool("IsDie",true);
            boss.GetComponent<BossController>().enabled = false;
            player.GetComponent<PlayerMovement>().enabled = false;
            pumpkin.GetComponent<Pumpkin>().enabled = false;
            DeathPanel.SetActive(true);
        }
    }
}
