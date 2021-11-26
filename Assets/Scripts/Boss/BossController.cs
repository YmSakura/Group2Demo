using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    
    public Transform leftpoint, rightpoint;
    public Transform VerticalAttackScope;
    
    public float moveSpeed;
    
    //控制转向，默认向右
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
        //VerticalAttackScope.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        anim.SetFloat("walkSpeed",moveSpeed);
        VerticalAttack();
        // HorizontalAttack();
        // UpwardAttack();
    }
    
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

    void VerticalAttackEffect()
    {
        //获取矩形对角线两顶点
        Transform leftTop = VerticalAttackScope.GetChild(1);
        Transform rightBottom = VerticalAttackScope.GetChild(2);
        //获取矩形范围内的Player，如果没有返回null
        GameObject player = Physics2D.OverlapArea(leftTop.position, rightBottom.position).gameObject;
        if (player)
        {
            //获取人物的脚本，调用相关函数
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (player.CompareTag("Player"))
            {
                playerController.Collapsing(1f);
            }
        }
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
