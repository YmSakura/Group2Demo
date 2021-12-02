using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider HPSlider, enduranceSlider;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HPSlider.value = PlayerHurt.health;
        enduranceSlider.value = PlayerMovement.endurance;
    }
}
