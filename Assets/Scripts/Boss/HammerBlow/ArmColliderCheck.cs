using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmColliderCheck : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        if (playerHurt)
        {
            //人物被击退
            playerHurt.Repeled(1,1);
            PlayerMovement.getDamage = 18;
        }
        
    }
}
