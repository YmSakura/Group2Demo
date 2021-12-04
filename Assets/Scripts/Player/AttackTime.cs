using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime : MonoBehaviour
{
    public float attackTimer, attackTimerSet = 0.7f;
    public float thirdTimer, thirdTimerSet = 0.15f;
    public bool stateLock;

    // Start is called before the first frame update
    private void OnEnable()
    {
        stateLock = false;
        attackTimer = attackTimerSet;
        thirdTimer = thirdTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        AttackCheckTest();
    }
    void AttackCheckTest()
    {
        if (PlayerMovement.endurance >= 15)
        {
            if (attackTimer > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    attackTimer = attackTimerSet;

                    if (PlayerMovement.attackTime < 3)
                    {
                        stateLock = true;
                    }

                    if (PlayerMovement.attackTime == 3)
                    {
                        thirdTimer -= Time.deltaTime;
                        PlayerMovement.rb.velocity = Vector2.zero;
                        if (thirdTimer <= 0)
                        {
                            this.enabled = false;
                            PlayerMovement.anim.SetBool("AttackPause", true);
                        }
                    }
                    else
                    {
                        this.enabled = false;
                    }
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
            }
            else
            {
                //PlayerMovement.anim.SetBool("AttackPause", true);
                this.enabled = false;
            }
        }
        /* else
         {
             stateLock = false;
         }*/
    }
}
