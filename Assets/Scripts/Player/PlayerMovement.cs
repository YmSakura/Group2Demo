using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static int attackTime = 0;                                                                   //攻击阶段
    private bool attackPause;                                                                              //是否要停止攻击判断
    private float attackTimer, attackTimerSet = 1.0f;                                       //攻击计时器
    public GameObject sword;                                                                            //获取到剑的对象
    public int damage;                                                                                          //造成的伤害

    [Header("基本组件")]
    public static Rigidbody2D rb;
    private Collider2D coll;
    public static Animator anim;

    private float inputX, inputY;                                                                           //获取玩家输入的XY方向
    private Vector2 moveInput;                                                                          //玩家输入的XY单位向量
    public float walkSpeed = 5, runSpeed = 10;                                                //走，跑 移速
    public int runCost = 1;                                                                                     //跑步耐力消耗
    private float runTimer, runTimerSet = 0.1f;                                                 //跑步耐力消耗计时器


    public int enduranceSet = 100;                                                                      //耐力上限
     public static int endurance;                                                                           // 耐力值
    private bool enduranceStop;                                                                         //耐力回复状态判断
    private float enduranceTimer, enduranceTimerSet = 1;                                //耐力恢复计时器
    private float enduranceCD, enduranceCDSet = 3.0f;                                       //耐力CD计时器
    public int enduranceIncrease = 20;                                                                  //耐力回复量

    public static bool rollLock;                                                                            //翻滚锁定
    public float rollSpeed = 0.5f;                                                                          //翻滚移速
    public int rollCost = 34;                                                                                   //翻滚耐力消耗

    public float defendSpeed = 20;                                                                      //防御移速
    public int defendPower = 8;                                                                             // 防御值
    public static bool isHurt;                                                                              //受伤状态判断
    public static bool shieldState;                                                                         //举盾状态
    public int defendCost;                                                                                      //防御耐力消耗

    private float spikeTimer = 0.0f;                                                                    //突刺计时器

    [Header("外部数据测试")]
    public static int getDamage = 1;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //
        //基本参数和计时器初始化
        endurance = enduranceSet;
        enduranceTimer = enduranceTimerSet;
        runTimer = runTimerSet;
        attackTimer = attackTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        DefendingAnim();

        if (attackTime == 0 && Input.GetMouseButton(0) && endurance >= 15)
        {
            attackTime = 1;
            anim.SetInteger("AttackState", attackTime);
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    //人物移动
    void Movement()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(inputX, inputY).normalized;
        if (inputX != 0 || inputY != 0)
        {
            if (inputX * transform.localScale.x < 0)    //如果输入运动方向和面朝方向相反，则转向
            {
                if (inputX == 1)                        //输入为右，则向右转
                {
                    transform.position = new Vector3(transform.position.x + 3.14f, transform.position.y,
                        transform.position.z);          //因为人物和对称轴有相对位移，反转时需要手动+-3.14f弥补位移差
                    transform.localScale = Vector3.one * 0.08f;
                }
                else if (inputX == -1)                  //输入为左，则向左转
                {
                    transform.position = new Vector3(transform.position.x - 3.14f, transform.position.y,
                        transform.position.z);          //同上
                    transform.localScale = new Vector3(-1, 1, 1) * 0.08f;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))                        //按下左shift跑步
            {
                if (endurance >= runCost)                               //当且仅有耐力值大于跑步消耗时才可以跑步
                {
                    rb.velocity = moveInput * runSpeed;                 //速度设为跑步速度
                    anim.SetInteger("moveSpeed", 2);        //动画设置为跑步状态
                    runTimer += Time.fixedDeltaTime;                    //开始跑步时，跑步计时器开始计时
                    if (runTimer > runTimerSet)                         //如果跑步计时器超过设定值
                    {
                        runTimer = 0;                                   //则重置计时器
                        endurance -= runCost;                           //并减去花费耐力
                    }

                    enduranceCD = enduranceCDSet;                       //耐力CD重置
                    enduranceTimer = 0;                                 //耐力计时器重置
                }
            }
            else                                                        //未按下则是行走
            {
                rb.velocity = moveInput * walkSpeed;                    //速度设置为行走速度
                anim.SetInteger("moveSpeed", 1);               //动画设置为行走
                EnduranceRecover();

            }
        }
        else
        {
            rb.velocity = Vector2.zero;                                   //速度设为0
            anim.SetInteger("moveSpeed", 0);                   //动画设置为站立
            EnduranceRecover();
        }
    }
    


    //回复耐力
    void EnduranceRecover()
    {
        if (enduranceCD <= 0 && endurance != enduranceSet)          //如果耐力恢复CD为0且耐力值未满
        {
            enduranceTimer += Time.fixedDeltaTime;              //耐力回复计时器开始计时
            if (enduranceTimer > enduranceTimerSet)             //如果耐力回复计时器超过设定值
            {
                endurance += enduranceIncrease;                 //则回复耐力
                if (endurance > enduranceSet)
                {
                    endurance = enduranceSet;
                }
                enduranceTimer = 0;                             //并重置计时器
            }
        }
        else
        {
            enduranceCD -= Time.fixedDeltaTime;                 //耐力恢复CD不为0，则加载CD
        }
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
        GameObject.Find("PLAYER0").GetComponent<AttackTime>().enabled = true;
    }
    void DoAttack(int attackTime)
    {
        if (true)
        {

        }
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
    }
    void ResetAttack()
    {
        attackTime = 0;
        anim.SetInteger("AttackState", attackTime);
        attackTimer = attackTimerSet;
        attackPause = false;
        anim.SetBool("AttackPause", attackPause);
        enduranceStop = false;
    }
    void AttackCost()
    {
        GameObject.Find("PLAYER0").GetComponent<AttackTime>().enabled = false;
        endurance -= 15;
        enduranceStop = true;
    }

    //突刺
    /*void SpikeCheck()
    {
        spikeTimer += Time.deltaTime;
        if (spikeTimer >= 1)
        {
            anim.SetBool("SpikeLock", false);
        }
        if (Input.get)
        {

        }
    }*/

    //耐力回复
    void EnduranceIncreasing()
    {
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
}
