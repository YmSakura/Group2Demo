using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private float speed = 250f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            gameObject.SetActive(false);
            ObjectPool.Instance.PushObject(gameObject);
            
        }
    }
    
    public void SetSpeed(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }
}
