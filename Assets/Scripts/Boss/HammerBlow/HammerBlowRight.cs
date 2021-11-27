using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBlowRight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        playerHurt.Collapsing(1);
    }
}
