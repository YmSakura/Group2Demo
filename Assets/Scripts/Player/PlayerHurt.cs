using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] private float healthTimer, healthTimerSet = 5, collapseTimer, collapseTimerSet;
    [SerializeField] public int healthSet = 120, healthIncrease = 1;
    public static int health;
    public bool isHurt;

    // Start is called before the first frame update
    void Awake()
    {
        healthTimer = healthTimerSet;
        health = healthSet;
    }

    private void Update()
    {
        if (!PlayerMovement.rollLock)
        {
            if (isHurt)
            {
                health -= PlayerMovement.getDamage;
                PlayerMovement.anim.SetBool("isHurt", isHurt);
            }
        }
        else
        {
            Curing();
        }
    }

    //血量回复
    void Curing()
    {
        if (health < healthSet)
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
    public void Repeled(float x, float y)
    {
        Debug.Log("人物被击退");
    }

    //眩晕
    public IEnumerator Collapsing(float CollapseTime)
    {
        GameObject.Find("PLAYER0").GetComponent<PlayerMovement>().enabled = false;
        Debug.Log("人物被眩晕");
        yield return new WaitForSeconds(CollapseTime);
        ResetMovement();
    }

    //重置人物移动脚本
    private void ResetMovement()
    {
        GameObject.Find("PLAYER0").GetComponent<PlayerMovement>().enabled = true;
    }

    //重置受伤状态
    public void RemoveHurt()
    {
        isHurt = false;
        PlayerMovement.anim.SetBool("isHurt", isHurt);
    }
}
