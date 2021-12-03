using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    [SerializeField] private Transform playerAt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject fireballPrefab;

    private float shootInterval=5f;        //射击CD
    private float lastShoot = -10f;     //上次射击时间
    private float shoottimes=0;           //射击计数器

    private Vector2 direction;          //南瓜头xy坐标
    private Vector2 PlayerTrans;        //玩家xy坐标

    private float speed = 1f;           //南瓜头移动速度

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTrans = new Vector2(playerAt.transform.position.x, playerAt.transform.position.y);
        MoveAndTurn();
        //if (anim.GetBool("Entry") == false)
        {
            Attack();
        }
    }


    //指向玩家&移动
    private void MoveAndTurn()
    {

        direction = (PlayerTrans - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;
        rb.velocity = direction * speed;
    }

    private void Attack()
    {
        if(Time.time > shootInterval + lastShoot)
        {
            Fire();
            lastShoot = Time.time;
            shoottimes++;

        }
        
    }

    protected virtual void Fire()
    {
        GameObject fireball = ObjectPool.Instance.GetObject(fireballPrefab);
        fireball.transform.position = transform.position;
        fireball.GetComponent<FireBall>().SetSpeed(-direction);
    }
}
