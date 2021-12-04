using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [Header("BOSS属性")] 
    public float moveSpeed;                 //移动速度
    public float AttackCd = 2f;             //技能的CD
    public static float healthValue = 120f;       //Boss总血量
    private float secondStageHealth;        //进入第二阶段的血量，在awake初始化，为总血量的一半
    private int secondStageCount;           //进入半血的次数，只有第一次掉到半血才进入第二阶段
    public float AttackCdCount;             //技能的计时
    private float droppedHealth = 20f;      //第二阶段固定血量释放魔法攻击
    private int startCount;                 //播放开始动画的次数，只会播放一次
    private int sprintCount;                //冲刺的次数
    private int closeAttackCount;           //进行近身攻击的次数

    [SerializeField] 
    private bool isIdle,
        isWalk,
        isAttacked,
        isWait,
        isDie,
        isChase,
        isPatrol,
        isAttack,
        isInSecondStage,
        isStart,
        canEnergyStorage,
        canShortEnergyStorage,
        canHammerBlow,
        isEnergyStorage,
        isSprint;

    [Header("技能伤害")] 
    private float horizontalAttackDamage = 18f;
    private float verticalAttackDamage = 24f;
    private float hammerBlowDamage = 18f;
    private float magicAttackDamage = 20f;

    [Header("自身组件")] 
    public Transform bossAt;         //Boss的实际位置
    public GameObject head;          //Boss的南瓜头
    private Animator anim;           //animator
    private Rigidbody2D rb;          //rigidbody

    [Header("技能检测相关组件")] 
    public Transform verticalAttackScope;           //竖劈的范围显示（有Sprite，通过overlap检测）
    public GameObject horizontalAttackScope;        //横划的范围显示（通过碰撞体检测）
    public GameObject magicAttackScope;             //魔法攻击点的父物体
    private Transform[] magicCircle;                //魔法攻击具体位置
    public Transform player;                        //player本体
    private Transform playerTransform;              //player的实际位置
    private Transform playerAtLeft, playerAtRight;  //player的左右点位
    public LayerMask playerLayer;                   //player的Layer
    public GameObject pumpkin;                      //南瓜头
    public bool canAttack, canMagicAttack;          //是否可以进行近距离攻击

    [Header("技能名称")] 
    private String horizontalAttack = "HorizontalAttack";
    private String verticalAttack = "VerticalAttack";
    private String hammerBlow = "HammerBlow";
    private String magicAttack = "MagicAttack";
    private String headFly = "HeadFly";

    [Header("锤击检测组件")] 
    public GameObject hammerBlowScope;                  //锤击范围
    public Collider2D armCollider;                      //横扫的碰撞体(骨骼绑定)
    private Transform rightCircle, leftCircle;          //锤击左右圆形

    [Header("巡逻相关")] 
    public GameObject patrolArea;           //巡逻区域
    private List<Transform> patrolPoint;    //巡逻点数组
    private int patrolPosition;             //巡逻点下标
    public LayerMask bossLayer;             //Boss的Layer

    [Header("范围检测")] 
    public Transform attackScope;                           //近身攻击范围
    public bool isInAttackScope;                            //是否处于近身攻击范围
    public GameObject chaseArea;                            //追击范围
    private Transform leftChasePoint, rightChasePoint;      //追击范围矩形的顶点
    private bool isInChaseScope;                            //是否处于追击范围
    private float attackScopeRadius = 5f;                   //攻击范围圆形的半径
    private float xDistance, yDistance;

    [Header("场景部件")] 
    public Light playerLight;   //玩家身上的光
    public Light dirLight;      //场景中的线性光
    public Light headLight;     //Boss南瓜头的点光
    public Light spotLight;    //出口聚光
    public GameObject exit;     //场景出口
    public GameObject BossHp;   //Boss血条
    public GameObject shadow;   //影子

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //锤击范围
        rightCircle = hammerBlowScope.transform.Find("RightCircle");
        leftCircle = hammerBlowScope.transform.Find("LeftCircle");
        //魔法攻击（获取全部子物体包括自己，使用时记得下标从1开始）
        magicCircle = magicAttackScope.GetComponentsInChildren<Transform>();
        patrolPoint = new List<Transform>();
        //获取巡逻点位
        for (int i = 0; i < 4; i++)
        {
            patrolPoint.Add(patrolArea.transform.GetChild(i));
        }
        //追击区域
        leftChasePoint = chaseArea.transform.GetChild(0);
        rightChasePoint = chaseArea.transform.GetChild(1);
        //获取Player左右点位
        playerAtLeft = player.Find("PlayerAtLeft");
        playerAtRight = player.Find("PlayerAtRight");
        //默认以playerAtLeft作为player的位置
        playerTransform = playerAtLeft;
        
        //初始化第二阶段血量
        secondStageHealth = healthValue / 2;

        //默认不开启技能范围检测
        verticalAttackScope.gameObject.SetActive(false);
        horizontalAttackScope.SetActive(false);
        for (int i = 1; i < 4; i++)
        {
            magicCircle[i].gameObject.SetActive(false);
        }
        armCollider.enabled = false;
        pumpkin.SetActive(false);
    }

    void Update()
    {
        PatrolMove();
        IsInAttackScope();
        UpdateStatus();
        CloseAttack();
        ChasePlayer();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            BeAttacked(10f);
        }
    }

    //画出一些范围
    private void OnDrawGizmos()
    {
        //boss的攻击范围
        Gizmos.DrawWireSphere(attackScope.position, attackScopeRadius);
    }


    void UpdateStatus()
    {
        anim.SetFloat("HealthValue", healthValue);
        anim.SetBool("IsAttacked", isAttacked);
        anim.SetBool("IsIdle", isIdle);
        anim.SetBool("IsWalk", isWalk);
        anim.SetBool("IsDie", isDie);
        anim.SetBool("IsAttack", isAttack);
        anim.SetBool("CanMagicAttack", canMagicAttack);
        anim.SetInteger("SprintCount", sprintCount);
        anim.SetBool("CanEnergyStorage", canEnergyStorage);
        anim.SetBool("CanShortEnergyStorage", canShortEnergyStorage);
        UpdateChaseStatus();
        UpdatePlayerTransform();
        yDistance = Mathf.Abs(playerTransform.position.y - bossAt.position.y);
        xDistance = Mathf.Abs(playerTransform.position.x - bossAt.position.x);
    }

    //被攻击时调用
    public void BeAttacked(float damageValue)
    {
        //只有在idle和walk状态下才可以播放受击动画（意味着在释放技能时要关闭Idle和Walk）
        if ((isIdle || isWalk) && !isDie && !isAttack)
        {
            isAttacked = true;
            //SoundManager.Sound.AudioName("boss", "hurt");
            //这里要直接调用一下受击的动画来显示受伤
            anim.SetBool("IsAttacked", isAttacked);
        }
        
        //减少血量
        healthValue -= damageValue;

        if (isInSecondStage)
        {
            //进入第二阶段判定释放魔法攻击的血量
            droppedHealth -= damageValue;
            if (droppedHealth <= 0)
            {
                canMagicAttack = true;
                droppedHealth = 150f;
            }
        }

        //判断是否进入第二阶段
        if (healthValue < secondStageHealth && secondStageCount.Equals(0))
        {
            //释放扔头技能
            anim.SetBool(headFly, true);
            isAttack = true;
            isInSecondStage = true;
            Invoke("OpenPumpkin", 3f);
            playerLight.gameObject.SetActive(true);
            dirLight.gameObject.SetActive(false);//进入第二阶段禁用场景线性光
            headLight.gameObject.SetActive(false);//禁用boss头骨部分的灯光
            //shadow.SetActive(false);//关闭影子
            
            //为了确保只进入一次第二阶段
            secondStageCount++;
            
            //第二阶段属性增益
            moveSpeed += 5f;
            horizontalAttackDamage += 5f;
            verticalAttackDamage += 5f;
            magicAttackDamage += 5f;
            hammerBlowDamage += 5f;
            AttackCd -= 1f;
        }

        //判断是否死亡
        if (healthValue < 0.1f)
        {
            //更新死亡状态
            isDie = true;
            //关闭南瓜头
            StartCoroutine(pumpkin.GetComponent<Pumpkin>().Death());
            //开启播放死亡动画
            anim.Play("die");
            exit.SetActive(false);//关闭出口墙体，打开出口
            BossHp.SetActive(false);//关闭BOSS血条
            shadow.SetActive(false);
            spotLight.gameObject.SetActive(true);
            SoundManager.Sound.StopMusic();
        }
    }

    //扔头动画结束时调用
    void CloseHeadFly()
    {
        anim.SetBool(headFly, false);
    }
    
    //开启南瓜头
    void OpenPumpkin()
    {
        pumpkin.SetActive(true);
    }
    
    //死亡动画结束时调用，为了只播放一次死亡动画，之后就一直处于die state
    // void CloseDieAnim()
    // {
    //     anim.SetBool("StartDieAnim", false);
    // }

    //检测玩家是否处于追击范围
    public void UpdateChaseStatus()
    {
        //出了boss攻击范围才可以追击
        if (!isInAttackScope && !isAttack && !isAttacked && isStart)
        {
            //如果玩家位于追击范围内就进行追击，停止巡逻
            if (Physics2D.OverlapArea(leftChasePoint.position, rightChasePoint.position, playerLayer))
            {
                isChase = true;
                isPatrol = false;
            }
            else
            {
                //如果玩家逃出追击范围就停止追击，开始巡逻
                isChase = false;
                isPatrol = true;
            }
        }
    }
    
    void ChasePlayer()
    {
        if (isSprint)
        {
            //冲刺的时候往玩家方向冲
            Sprint();
        }
        
        //当玩家处于追击范围内 并且位于攻击范围外才可以追击
        if (isChase && !isDie && isStart && !isEnergyStorage && !isAttack && !isAttacked)
        {
            FlipTo(playerTransform);
            isWalk = true;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,
                moveSpeed * Time.deltaTime);
            
            //玩家超出一定距离时boss可以冲刺到玩家身边(第二阶段)
            if (xDistance > attackScopeRadius * 2.5 && isInSecondStage)
            {
                canEnergyStorage = true;
            }
            else
            {
                canEnergyStorage = false;
            }
        }
    }

    //受击动画结束时调用，关闭被攻击状态
    public void CloseBeAttacked()
    {
        isAttacked = false;
    }
    

    //检测玩家是否位于攻击范围内，Update中调用
    void IsInAttackScope()
    {
        if (Physics2D.OverlapCircle(attackScope.position, attackScopeRadius, playerLayer))
        {
            //只要进入攻击范围，就停止chase，通过内部函数来追击
            isInAttackScope = true;
            isChase = false;
            //Debug.Log(yDistance);
            //如果玩家与boss的垂直距离大于指定距离，boss就向y方向移动
            if (yDistance > attackScopeRadius/4)
            {
                canAttack = false;
                
                //释放技能和蓄力时无法移动
                if (!isAttack && isStart && !isDie && !isEnergyStorage && !isAttacked)
                {
                    FlipTo(playerTransform);
                    isWalk = true;
                    transform.position = Vector2.MoveTowards(transform.position,
                        new Vector2(transform.position.x, playerTransform.position.y),
                        moveSpeed * Time.deltaTime);
                }
            }
            else if (yDistance <= attackScopeRadius / 4 && xDistance <= attackScopeRadius)
            {
                //x距离足够小时才可以释放锤击
                if (xDistance <= attackScopeRadius / 2.5)
                {
                    canHammerBlow = true;
                }
                
                canAttack = true;
            }
        }
        else
        {
            isInAttackScope = false;
            canAttack = false;
            
            //当人物离开攻击范围时重置cd
            closeAttackCount = 0;
        }

    }

    //近距离攻击(横划竖劈锤击)，Update中调用
    void CloseAttack()
    {
        if (canAttack && isStart)
        {
            //玩家到达指定范围后boss不再移动
            if (yDistance <= attackScopeRadius / 4 && xDistance <= attackScopeRadius * 1.1)
            {
                isIdle = true;
                isWalk = false;
            }

            //前三次攻击没有cd
            if (closeAttackCount <= 2)
            {
                AttackCdCount = 3f;
            }

            if (AttackCdCount >= AttackCd)
            {
                //攻击次数++
                closeAttackCount++;
                int randomInt;
                if (isInSecondStage)
                {
                    //进入第二阶段近距离攻击只有横划和竖劈
                    randomInt = Random.Range(0, 3);
                    anim.SetInteger("RandomInt", randomInt);
                }
                else
                {
                    //第一阶段随机释放横划、竖劈和锤击
                    randomInt = Random.Range(0, 4);
                    if (randomInt.Equals(3))
                    {
                        //当随机到锤击时，检测距离是否足够，如果不够就释放横划和竖劈
                        if (canHammerBlow)
                        {
                            anim.SetInteger("RandomInt", 3);
                            canHammerBlow = false;
                        }
                        else
                        {
                            randomInt = Random.Range(0, 3);
                            anim.SetInteger("RandomInt", randomInt);
                        }
                    }
                }
                
            }

            //计算cd
            AttackCdCount += Time.deltaTime;
        }
        else
        {
            //如果出了近身攻击范围，就取消idle
            isIdle = false;
        }
    }

    //设置技能cd，技能动画结束时调用
    void SetAttackCd()
    {
        AttackCdCount = 0;
        //并将randomInt置为0，保证不重复释放技能
        anim.SetInteger("RandomInt", 0);
    }

    //巡逻时的等待
    IEnumerator TimeWaiter(float waitSeconds)
    {
        Debug.Log("开始等待");
        isIdle = true;
        isWait = true;
        isWalk = false;
        yield return new WaitForSeconds(waitSeconds);
        Debug.Log("等待完毕");
        isWait = false;
        isIdle = false;
        isWalk = true;
        //切换到下一个巡逻点
        patrolPosition++;
        //点位超出数组长度时重置
        if (patrolPosition >= patrolPoint.Count)
        {
            patrolPosition = 0;
        }
    }

    //巡逻
    void PatrolMove()
    {
        if (isPatrol && !isWait && isStart)
        {
            if (!isDie && !isChase)
            {
                isPatrol = true;
                //巡逻过程中以巡逻点为target更改朝向
                FlipTo(patrolPoint[patrolPosition]);
                isWalk = true;
                transform.position = Vector2.MoveTowards(transform.position, patrolPoint[patrolPosition].position,
                    moveSpeed * Time.deltaTime);
            }

            //到达一个巡逻点之后切换到下一个巡逻点
            if (Physics2D.OverlapCircle(patrolPoint[patrolPosition].position, 1f, bossLayer))
            {
                Debug.Log("已到达巡逻点");

                //到达巡逻点后等待一段时间再前往下一个巡逻点
                StartCoroutine(TimeWaiter(3f));
            }
        }
    }
    
    //竖劈蓄力时boss的转向，在动画事件中调用
    void VerticalAttackDirection()
    {
        FlipTo(playerTransform);
    }

    //竖劈的效果，竖劈动画时通过event调用
    void VerticalAttackEffect()
    {
        //获取矩形对角线两顶点
        Transform leftTop = verticalAttackScope.GetChild(0);
        Transform rightBottom = verticalAttackScope.GetChild(1);

        //玩家的碰撞体
        Collider2D playerCollider;
        try
        {
            playerCollider = Physics2D.OverlapArea(leftTop.position, rightBottom.position, playerLayer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.Log("区域内无Player");
            throw;
        }

        //如果检测到玩家
        if (playerCollider)
        {
            PlayerHurt playerHurt = CollisionCheck.PlayerCheck(playerCollider);
            if (playerHurt)
            {
                playerHurt.Collapsing(1);
                PlayerMovement.getDamage = 24;
            }

        }
    }
    
    //开启竖劈技能的范围显示
    void OpenVerticalAttackScope()
    {
        verticalAttackScope.gameObject.SetActive(true);
    }
    
    //关闭竖劈技能范围显示，动画中调用
    void CloseVerticalAttackScope()
    {
        verticalAttackScope.gameObject.SetActive(false);
    }
    

    //开启和关闭横划的检测范围(碰撞体)
    void OpenHorizontalAttackScope()
    {
        horizontalAttackScope.SetActive(true);
    }
    void CloseHorizontalAttackScope()
    {
        horizontalAttackScope.SetActive(false);
    }
    

    //开启和关闭左右两侧圆形碰撞体
    void HammerBlowRight()
    {
        rightCircle.gameObject.SetActive(true);
    }
    void HammerBlowLeft()
    {
        leftCircle.gameObject.SetActive(true);
    }

    void CloseRightCircle()
    {
        rightCircle.gameObject.SetActive(false);
    }
    void CloseLeftCircle()
    {
        leftCircle.gameObject.SetActive(false);
    }

    //开启和关闭横扫的碰撞体
    void OpenArmCollider()
    {
        armCollider.enabled = true;
    }
    void CloseArmCollider()
    {
        armCollider.enabled = false;
    }
    

    //index代表是第几个圆圈，动画中boss三次举手时调用
    void OpenMagicAttackScope(int index)
    {
        StartCoroutine(MagicAttackScope(index));
    }
    IEnumerator MagicAttackScope(int index)
    {
        //每一次出现的时候都是关闭碰撞体的状态
        Collider2D circleCollider = magicCircle[index].GetComponent<Collider2D>();
        circleCollider.enabled = false;
        //获取玩家坐标并显示圆形
        magicCircle[index].position = playerTransform.position;
        magicCircle[index].gameObject.SetActive(true);
        //延时1s开启碰撞体
        yield return new WaitForSeconds(1f);
        MagicAttackEffect(circleCollider);
    }
    //开启碰撞体（上面的函数调用）
    void MagicAttackEffect(Collider2D circleCollider)
    {
        circleCollider.enabled = true;
    }

    //关闭圆形范围显示和碰撞体（动画中碰撞检测后调用）
    void CloseMagicAttackScope(int index)
    {
        magicCircle[index].GetComponent<Collider2D>().enabled = false;
        magicCircle[index].gameObject.SetActive(false);
    }
    
    void UpdatePlayerTransform()
    {
        //计算boss和playerAt左右点的水平距离
        float LeftDistance = Mathf.Abs(bossAt.position.x - playerAtLeft.position.x);
        float rightDistance = Mathf.Abs(bossAt.position.x - playerAtRight.position.x);
        //哪个离boss近就选择哪个
        if (LeftDistance < rightDistance)
        {
            playerTransform = playerAtLeft;
        }
        else
        {
            playerTransform = playerAtRight;
        }
    }

    //以target为目标更改朝向
    void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(0.15f, 0.15f, 1);
            }
            else if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-0.15f, 0.15f, 1);
            }
        }
    }

    //关闭南瓜头，扔头动画中调用
    void HeadHidden()
    {
        head.SetActive(false);
    }

    //由animation event调用，释放技能时处于isAttack状态，不移动，技能结束关闭isAttack状态
    void IsAttack()
    {
        isAttack = true;
        isIdle = false;
        isWalk = false;
    }
    void IsNotAttack()
    {
        isAttack = false;
        isIdle = true;
    }
    
    //检测区域内按下E后延时调用
    public void StartChase()
    {
        isStart = true;
        isIdle = true;
        headLight.intensity = 2;
        BossHp.SetActive(true);
    }
    
    //开启环境光
    public void OpenDirLight()
    {
        dirLight.gameObject.SetActive(true);
        playerLight.gameObject.SetActive(false);
        shadow.SetActive(true);
    }
    
    //魔法攻击时调用
    void CantMagicAttack()
    {
        canMagicAttack = false;
    }
    
    
    //蓄力动画开始时调用
    void IsEnergyStorage()
    {
        isEnergyStorage = true;
    }
    //蓄力结束时调用,完成蓄力，可以进行冲刺
    void FinishStorage()
    {
        anim.SetBool("FinishStorage", true);
    }
    //冲刺开始时调用
    void CloseEnergyStorage()
    {
        //结束“蓄力完成”状态，为了不重复播放冲刺动画
        anim.SetBool("FinishStorage", false);
        //切换状态
        isEnergyStorage = false;
        isSprint = true;
    }
    //冲刺时调用
    void Sprint()
    {
        FlipTo(playerTransform);
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,
            moveSpeed * 5 * Time.deltaTime);
    }
    //每次冲刺结束时调用
    void CloseSprint()
    {
        isSprint = false;
        
        //第一次冲刺时为0，关闭蓄力状态，并开始第二次短蓄力
        if (sprintCount.Equals(0))
        {
            canEnergyStorage = false;
            canShortEnergyStorage = true;
        }
        
        //第二次冲刺结束开启第三次短蓄力
        if (sprintCount.Equals(1))
        {
            canShortEnergyStorage = true;
        }
        
        if (sprintCount.Equals(2))
        {
            //第三次冲刺结束时关闭可以进行短蓄力状态
            canShortEnergyStorage = false;
            sprintCount = 0;
        }
        
        sprintCount++;
    }
    
    //短蓄力开始时调用
    void CloseCanShortStorage()
    {
        canShortEnergyStorage = false;
    }
}
