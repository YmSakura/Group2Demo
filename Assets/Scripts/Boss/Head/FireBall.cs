using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerHurt.health -= 10;
            GameObjectPool.PoolInstance.PushBullet(gameObject);
        }
        else if(collision.tag == "Shield")
        {

        }
        else if(collision.tag == "Boss")
        {

        }
        else if(collision.tag == "Wall")
        {
            gameObject.SetActive(false);
            FireBallPool
        }
    }
}
