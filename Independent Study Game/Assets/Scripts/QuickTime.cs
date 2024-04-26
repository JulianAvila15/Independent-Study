using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuickTime : MonoBehaviour
{
    public float GoodRange; //.51 - .57
    public Slider slider;
    private bool reachedMax,stop;
    public Button stopButton;
    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponentInChildren<Slider>();
        reachedMax = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (stop == false)
        {

            if (slider.value == slider.maxValue)
                reachedMax = true;
            if (slider.value == slider.minValue)
                reachedMax = false;
            if (slider.value <= slider.maxValue && !reachedMax)
            {
                slider.value += (.1f / 30);
            }

            if (slider.value >= slider.minValue && reachedMax)
            {
                slider.value -= (.1f / 30);
            }
            else if (reachedMax == true)
            {
                reachedMax = false;
            }

        }

       


    }

    public void StopButton()
    {
        stop = true;

        if (slider.value > .51 && slider.value < .60)
        {
            Debug.Log("Correct timing!");
        }
        else
        {
            Debug.Log("Incorrect timing!");
        }
    }
}
