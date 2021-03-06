using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalAttackScope : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        
        if (playerHurt)
        {
            playerHurt.Repeled();
            PlayerMovement.getDamage = 18;
        }
    }
}
