using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] private float healthTimer, healthTimerSet = 5, collapseTimer, collapseTimerSet;
    [SerializeField] public int healthSet = 120, healthIncrease = 1;
    public static int health;

    // Start is called before the first frame update
    void Awake()
    {
        healthTimer = healthTimerSet;
        health = healthSet;
    }

    private void Update()
    {
        Curing();
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
    public void Collapsing(float CollapseTime)
    {
        Debug.Log("人物被眩晕");
    }

}
