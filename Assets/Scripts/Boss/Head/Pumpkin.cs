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

    private bool ready, death;               //ready?????????????????,death????boss???????????
    private float shootInterval=3f;         //???CD
    private float shootTimer=0f;            //?????????
    //private float shootTimes=0;             //?????????
    
    private Vector2 direction;              //????xy????
    private Vector2 playerTrans;            //???xy????

    [SerializeField]private float speed = 1f;               //??????????
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        col.enabled = false;
        ready = false;
        death = false;
    }

    private void OnEnable()//????????????????????
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


    void checkReady()
    {
        if (Mathf.Abs(transform.position.x) < 1 || Mathf.Abs(transform.position.y) < 1)//??????(??1,??1)????????????湥??
        {
            ready = true;
            col.enabled = true;
        }
        
    }
        
    //??????&???
    private void MoveAndTurn()
    {

        direction = (playerTrans - new Vector2(transform.position.x, transform.position.y)).normalized;//?????????н?
        transform.right = direction;//??????
        rb.velocity = direction * speed;//????
    }

    
    //???
    private void Attack()
    {
        if (shootTimer != 0)                //???CD????
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
                shootTimer = 0;
        }

        if (shootTimer == 0)                //???CD???????????????
        {
            shootTimer = shootInterval;
            Fire();
        }
        
        
    }

    
    //???????
    protected virtual void Fire()
    {
        GameObject fireball = ObjectPool.Instance.GetObject(fireballPrefab);    //????????????
        fireball.transform.position = transform.position;                       //???????????????λ??
        fireball.GetComponent<FireBall>().SetSpeed(direction);                  //????????????
    }

    
    public IEnumerator Death()//BOSS?????????????????????????
    {
        death = true;
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;//??????0
        while (sprite.color.a > 0)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - 0.1f);
            //????????С
            yield return new WaitForFixedUpdate();//等待一个FixedUpdate帧
        }
        gameObject.SetActive(false);//使南瓜头失活
    }

}
