using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime : MonoBehaviour
{
    public float attackTimer, attackTimerSet = 0.7f;
    public float thirdTimer, thirdTimerSet = 0.15f;//第三阶段硬直
    public bool stateLock;//阶段转换锁，开启时可以转阶段

    // Start is called before the first frame update
    private void OnEnable()//每当启用连击次数判断时重置条件
    {
        stateLock = false;
        attackTimer = attackTimerSet;
        thirdTimer = thirdTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        AttackCheck();
    }
    void AttackCheck()
    {
        if (PlayerMovement.endurance >= 15)
        {
            attackTimer = attackTimerSet;
            if (attackTimer > 0)
            {
                if (Input.GetMouseButtonDown(0))//每个阶段读取一次攻击输入
                {

                    if (PlayerMovement.attackTime < 3)
                    {
                        stateLock = true;//小于3阶段时，阶段锁开
                    }

                    if (PlayerMovement.attackTime == 3)
                    {
                        thirdTimer -= Time.deltaTime;//开启第三阶段硬直计时
                        PlayerMovement.rb.velocity = Vector2.zero;
                        if (thirdTimer <= 0)
                        {
                            PlayerMovement.anim.SetBool("AttackPause", true);
                            this.enabled = false;//禁用该脚本
                        }
                    }
                    else
                    {
                        this.enabled = false;//禁用该脚本
                    }
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
            }
            else
            {
                //PlayerMovement.anim.SetBool("AttackPause", true);
                this.enabled = false;
            }
        }
        /* else
         {
             stateLock = false;
         }*/
    }
}
