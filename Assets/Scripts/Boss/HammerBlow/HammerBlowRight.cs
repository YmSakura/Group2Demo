using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//锤击右侧范围
public class HammerBlowRight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        if (playerHurt)
        {
            playerHurt.Collapsing(1);
            PlayerMovement.getDamage = 18;
        }
            
    }
}
