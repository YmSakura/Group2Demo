using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask Wall;
    [SerializeField] private Collider2D col;
    private float speed = 5f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
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
        if (col.IsTouchingLayers(Wall))
        {
            Debug.Log("Touch");
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    
    public void SetSpeed(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }
}
