using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//锤击右侧范围
public class HammerBlowCircle : MonoBehaviour
{
    public GameObject crack;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        if (playerHurt)
        {
            StartCoroutine(playerHurt.Collapsing(1)) ;
            PlayerMovement.getDamage = 18;
        }
            
    }

    void OpenCrack()
    {
        crack.SetActive(true);
    }

    void CloseCrack()
    {
        crack.SetActive(false);
    }
}
