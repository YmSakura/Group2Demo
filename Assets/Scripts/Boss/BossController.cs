using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [Header("BOSS属性")] 
    [SerializeField]
    private float healthValue = 100;

    private float secondStageHealth = 50;
    private int secondStageCount;
    public float AttackCd = 2f;
    private bool isHealthDown = true;
    [SerializeField]
    private bool isIdle, isWalk, isAttacked, isWait, isDie, isChase, isPatrol, isInSecondStage;

    [Header("自身组件")] public GameObject head;
    private Animator anim;
    private Rigidbody2D rb;
    
    [Header("技能检测相关组件")]
    public Transform verticalAttackScope;   //竖劈的范围显示（有Sprite，通过overlap检测）
    public GameObject horizontalAttackScope;//横划的范围显示（通过碰撞体检测）
    public GameObject magicAttackScope;     //魔法攻击点的父物体
    private Transform[] magicCircle;        //魔法攻击具体位置
    public GameObject chaseArea;
    private Transform leftChasePoint, rightChasePoint;
    public Transform playerTransform;       //player的位置
    public LayerMask playerLayer;           //player的Layer
    private String horizontalAttack = "HorizontalAttack";
    private String verticalAttack = "VerticalAttack";
    private String hammerBlow = "HammerBlow";
    private String magicAttack = "MagicAttack";
    private String headFly = "HeadFly";

    [Header("锤击检测组件")] 
    public GameObject hammerBlowScope;          //锤击范围
    public Collider2D armCollider;              //横扫的碰撞体
    private Transform rightCircle, leftCircle;  //锤击左右圆形
    
    [Header("移动相关")]
    public float moveSpeed;             //移动速度
    public Transform[] patrolPoint;     //巡逻点数组
    public int patrolPosition;          //巡逻点下标
    public LayerMask bossLayer;         //Boss的Layer

    [Header("范围检测")] 
    public Transform attackScope;
    [SerializeField]
    private bool isInAttackScope;
    private bool isInChaseScope;
    private float attackScopeRadius = 15f;  //攻击范围圆形的半径
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rightCircle = hammerBlowScope.transform.Find("RightCircle");
        leftCircle = hammerBlowScope.transform.Find("LeftCircle");
        magicCircle = magicAttackScope.GetComponentsInChildren<Transform>();
        leftChasePoint = chaseArea.transform.GetChild(0);
        rightChasePoint = chaseArea.transform.GetChild(1);
        
        //默认不开启技能范围检测
        verticalAttackScope.gameObject.SetActive(false);
        horizontalAttackScope.SetActive(false);
        for (int i = 1; i < 4; i++)
        {
            magicCircle[i].gameObject.SetActive(false);
        }
        armCollider.enabled = false;
    }
    
    void Update()
    {
        if (!isIdle)
        {
            PatrolMove();
        }
        IsInAttackScope();
        UpdateStatus();
        CloseAttack();
        MagicAttack();
        ChasePlayer();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            BeAttacked(10f);
        }
        //anim.SetInteger("RandomInt",Random.Range(0,4));
    }
    
    
    void UpdateStatus()
    {
        anim.SetFloat("WalkSpeed",moveSpeed);
        anim.SetFloat("HealthValue",healthValue);
        anim.SetBool("IsAttacked", isAttacked);
        anim.SetBool("IsIdle",isIdle);
        //anim.SetBool("isWalk",isWalk);
        anim.SetBool("IsDie",isDie);
        
    }

    void HeadHidden()
    {
        head.SetActive(false);
    }
    
    void BeAttacked(float damageValue)
    {
        //只有在idle和walk状态下才可以播放受击动画
        if (isIdle || isWalk)
        {
            isAttacked = true;
            //这里要直接调用一下受击的动画
            anim.SetBool("IsAttacked", isAttacked);
        }
        
        //减少血量并判断是否死亡
        healthValue -= damageValue;
        if (healthValue.Equals(secondStageHealth) && secondStageCount.Equals(0))
        {
            anim.SetTrigger(headFly);
            secondStageCount++;
        }
        
        if (healthValue < 0.1f)
        {
            isDie = true;
            //开启播放死亡动画，动画开始时调用设置StartDieAnim为false，避免循环播放
            anim.SetBool("StartDieAnim", true);
        }
    }
    void CloseDieAnim()
    {
        anim.SetBool("StartDieAnim", false);
    }

    //检测玩家是否处于追击范围
    public void UpdateChaseStatus()
    {
        if ( Physics2D.OverlapArea(leftChasePoint.position, rightChasePoint.position, playerLayer) )
        {
            isChase = true;
            isPatrol = false;
            //Debug.Log("开始追击");
        }
        else
        {
            isChase = false;
            Debug.Log("结束追击");
            isPatrol = true;
        }
    }
    
    void ChasePlayer()
    {
        if (isChase && !isDie)
        {
            FlipTo(playerTransform);
            isWalk = true;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,
                moveSpeed * Time.deltaTime);
        }
    }

    
    
    //受击动画结束时调用
    public void CloseBeAttacked()
    {
        isAttacked = false;
    }
    
    
    //画出一些范围
    private void OnDrawGizmos()
    {
        //boss的攻击范围
        Gizmos.DrawWireSphere(attackScope.position, attackScopeRadius);
        //巡逻点判定范围
        Gizmos.DrawWireSphere(patrolPoint[patrolPosition].position,0.5f);
    }

    //检测玩家是否位于攻击范围内，Update中调用
    void IsInAttackScope()
    {
        if (Physics2D.OverlapCircle(transform.position, attackScopeRadius, playerLayer))
        {
            //Debug.Log("玩家在攻击范围内");
            isInAttackScope = true;
            //如果玩家已经处于攻击范围内，则停止追击，开始攻击玩家
            isChase = false;
        }
        else
        {
            isInAttackScope = false;
            //如果玩家不在攻击范围内，才检测玩家是否处于追击范围
            UpdateChaseStatus();
        }

    }

    //近距离攻击
    void CloseAttack()
    {
        if (isInAttackScope)
        {
            isIdle = false;
            isWalk = false;
            //当玩家在boss攻击范围内时，随机释放横划、竖劈和锤击
            anim.SetInteger("RandomInt",Random.Range(0,4));
        }
        else
        {
            anim.SetInteger("RandomInt", 0);
        }
    }
    
    IEnumerator TimeWaiter(float waitSeconds)
    {
        Debug.Log("开始等待");
        isIdle = true;
        isWait = true;
        yield return new WaitForSeconds(waitSeconds);
        Debug.Log("等待完毕");
        isWait = false;
        isIdle = false;
        patrolPosition++;
        if (patrolPosition >= patrolPoint.Length)
        {
            patrolPosition = 0;
        }
    }

    //巡逻
    void PatrolMove()
    {
        if (isPatrol)
        {
            if (!isWait && !isDie && !isChase)
            {
                isPatrol = true;
                //巡逻过程中以巡逻点为target更改朝向
                FlipTo(patrolPoint[patrolPosition]);
                isWalk = true;
                transform.position = Vector2.MoveTowards(transform.position, patrolPoint[patrolPosition].position,
                    moveSpeed * Time.deltaTime);
            }

            //到达一个巡逻点之后切换到下一个巡逻点
            if (Physics2D.OverlapCircle(patrolPoint[patrolPosition].position, 0.5f, bossLayer))
            {
                Debug.Log("已到达巡逻点");

                //到达巡逻点后等待一段时间再前往下一个巡逻点
                StartCoroutine(TimeWaiter(5f));
            }
        }
    }

    //竖劈动画的启动
    void VerticalAttack()
    {
        anim.SetTrigger(verticalAttack);
    }
    //竖劈蓄力时boss的转向
    void VerticalAttackDirection()
    {
        FlipTo(playerTransform);
    }
    //竖劈的效果，竖劈动画时通过event调用
    void VerticalAttackEffect()
    {
        verticalAttackScope.gameObject.SetActive(true);
        //获取矩形对角线两顶点
        Transform leftTop = verticalAttackScope.GetChild(0);
        Transform rightBottom = verticalAttackScope.GetChild(1);
        
        Collider2D playerCollider;  //玩家的碰撞体
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
        
        if (playerCollider)
        {
            PlayerHurt playerHurt = CollisionCheck.PlayerCheck(playerCollider);
            if(playerHurt)
                playerHurt.Collapsing(1);
        }
        
        verticalAttackScope.gameObject.SetActive(false);
    }
    
    //横划动画的启动
    void HorizontalAttack()
    {
        anim.SetTrigger(horizontalAttack);
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
    
    //锤击动画的启动
    void HammerBlow()
    {
        anim.SetTrigger(hammerBlow);
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

    //魔法攻击
    //全图范围内获取Player坐标，生成一个红色圆圈，延迟爆炸
    void MagicAttack()
    {
        //当生命值下降到一定数值时释放
        if (healthValue.Equals(50.0f) && isHealthDown)
        {
            anim.SetTrigger(magicAttack);
        }
    }
    void OpenMagicAttackScope(int i)
    {
        StartCoroutine(MagicAttackScope(i));
    }
    IEnumerator MagicAttackScope(int index)
    {
        //默认关闭碰撞体
        Collider2D circleCollider = magicCircle[index].GetComponent<Collider2D>();
        circleCollider.enabled = false;
        //获取玩家坐标并显示圆形范围
        magicCircle[index].position = playerTransform.position;
        magicCircle[index].gameObject.SetActive(true);
        //延时1s调用碰撞检测函数
        yield return new WaitForSeconds(1f);
        MagicAttackEffect(circleCollider);
    }
    //检测是否攻击到玩家
    void MagicAttackEffect(Collider2D circleCollider)
    {
        circleCollider.enabled = true;
    }
    //关闭圆形范围显示和碰撞体
    void CloseMagicAttackScope(int index)
    {
        magicCircle[index].GetComponent<Collider2D>().enabled = false;
        magicCircle[index].gameObject.SetActive(false);
    }
    

    //以target为目标更改朝向
    void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }else if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
