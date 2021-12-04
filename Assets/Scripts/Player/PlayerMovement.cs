using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static int attackTime = 0; //攻击阶段
    private bool attackPause; //是否要停止攻击判断
    public GameObject sword; //获取到剑的对象
    public int damage; //造成的伤害
    public TrailRenderer slash;//剑光
    

    [Header("基本组件")] public static Rigidbody2D rb;
    private Collider2D coll;
    public static Animator anim;

    [Header("基本移动")]
    private bool runEnabled;//是否允许跑步
    private float inputX, inputY; //获取玩家输入的XY方向
    private Vector2 moveInput; //玩家输入的XY单位向量
    private float walkSpeed = 2.5f, runSpeed = 6; //走，跑 移速
    private int runCost = 1; //跑步耐力消耗
    private float runTimer, runTimerSet = 0.1f; //跑步耐力消耗计时器

    [Header("耐力")]
    public int enduranceSet = 100; //耐力上限
    public static int endurance; //耐力值
    [SerializeField] private float enduranceTimer, enduranceTimerSet = 0.1f; //耐力恢复计时器
    [SerializeField] private float enduranceCD;//耐力CD计时器
    private const float enduranceCDSet = 1.2f; //耐力CD时间
    private const int enduranceIncrease = 2; //耐力回复量

    [Header("翻滚")]
    private Vector2 rollDirection;//翻滚方向
    public static bool rollLock; //翻滚锁定
    [SerializeField] private float rollSpeed = 10; //翻滚移速
    private float rollSpeedMultiplier = 2.5f;//翻滚速度乘数,用于递减翻滚速度
    private int rollCost = 20; //翻滚耐力消耗

    [Header("防御")]
    public GameObject shield; //获取到盾牌的对象
    public float defendSpeed = 5; //防御移速
    public int defendPower = 8; //防御值
    //public static bool isHurt; //受伤状态判断
    public static int getDamage;//收到伤害
    public static bool shieldState; //举盾状态
    public int defendCost; //防御耐力消耗

    private float spikeTimer = 0.0f; //突刺计时器

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!rollLock)
        {
            if (attackTime > 0)//如果在攻击则速度设置为0
            {
                rb.velocity = Vector2.zero;
            }
            else//不在战斗则可以进行移动
            {
                Moving();
                slash.gameObject.SetActive(false);//启用剑光
                /*if (shieldState)
                {
                    Defending(moveInput, getDamage);//如果正在举盾，则进行防御受伤判定
                }
                else
                {
                    isHurt = true;//如果不在举盾，则直接变为受伤状态
                }*/
                if (Input.GetMouseButton(0) && endurance >= 15 )
                {
                    attackTime = 1;
                    anim.SetInteger("AttackState", 1);
                    slash.gameObject.SetActive(true);
                }
            }
            DefendingAnim();//随时可以防御
        }
        else//在翻滚则速度逐渐下降,低至一定程度则取消翻滚
        {
            if (rollSpeed > 3)
            {
                rollSpeed -= rollSpeed * rollSpeedMultiplier * Time.deltaTime;
            }
            else
            {
                EndRolling();
            }
        }
        Rolling();
        if (getDamage != 0)
        {
            if (shieldState)
            {
                Defending(moveInput,getDamage);
            }
            else
            {
                Hurt();
            }
        }

    }

    private void FixedUpdate()
    {
        if (rollLock)
        {
            rb.velocity = rollDirection * rollSpeed;
        }

    }


    //人物移动
    void Moving()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(inputX, inputY).normalized;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            runEnabled = true;//按下shift时重置跑步
        if (inputX != 0 || inputY != 0)
        {
            if (inputX * transform.localScale.x < 0) //如果输入运动方向和面朝方向相反，则转向
            {
                transform.position += new Vector3(3.14f * inputX, 0, 0); //因为人物和对称轴有相对位移，反转时需要手动+-3.14f弥补位移差
                transform.localScale = new Vector3(inputX, 1, 1) * 0.08f; //根据输入方向调整左右
            }

            if (Input.GetKey(KeyCode.LeftShift) && runEnabled) //当左shift被按住时并且允许跑步时才开始跑步
            {
                if (endurance >= runCost)//当且仅当耐力值大于跑步消耗时才可以跑步
                {
                    rb.velocity = moveInput * runSpeed; //速度设为跑步速度
                    anim.SetInteger("moveSpeed", 2); //动画设置为跑步状态
                    runTimer += Time.fixedDeltaTime; //开始跑步时，跑步计时器开始计时
                    if (runTimer > runTimerSet) //如果跑步计时器超过设定值
                    {
                        runTimer = 0; //则重置计时器
                        endurance -= runCost; //并减去花费耐力
                    }

                    enduranceCD = enduranceCDSet; //耐力CD重置
                    enduranceTimer = 0; //耐力计时器重置
                }
                else//否则行走
                {
                    rb.velocity = moveInput * walkSpeed; //速度设置为行走速度
                    anim.SetInteger("moveSpeed", 1); //动画设置为行走
                    runEnabled = false;//如果耐力消耗殆尽,禁用跑步,需要等待下次按下shift才能开始重新跑步
                }
            }
            else //未按下左Shift则是行走
            {
                rb.velocity = moveInput * walkSpeed; //速度设置为行走速度
                anim.SetInteger("moveSpeed", 1); //动画设置为行走
                EnduranceRecover();
            }
        }
        else
        {
            rb.velocity = Vector2.zero; //速度设为0
            anim.SetInteger("moveSpeed", 0); //动画设置为站立
            EnduranceRecover();
        }
    }

    //回复耐力
    void EnduranceRecover()
    {
        if (enduranceCD <= 0 && endurance != enduranceSet) //如果耐力恢复CD为0且耐力值未满
        {
            enduranceTimer += Time.fixedDeltaTime; //耐力回复计时器开始计时
            if (enduranceTimer > enduranceTimerSet) //如果耐力回复计时器超过设定值
            {
                endurance += enduranceIncrease; //则回复耐力
                if (endurance > enduranceSet)
                {
                    endurance = enduranceSet;
                }

                enduranceTimer = 0; //并重置计时器
            }
        }
        else if (enduranceCD > 0)
        {
            enduranceCD -= Time.fixedDeltaTime; //耐力恢复CD不为0，则加载CD
        }
    }

    //空格翻滚
    void Rolling()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (endurance >= rollCost)//仅在耐力足够时允许翻滚
            {
                if (moveInput.Equals(Vector2.zero))//如果没有输入，则向人物面朝方向翻滚
                {
                    rollDirection = new Vector2(transform.localScale.x / 0.08f, 0);
                }
                else//否则，则向输入运动方向翻滚
                {
                    rollDirection = moveInput;
                }
                endurance -= rollCost;//扣除对应耐力
                rollSpeed = 15f;//重置翻滚速度(由于每次翻滚时速度线性减小,故每次翻滚前需重置)
                rb.velocity = rollDirection * rollSpeed;
                rollLock = true;//翻滚时禁用其他操作
                //anim.SetBool("rollLock",true);
                anim.Play("roll");//播放翻滚动画
                //Invoke("EndRolling",0.65f);//延时结束
            }
        }
    }


    //结束翻滚（动画事件）
    void EndRolling()
    {
        rb.velocity = Vector2.zero;//速度归0
        rollLock = false;//取消翻滚锁定
        anim.SetBool("rollLock", false);
        enduranceCD = enduranceCDSet;//重置CD
    }

    //普通防御（动画）
    void DefendingAnim()
    {
        if (Input.GetMouseButton(1))
        {
            shield.GetComponent<Collider2D>().enabled = true;
            shieldState = true;
            anim.SetBool("shieldState",true);
        }
        else
        {
            shield.GetComponent<Collider2D>().enabled = false;
            shieldState = false;
            anim.SetBool("shieldState",false);
        }

        /*{
            shield.GetComponent<Collider2D>().enabled = true; //启用盾牌碰撞器

            if (endurance > 0 && Input.GetMouseButton(1))
            {
                shieldState = true; //举盾状态
                anim.SetBool("shieldState", shieldState);
            }
            else
            {
                shield.GetComponent<Collider2D>().enabled = false; //禁用盾牌碰撞器
                shieldState = false; //放下盾牌
                anim.SetBool("shieldState", shieldState);
            }
        }*/
    }

    void Defending(Vector2 moveInput, int Damage)
    {
        if (Damage == 0)//没有收到伤害就是正常的举盾行动
        {
            rb.velocity = defendSpeed * moveInput;//速度设置为防御状态
        }
        else if (Damage > 0)//收到伤害时进行伤害值判定
        {
            rb.velocity = Vector2.zero;//受击止步
            if (endurance < (Damage - defendPower))//伤害值大于耐力，则破盾，返回受伤状态
            {
                endurance = 0;
                //isHurt = true;//更改受伤状态
            }
            else
            {
                endurance -= Damage;//耐力减少等同于原本伤害的数值
                getDamage -= defendPower;//伤害抵消
            }
        }
    }

    //受伤
    void Hurt()
    {
        if (!rollLock)
        {
            PlayerHurt.health -= getDamage;
            anim.Play("hit");//播放受伤动画
            //anim.SetBool("isHurt", true);
            //isHurt = false;
            getDamage = 0;//重置伤害
            attackTime = 0;//打断攻击
        }
    }

    //攻击（仅有动画）
    void AttackCheck()
    {
        GameObject.Find("PLAYER0").GetComponent<AttackTime>().enabled = true;//启用AttackTime连击次数计数脚本
    }

    void ResetAttack()//连击条件重置
    {
        attackTime = 0;
        anim.SetInteger("AttackState", 0);
        attackPause = false;
        anim.SetBool("AttackPause", false);
    }

    void AttackCost()//每次攻击带来的影响
    {
        GameObject.Find("PLAYER0").GetComponent<AttackTime>().enabled = false;//重置AttackTime连击次数计数脚本
        endurance -= 15;
        enduranceCD = enduranceCDSet;
        sword.GetComponent<Sword>().HurtAble();//剑的攻击脚本下，设置为可造成伤害
    }

    void AttackStateChange()//连击转阶段
    {
        if (GameObject.Find("PLAYER0").GetComponent<AttackTime>().stateLock == true)//转换锁开启时才可以转阶段
        {
            attackTime++;
            anim.SetInteger("AttackState", attackTime);
        }
        else
        {
            PlayerMovement.anim.SetBool("AttackPause", true);
        }
    }
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