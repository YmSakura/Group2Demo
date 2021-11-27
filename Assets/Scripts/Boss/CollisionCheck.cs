using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    private static CollisionCheck instance;
    
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    //因为很多trigger都需要检测是否碰撞到player，提取出检测player的方法
    public static void PlayerCheck(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerHurt playerHurt = player.GetComponent<PlayerHurt>();
            playerHurt.Repeled(1,1);
        }
    }
}
