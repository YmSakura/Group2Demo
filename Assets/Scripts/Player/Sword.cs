using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Collider2D swordColl;

    // Update is called once per frame
    void Update()
    {
        swordColl = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boss")
        {
            if (PlayerMovement.attackTime == 1)
            {
                collision.GetComponent<BossController>().BeAttacked(12);
            }
            else if (PlayerMovement.attackTime == 2)
            {
                collision.GetComponent<BossController>().BeAttacked(16);
            }
            else if (PlayerMovement.attackTime == 3)
            {
                collision.GetComponent<BossController>().BeAttacked(22);
            }
        }
    }
}
