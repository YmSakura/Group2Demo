using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [Header("自身组件")]
    private Animator anim;
    private Rigidbody2D rb;
    
    [Header("技能检测相关组件")]
    public Transform verticalAttackScope;
    public GameObject horizontalAttackScope;
    public Transform playerTransform;
    public LayerMask playerLayer;

    [Header("锤击检测组件")] 
    public GameObject hammerBlowScope;
    public Collider2D armCollider;
    private Transform rightCircle, leftCircle;
    
    [Header("移动相关")]
    public float moveSpeed;
    public Transform[] patrolPoint;
    public int patrolPosition;
    public LayerMask bossLayer;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rightCircle = hammerBlowScope.transform.Find("RightCircle");
        leftCircle = hammerBlowScope.transform.Find("LeftCircle");
        
        
        //默认不开启技能范围检测
        verticalAttackScope.gameObject.SetActive(false);
        horizontalAttackScope.SetActive(false);
        //hammerBlowScope.SetActive(false);
        armCollider.enabled = false;
    }
    
    void Update()
    {
        PatrolMove();
        anim.SetFloat("walkSpeed",moveSpeed);
        anim.SetInteger("RandomInt",Random.Range(1,3));
    }

    //巡逻
    void PatrolMove()
    {
        //巡逻过程中以巡逻点为target更改朝向
        FlipTo(patrolPoint[patrolPosition]);
        transform.position = Vector2.MoveTowards(transform.position, patrolPoint[patrolPosition].position,
            moveSpeed * Time.deltaTime);
        //到达一个巡逻点之后切换到下一个巡逻点
        if (Physics2D.OverlapCircle(patrolPoint[patrolPosition].position,1f,bossLayer))
        {
            patrolPosition++;
            if (patrolPosition >= patrolPoint.Length)
            {
                patrolPosition = 0;
            }
        }
    }

    //竖劈动画的启动
    void VerticalAttack()
    {
        anim.SetTrigger("VerticalAttack");
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
            playerHurt.Collapsing(1);
        }
        
        verticalAttackScope.gameObject.SetActive(false);
    }
    
    //横划动画的启动
    void HorizontalAttack()
    {
        anim.SetTrigger("HorizontalAttack");
    }
    //横划动画开始时调用
    void HorizontalAttackEffect()
    {
        horizontalAttackScope.SetActive(true);
    }
    //横划动画结束时调用
    void CloseHorizontalAttackScope()
    {
        horizontalAttackScope.SetActive(false);
    }
    
    //锤击动画的启动
    void HammerBlow()
    {
        anim.SetTrigger("HammerBlow");
    }
    //锤击左右圆圈的碰撞体
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
    //横扫的碰撞体
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
        anim.SetTrigger("MagicAttack");
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
