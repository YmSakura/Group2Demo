using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("基本组件")]
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    private float inputX, inputY;
    private float moveSpeed;


    [Header("人物设置")]
    [SerializeField] public float walkSpeed = 1.5f, runSpeed = 2.5f, rollSpeed = 0.5f, defendSpeed = 0.5f;
    [SerializeField] public int stayingPowerSet = 100, defendPower = 8; // 耐力， 防御值
    public int damage;
    private int stayingPower; // 耐力， 防御值
    public bool isHurt;
    private bool rollLock, shieldState;//翻滚锁定, 举盾状态

    [Header("计时器&耐力消耗&回复")]
    //分别 耐力回复计时器， 跑步计时器
    [SerializeField] private float stayingPowerTimer, runTimer, defendTimer;
    //分别 耐力回复间隔， 跑步间隔 0.1s
    [SerializeField] private float stayingPowerTimerSet = 1, runTimerSet = 0.1f;
    public int runCost = 1, rollCost = 34, defendCost;
    public int stayingPowerIncrease = 20;

    [Header("外部数据测试")]
    public int getDamage = 1;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //基本参数和计时器初始化
        stayingPower = stayingPowerSet;
        runTimer = runTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        DefendingAnim();
    }


    //人物移动
    void Moving()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(inputX, inputY).normalized;//标准化向量长度

        //行动
        if (!rollLock && (inputX != 0 || inputY != 0))
        {
            //跑步
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (stayingPower >= runCost)
                {
                    rb.velocity = moveInput * runSpeed;
                    moveSpeed = System.Math.Abs(rb.velocity.x) > System.Math.Abs(rb.velocity.y) ? System.Math.Abs(rb.velocity.x) : System.Math.Abs(rb.velocity.y);
                    anim.SetFloat("moveSpeed", moveSpeed);
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

            //行走
            else
            {
                rb.velocity = moveInput * walkSpeed;
                moveSpeed = System.Math.Abs(rb.velocity.x) > System.Math.Abs(rb.velocity.y) ? System.Math.Abs(rb.velocity.x) : System.Math.Abs(rb.velocity.y);
                anim.SetFloat("moveSpeed", moveSpeed);
                if (stayingPower < stayingPowerSet)
                {
                    if (stayingPowerTimer != 0)
                    {
                        stayingPowerTimer -= Time.deltaTime;
                        if (stayingPowerTimer <= 0)
                        {
                            stayingPowerTimer = stayingPowerTimerSet;
                            if ((stayingPowerSet - stayingPower) < stayingPowerIncrease)
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
            anim.SetBool("isMoving", true);
            anim.SetBool("isIdling", false);
        }

        //站立
        else
        {
            rb.velocity = Vector2.zero;
            moveSpeed = 0;
            anim.SetFloat("moveSpeed", moveSpeed);
            anim.SetBool("isMoving", false);
            anim.SetBool("isIdling", true);
        }

        //翻滚
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stayingPower >= rollCost)
            {
                rollLock = true;
                anim.SetBool("rollLock", true);
                coll.enabled = false;
                stayingPower -= runCost;
                rb.velocity = moveInput * rollSpeed;
                anim.SetBool("isRolling", true);
            }
        }

        //防御
        Defending(moveInput, getDamage);    //缺少目标的受到的伤害数值；


    }

    //结束翻滚（动画事件）
    void EndRolling()
    {
        rollLock = false;
        coll.enabled = true;
        anim.SetBool("rollLock", false);
    }

    //普通防御（动画+动画事件）
    void DefendingAnim()
    {
        if (stayingPower > 0 && Input.GetMouseButton(1))
        {
            shieldState = true;
            anim.SetBool("shieldState", shieldState);

        }
        else if (stayingPower <= 0 || !Input. GetMouseButton(1))
        {
            shieldState = false;
            anim.SetBool("shieldState", shieldState);
        }
    }
    void Defending(Vector2 moveInput, int getDamage)
    {
        if (!isHurt)
        {
            rb.velocity = defendSpeed * moveInput;
        }
        else
        {
            int realDamage = 0;
            rb.velocity = -walkSpeed * moveInput;
            if (stayingPower < (getDamage - defendPower))
            {
                stayingPower = 0;
                realDamage = getDamage;
            }
            else
            {
                stayingPower -= getDamage - defendPower;
            }
            PlayerHurt.health -= realDamage;
        }
    }


}