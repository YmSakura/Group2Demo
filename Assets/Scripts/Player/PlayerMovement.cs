using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("基本组件")]
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    private float HPPercent, endurancePercent;

    private float inputX, inputY;
    private float moveSpeed;
    private GameObject sword;
    [SerializeField] private int attackTime = 0;
    [SerializeField] private bool attackPause;
    [SerializeField] private float attackTimer, attackTimerSet = 0.6f;
    private Vector2 moveInput;


    [Header("人物设置")]
    [SerializeField] public float walkSpeed = 50, runSpeed = 100, rollSpeed = 0.5f, defendSpeed = 20;
    [SerializeField] public int enduranceSet = 100, defendPower = 8; // 耐力， 防御值
    public int damage;
    public static int endurance; // 耐力
    public static bool isHurt;
    public static bool rollLock, shieldState;//翻滚锁定, 举盾状态

    [Header("计时器&耐力消耗&回复")]
    //分别 耐力回复计时器， 跑步计时器
    [SerializeField] private float enduranceTimer, runTimer, defendTimer;
    //分别 耐力回复间隔， 跑步间隔 0.1s
    [SerializeField] private float enduranceTimerSet = 1, runTimerSet = 0.1f;
    public int runCost = 1, rollCost = 34, defendCost;
    public int enduranceIncrease = 20;

    [Header("外部数据测试")]
    public static int getDamage = 1;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //
        //sword = GameObject.Find("player0/bone_1/bone_2/bone_7/bone_8/bone_9/bone_10");
        //基本参数和计时器初始化
        endurance = enduranceSet;
        runTimer = runTimerSet;
        attackTimer = attackTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        DefendingAnim();

        AttackCheck();
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
            if (inputX != 0 && inputX != transform.localScale.x)
            {
                //float playerX = transform.position.x;
                /*if (inputX == 1)
                {
                    transform.position = new Vector2(0, transform.position.y);
                }*/
                /*else if(inputX == -1)
                {
                    transform.position = new Vector2(-34.7, transform.position.y);
                }*/
                //transform.localScale = new Vector3(inputX, 1, 1);

                /*if (inputX == 1)
                {
                    transform.position = new Vector2(transform.position.x + 2 * playerX, transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(transform.position.x - 2 * playerX, transform.position.y);
                }*/
            }

            //跑步
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (endurance >= runCost)
                {
                    rb.velocity = moveInput * runSpeed;
                    anim.SetInteger("moveSpeed", 2);
                    if (runTimer != 0)
                    {
                        runTimer -= Time.deltaTime;
                        if (runTimer <= 0)
                        {
                            runTimer = runTimerSet;
                            endurance -= runCost;
                        }
                    }
                }
            }

            //行走
            else
            {
                rb.velocity = moveInput * walkSpeed;
                anim.SetInteger("moveSpeed", 1);
                if (endurance < enduranceSet)
                {
                    if (enduranceTimer != 0)
                    {
                        enduranceTimer -= Time.deltaTime;
                        if (enduranceTimer <= 0)
                        {
                            enduranceTimer = enduranceTimerSet;
                            if ((enduranceSet - endurance) < enduranceIncrease)
                            {
                                endurance = enduranceSet;
                            }
                            else
                            {
                                endurance += enduranceIncrease;
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
            anim.SetInteger("moveSpeed", 0);
            anim.SetBool("isMoving", false);
            anim.SetBool("isIdling", true);
            if (endurance < enduranceSet)
            {
                if (enduranceTimer != 0)
                {
                    enduranceTimer -= Time.deltaTime;
                    if (enduranceTimer <= 0)
                    {
                        enduranceTimer = enduranceTimerSet;
                        if ((enduranceSet - endurance) < enduranceIncrease)
                        {
                            endurance = enduranceSet;
                        }
                        else
                        {
                            endurance += enduranceIncrease;
                        }
                    }
                }
            }
        }

        //翻滚
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (endurance >= rollCost)
            {
                rollLock = true;
                anim.SetBool("rollLock", true);
                coll.enabled = false;
                endurance -= rollCost;
                rb.velocity = moveInput * rollSpeed;
            }
        }

        //防御
        Defending(moveInput, getDamage);    //缺少目标的受到的伤害数值；


    }

    //结束翻滚（动画事件）
    void EndRolling()
    {
        rb.velocity = Vector2.zero;
        rollLock = false;
        coll.enabled = true;
        anim.SetBool("rollLock", false);
    }

    //普通防御（动画+动画事件）
    void DefendingAnim()
    {
        if (endurance > 0 && Input.GetMouseButton(1))
        {
            shieldState = true;
            anim.SetBool("shieldState", shieldState);

        }
        else if (endurance <= 0 || !Input.GetMouseButton(1))
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
            if (endurance < (getDamage - defendPower))
            {
                endurance = 0;
                realDamage = getDamage;
            }
            else
            {
                endurance -= getDamage - defendPower;
            }
            PlayerHurt.health -= realDamage;
        }
    }

    //攻击（仅有动画）
    void AttackCheck()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0) /*&& !attackPause*/)
            {
                attackTime++;
                if (attackTime <= 3)
                {
                    DoAttack();
                    attackTimer = attackTimerSet;
                }
                else if (attackTime > 3)
                {
                    /*attackPause = true;
                    anim.SetBool("AttackPause", attackPause);
                    ResetAttack();*/
                    attackTimer = 0;
                }
            }
        }
        else
        {
            if (attackTime > 0)
            {
                attackPause = true;
                anim.SetBool("AttackPause", attackPause);
                attackTime = 0;
                anim.SetInteger("AttackState", attackTime);
            }
            attackTimer = attackTimerSet;
        }
    }
    void DoAttack()
    {
        if (attackTime == 1)
        {
            anim.SetInteger("AttackState", attackTime);
        }
        else if (attackTime == 2)
        {
            anim.SetInteger("AttackState", attackTime);
        }
        else if (attackTime == 3)
        {
            anim.SetInteger("AttackState", attackTime);
        }
        /*else
        {
            anim.SetBool("AttackPause", true);
        }*/
    }
    void ResetAttack()
    {
        attackTime = 0;
        anim.SetInteger("AttackState", attackTime);
        attackTimer = attackTimerSet;
        attackPause = false;
        anim.SetBool("AttackPause", attackPause);
    }

}
