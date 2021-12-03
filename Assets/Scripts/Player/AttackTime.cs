using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTime : MonoBehaviour
{
    private float attackTimer, attackTimerSet = 0.8f;
    private float thirdTimer, thirdTimerSet = 1.6f;

    // Start is called before the first frame update
    private void OnEnable()
    {
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
            if(attackTimer > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerMovement.attackTime++;
                    attackTimer = attackTimerSet;
                    PlayerMovement.anim.SetInteger("AttackState", PlayerMovement.attackTime);
                    if (PlayerMovement.attackTime > 3)
                    {
                        PlayerMovement.attackTime = 3;
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
                    Debug.Log(attackTimer);
                }
            }
            else
            {
                PlayerMovement.anim.SetBool("AttackPause", true);
                this.enabled = false;
            }
        }
        else
        {
            PlayerMovement.anim.SetBool("AttackPause", true);
            this.enabled = false;
        }
    }
}
