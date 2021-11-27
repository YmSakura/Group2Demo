using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    
    public Transform leftpoint, rightpoint;
    public Transform verticalAttackScope;
    public Transform player;
    
    public float moveSpeed;
    
    //控制转向，默认向右
    [SerializeField]
    private bool faceRight = true;
    private float leftX, rightX;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        //获取子物体坐标
        leftX = leftpoint.position.x;
        rightX = rightpoint.position.x;
        //销毁子物体
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);
        
        //默认不开启技能范围检测
        verticalAttackScope.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
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

    
    void VerticalAttack()
    {
        //播放动画
        anim.SetTrigger("VerticalAttack");
    }

    //蓄力时boss可以转向
    void VerticalAttackDirection()
    {
        if ((faceRight && player.position.x < transform.position.x) || (!faceRight&&player.position.x > transform.position.x))
        {
            transform.localScale.Set(-1,1,1);
        }
    }
    
    //竖劈的效果，竖劈动画时通过event调用
    void VerticalAttackEffect()
    {
        verticalAttackScope.gameObject.SetActive(true);
        //获取矩形对角线两顶点
        Transform leftTop = verticalAttackScope.GetChild(0);
        Transform rightBottom = verticalAttackScope.GetChild(1);
        Debug.Log("检测开始");
        Collider2D player = null;
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
                PlayerController playerController = player.GetComponent<PlayerController>();
                playerController.Collapsing(1f);
            }
        }
        
        verticalAttackScope.gameObject.SetActive(false);
    }
    
    void HorizontalAttack()
    {
        anim.SetTrigger("HorizontalAttack");
    }


    void UpwardAttack()
    {
        anim.SetTrigger("UpwardAttack");
    }
}
