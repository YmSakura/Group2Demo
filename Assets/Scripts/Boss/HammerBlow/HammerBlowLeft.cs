using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//锤击左侧范围
public class HammerBlowLeft : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHurt playerHurt = CollisionCheck.PlayerCheck(other);
        if (playerHurt)
        {
            playerHurt.Collapsing(1);
        }
    }
}
