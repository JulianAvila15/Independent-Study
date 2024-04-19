using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuickTime : MonoBehaviour
{
    public float GoodRange; //.51 - .57
    public Slider slider;
    private bool reachedMax;
    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponentInChildren<Slider>();
        reachedMax = false;
    }

    // Update is called once per frame
    void Update()
    {



        if (slider.value == slider.maxValue)
            reachedMax = true;
        if (slider.value <= slider.maxValue && !reachedMax)
        {
            slider.value += (.1f / 60);
            

        }
       


        if (slider.value >= slider.minValue && reachedMax)
        {
            slider.value -= (.1f / 60);
        }
        else if (reachedMax == true)
        {
            reachedMax = false;
        }

      




    }
}
