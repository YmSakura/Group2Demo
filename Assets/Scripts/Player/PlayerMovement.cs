using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static int attackTime = 0; //攻击阶段
    private bool attackPause; //是否要停止攻击判断
    private float attackTimer, attackTimerSet = 1.0f; //攻击计时器
    public GameObject sword; //获取到剑的对象
    public int damage; //造成的伤害

    [Header("基本组件")] public static Rigidbody2D rb;
    private Collider2D coll;
    public static Animator anim;
    
    [Header("基本移动")]
    private float inputX, inputY; //获取玩家输入的XY方向
    private Vector2 moveInput; //玩家输入的XY单位向量
    public float walkSpeed = 5, runSpeed = 10; //走，跑 移速
    public int runCost = 1; //跑步耐力消耗
    private float runTimer, runTimerSet = 0.1f; //跑步耐力消耗计时器

    [Header("耐力")]
    public int enduranceSet = 100; //耐力上限
    public static int endurance; //耐力值
    [SerializeField]private float enduranceTimer, enduranceTimerSet = 0.1f; //耐力恢复计时器
    [SerializeField]private float enduranceCD;//耐力CD计时器
    private const float enduranceCDSet= 0.5f; //耐力CD时间
    private const int enduranceIncrease = 2; //耐力回复量

    [Header("翻滚")] 
    private Vector2 rollDirction;//翻滚方向
    public static bool rollLock; //翻滚锁定
    [SerializeField]private float rollSpeed = 15f; //翻滚移速
    private float rollSpeedMultiplier = 2.5f;//翻滚速度乘数,用于递减翻滚速度
    private int rollCost = 20; //翻滚耐力消耗
    
    [Header("防御")]
    public float defendSpeed = 20; //防御移速
    public int defendPower = 8; //防御值
    public static bool isHurt; //受伤状态判断
    public static bool shieldState; //举盾状态
    public int defendCost; //防御耐力消耗

    private float spikeTimer = 0.0f; //突刺计时器

    [Header("外部数据测试")] public static int getDamage = 1;

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
        if (!rollLock)
        {
            if (attackTime == 0)
            {
                Moving();
                DefendingAnim();
            }
            if (attackTime == 0 && Input.GetMouseButton(0) && endurance >= 15)
            {
                attackTime = 1;
                anim.SetInteger("AttackState", attackTime);
            }
        }
        else
        {
            if (rollSpeed > 3)
            {
                rollSpeed -= rollSpeed*rollSpeedMultiplier * Time.deltaTime;
            }
            else
            {
                EndRolling();
            }
        }
        Rolling();
    }

    private void FixedUpdate()
    {
        if (rollLock)
        {
            rb.velocity = rollDirction * rollSpeed;
        }
        
    }

    
    //人物移动
    void Moving()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(inputX, inputY).normalized;
        if (inputX != 0 || inputY != 0)
        {
            if (inputX * transform.localScale.x < 0) //如果输入运动方向和面朝方向相反，则转向
            {
                transform.position += new Vector3(3.14f * inputX, 0, 0); //因为人物和对称轴有相对位移，反转时需要手动+-3.14f弥补位移差
                transform.localScale = new Vector3(inputX, 1, 1) * 0.08f; //根据输入方向调整左右
            }

            if (Input.GetKey(KeyCode.LeftShift) && endurance >= runCost) //按下左shift跑步,并且仅有耐力值大于跑步消耗时才可以跑步
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
        else if(enduranceCD>0)
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
                    rollDirction = new Vector2(transform.localScale.x / 0.08f, 0);
                }
                else//否则，则向输入运动方向翻滚
                {
                    rollDirction = moveInput;
                }
                endurance -= rollCost;//扣除对应耐力
                rollSpeed = 15f;//重置翻滚速度(由于每次翻滚时速度线性减小,故每次翻滚前需重置)
                rb.velocity = rollDirction * rollSpeed;
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
    /*void DoAttack(int attackTime)
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
    }*/
    void ResetAttack()
    {
        attackTime = 0;
        anim.SetInteger("AttackState", attackTime);
        attackTimer = attackTimerSet;
        attackPause = false;
        anim.SetBool("AttackPause", attackPause);
    }

    void AttackCost()
    {
        GameObject.Find("PLAYER0").GetComponent<AttackTime>().enabled = false;
        endurance -= 15;
        enduranceCD = enduranceCDSet;
    }

    void AttackStateChange()
    {
        if (GameObject.Find("PLAYER0").GetComponent<AttackTime>().stateLock == true)
        {
            attackTime++;
            anim.SetInteger("AttackState", attackTime);
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