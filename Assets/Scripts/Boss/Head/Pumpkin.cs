using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    [SerializeField] private Transform playerAt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private SpriteRenderer sprite;

    private bool ready, death;               //ready代表南瓜头是否就绪可以开始战斗,death代表boss本体死亡
    private float shootInterval=3f;         //发射CD
    private float shootTimer=0f;            //发射计时器
    //private float shootTimes=0;             //射击次数
    
    private Vector2 direction;              //南瓜头xy坐标
    private Vector2 playerTrans;            //玩家xy坐标

    [SerializeField]private float speed = 1f;               //飞行速度
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        col.enabled = false;
        ready = false;
        death = false;
    }

    private void OnEnable()//启用时从摄像机外飞到地图中心
    {
        direction = (Vector2.zero - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        rb.velocity = direction * speed * 10;
    }

    // Update is called once per frame
    void Update()
    {
        playerTrans = new Vector2(playerAt.transform.position.x, playerAt.transform.position.y);
        if (ready)
        {
            if (!death)
            {
                MoveAndTurn();
                Attack();
            }
        }
        else
        {
            checkReady();
        }
    }

    //检测南瓜头进入,到达指定区域使其就绪进入战斗
    void checkReady()
    {
        if (Mathf.Abs(transform.position.x) < 1 || Mathf.Abs(transform.position.y) < 1)//在(+-1,+-1)的区域内激活南瓜头进入就绪状态
        {
            ready = true;
            col.enabled = true;
        }
        
    }
        
    //朝玩家飞行
    private void MoveAndTurn()
    {

        direction = (playerTrans - new Vector2(transform.position.x, transform.position.y)).normalized;//计算和玩家的夹角
        transform.right = direction;//面朝玩家
        rb.velocity = direction * speed;//飞行
    }

    
    //发射火球
    private void Attack()
    {
        if (shootTimer != 0)                //火球发射冷却
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
                shootTimer = 0;
        }

        if (shootTimer == 0)                //发射计时器为0则发射火球
        {
            shootTimer = shootInterval;
            Fire();
        }
        
        
    }

    
    //发射火球
    protected virtual void Fire()
    {
        GameObject fireball = ObjectPool.Instance.GetObject(fireballPrefab);    //从池中取出火球
        fireball.transform.position = transform.position;                       //将火球放置于南瓜头位置
        fireball.GetComponent<FireBall>().SetSpeed(direction);                  //朝玩家发射
    }

    
    public IEnumerator Death()//BOSS死亡时调用,使南瓜头失活
    {
        death = true;
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;//速度设置为0
        while (sprite.color.a > 0)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - 0.1f);
            //南瓜头逐渐透明
            yield return new WaitForFixedUpdate();//等待一个FixedUpdate帧
        }
        gameObject.SetActive(false);//ʹ�Ϲ�ͷʧ��
    }

}
