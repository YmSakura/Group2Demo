using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBlowLeft : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.Collapsing(1);
        }
    }
}
