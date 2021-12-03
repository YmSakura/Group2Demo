using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    [SerializeField] private Transform playerAt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject fireballPrefab;

    private float shootInterval=5f;        //���CD
    private float lastShoot = -10f;     //�ϴ����ʱ��
    private float shoottimes=0;           //���������

    private Vector2 direction;          //�Ϲ�ͷxy����
    private Vector2 PlayerTrans;        //���xy����

    private float speed = 1f;           //�Ϲ�ͷ�ƶ��ٶ�

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


    //ָ�����&�ƶ�
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
