using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask Wall;
    [SerializeField] private Collider2D col;
    private bool isUsed;
    private float speed = 5f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        isUsed = false;
    }
    private void OnEnable()
    {
        
    }

    private void Update()
    {
        Check();
    }


    private void Check()
    {
        if (col.IsTouchingLayers(Wall))                 //碰到墙则消失
        {
            ObjectPool.Instance.PushObject(gameObject);     //推回池中
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss") &&!isUsed)                                           //碰到boss则boss扣血，火球返回池中
        {
            collision.GetComponent<BossController>().BeAttacked(15f);
            //isUsed = true;
            ObjectPool.Instance.PushObject(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && !isUsed)
        {
            PlayerMovement.getDamage = 10;
            //isUsed = true;
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
    
    
    public void SetSpeed(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }
}
