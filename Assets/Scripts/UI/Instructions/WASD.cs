using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASD : MonoBehaviour
{
    private Collider2D coll;
    public GameObject panel;
    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
