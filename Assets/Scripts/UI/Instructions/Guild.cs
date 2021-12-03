using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    private Collider2D coll;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject Panel;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        anim.Play("GuildFaded");
        Invoke("Shut",0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Panel.SetActive(true);
    }

    public void Shut()
    {
        Panel.SetActive(false);
    }
}
