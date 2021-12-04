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
    private float shootInterval=3f;         //���CD
    private float shootTimer=0f;            //�ϴ����ʱ��
    private float shootTimes=0;             //���������
    
    private Vector2 direction;              //�Ϲ�ͷxy����
    private Vector2 playerTrans;            //���xy����

    [SerializeField]private float speed = 1f;               //�Ϲ�ͷ�ƶ��ٶ�
    

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
        
    //ָ�����&�ƶ�
    private void MoveAndTurn()
    {

        direction = (playerTrans - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        rb.velocity = direction * speed;
    }

    
    //���
    private void Attack()
    {
        if (shootTimer != 0)                //���CD����
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
                shootTimer = 0;
        }

        if (shootTimer == 0)                //���CD�����Ϳ�ʼ�������
        {
            shootTimer = shootInterval;
            Fire();
        }
        
        
    }

    
    //�������
    protected virtual void Fire()
    {
        GameObject fireball = ObjectPool.Instance.GetObject(fireballPrefab);    //�ӳ���ȡ������
        fireball.transform.position = transform.position;                       //������������Ϲ�ͷλ��
        fireball.GetComponent<FireBall>().SetSpeed(direction);                  //��������ҷ���
    }




}
