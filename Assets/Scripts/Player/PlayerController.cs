using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    [SerializeField] private float walkSpeed = 1.0f, runSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //击退击飞
    public void Repeled(float x, float y)
    {

    }

    //眩晕
    public void  Collapsing(float CollapseTime)
    {
        Debug.Log("人物被眩晕");
    }

    private void moving()
    {

    }
}
