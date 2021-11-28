using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("自身组件")]
    private Animator anim;
    private Rigidbody2D rb;
    
    [Header("技能检测相关组件")]
    public Transform verticalAttackScope;
    public GameObject horizontalAttackScope;
    public Transform player;
    public LayerMask playerLayer;

    [Header("锤击检测组件")] 
    public GameObject hammerBlowScope;
    public Collider2D armCollider;
    private Transform rightCircle, leftCircle;
    
    [Header("移动相关")]
    public float moveSpeed;
    public Transform leftpoint, rightpoint;
    private bool faceRight = true;
    private float leftX, rightX;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rightCircle = hammerBlowScope.transform.Find("RightCircle");
        leftCircle = hammerBlowScope.transform.Find("LeftCircle");
        
        //获取子物体坐标
        leftX = leftpoint.position.x;
        rightX = rightpoint.position.x;
        //销毁子物体
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);
        
        //默认不开启技能范围检测
        verticalAttackScope.gameObject.SetActive(false);
        horizontalAttackScope.SetActive(false);
        //hammerBlowScope.SetActive(false);
        armCollider.enabled = false;
    }
    
    void Update()
    {
        Movement();
        HammerBlow();
        anim.SetFloat("walkSpeed",moveSpeed);
    }
    
    //简单移动
    void Movement()
    {
        if(faceRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            if(transform.position.x > rightX)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                faceRight = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            if(transform.position.x < leftX)
            {
                transform.localScale = new Vector3(1, 1, 1);
                faceRight = true;
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
        //如果朝右并且人物在boss左侧就转向
        if (faceRight && player.position.x < transform.position.x)
        {
            Debug.Log("boss向左转");
            transform.localScale = new Vector3(-1, 1, 1);
        }else if (!faceRight && player.position.x > transform.position.x)
        {
            Debug.Log("boss向右转");
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    //竖劈的效果，竖劈动画时通过event调用
    void VerticalAttackEffect()
    {
        verticalAttackScope.gameObject.SetActive(true);
        //获取矩形对角线两顶点
        Transform leftTop = verticalAttackScope.GetChild(0);
        Transform rightBottom = verticalAttackScope.GetChild(1);
        
        Collider2D player;
        try
        {
            player = Physics2D.OverlapArea(leftTop.position, rightBottom.position);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.Log("区域内无Player");
            throw;
        }
        
        //如果检测到player就调用其自身的受伤函数
        if (player)
        {
            if (player.CompareTag("Player"))
            {
                //获取人物的脚本，调用相关函数
                PlayerHurt playerHurt = player.GetComponent<PlayerHurt>();
                playerHurt.Collapsing(1f);
            }
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
        //开启碰撞体检测
        horizontalAttackScope.SetActive(true);
    }
    //横划动画结束时调用
    void CloseHorizontalAttackScope()
    {
        horizontalAttackScope.SetActive(false);
    }

    //上挑动画的启动
    void UpwardAttack()
    {
        anim.SetTrigger("UpwardAttack");
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

    void ExplodeAttack()
    {
        
    }
}
