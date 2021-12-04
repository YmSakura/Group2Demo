using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    [SerializeField] private Transform playerAt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject fireballPrefab;

    public bool ready;
    private float shootInterval=3f;         //射击CD
    private float shootTimer=0f;            //上次射击时间
    private float shootTimes=0;             //射击计数器
    
    private Vector2 direction;              //南瓜头xy坐标
    private Vector2 playerTrans;            //玩家xy坐标

    [SerializeField]private float speed = 1f;               //南瓜头移动速度
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        col.enabled = false;
        ready = false;
    }

    private void OnEnable()
    {
        direction = (Vector2.zero - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        rb.velocity = direction * speed * 10;
    }

    // Update is called once per frame
    void Update()
    {
        playerTrans = new Vector2(playerAt.transform.position.x, playerAt.transform.position.y);
        checkReady();
        if (ready)
        {
            MoveAndTurn();
            Attack();
        }
    }


    void checkReady()
    {
        if (Mathf.Abs(transform.position.x) < 1 || Mathf.Abs(transform.position.y) < 1)
        {
            ready = true;
            col.enabled = true;
        }
        
    }
        
    //指向玩家&移动
    private void MoveAndTurn()
    {

        direction = (playerTrans - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        rb.velocity = direction * speed;
    }

    
    //射击
    private void Attack()
    {
        if (shootTimer != 0)                //射击CD计算
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
                shootTimer = 0;
        }

        if (shootTimer == 0)                //射击CD结束就开始发射火球
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
        fireball.GetComponent<FireBall>().SetSpeed(direction);                  //将火球朝玩家发射
    }




}
