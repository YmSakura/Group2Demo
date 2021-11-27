using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("基本组件")]
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    private float inputX, inputY;

    [Header("人物设置")]
    [SerializeField] public float walkSpeed = 1.5f, runSpeed = 2.5f, rollSpeed = 0.5f;
    [SerializeField] public int healthSet = 120, stayingPowerSet = 100, defensivePower = 8; //生命， 耐力， 防御值
    private int health, stayingPower; //生命， 耐力， 防御值
    private bool isRunning; // 跑步行走动画切换条件

    [Header("计时器&耐力消耗&回复")]
    //分别生命恢复计时器，耐力回复计时器， 跑步计时器， 眩晕计时器
    [SerializeField] private float healthTimer, stayingPowerTimer, runTimer, collapseTimer; 
    //分别生命恢复间隔 5s ，耐力回复间隔， 跑步间隔 1s ， 眩晕间隔
    [SerializeField] private float healthTimerSet = 5, stayingPowerTimerSet = 1, runTimerSet = 1, collapseTimerSet; 
    public int runCost = 10, rollCost = 34, defensiveCost;
    public int healthIncrease = 1, stayingPowerIncrease = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //基本参数和计时器初始化
        health = healthSet;
        stayingPower = stayingPowerSet;
        healthTimer = healthTimerSet;
        runTimer = runTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
    }

    //击退击飞
    public void Repeled(float x, float y)
    {
        Debug.Log("人物被击退");
    }

    //眩晕
    public void  Collapsing(float CollapseTime)
    {
        Debug.Log("人物被眩晕");
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

    //人物移动
    void Moving()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(inputX, inputY).normalized;//标准化向量长度
        //跑步
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stayingPower >= runCost)
            {
            isRunning = true;
            rb.velocity = moveInput * runSpeed;
                if (runTimer != 0)
                {
                    runTimer -= Time.deltaTime;
                    if (runTimer <= 0)
                    {
                        runTimer = runTimerSet;
                        stayingPower -= runCost;
                    }
                }
            }
        }
        //翻滚
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            if (stayingPower >= rollCost)
            {
            coll.enabled = false;
            stayingPower -= runCost;
            rb.velocity = moveInput * rollSpeed;
            anim.SetBool("isRolling", true);
            }
        }
        //行走
        else
        {
            isRunning = false;
            rb.velocity = moveInput * walkSpeed;
            if (stayingPower < stayingPowerSet)
            {
                if (stayingPowerTimer != 0)
                {
                    stayingPowerTimer -= Time.deltaTime;
                    if (stayingPowerTimer <= 0)
                    {
                        stayingPowerTimer = stayingPowerTimerSet;
                        if((stayingPowerSet - stayingPower) < stayingPowerIncrease)
                        {
                            stayingPower = stayingPowerSet;
                        }
                        else
                        {
                            stayingPower += stayingPowerIncrease;
                        }
                    }
                }
            }
        }
    }
    //结束翻滚（动画事件）
    void EndRolling()
    {
        coll.enabled = true;
        anim.SetBool("isRolling", false);
    }


}
