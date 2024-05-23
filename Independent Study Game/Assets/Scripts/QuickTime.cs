using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuickTime : MonoBehaviour
{
    public float GoodRange; //.51 - .57
    private bool goodRangeMet,okRangeMet;
    public Slider slider;
    private bool reachedMax,stop;
    public Button stopButton;
    public CraftingManager craftMan;
    int slotIndex;
    // Start is called before the first frame update

    public Text goodText, okayText1, okayText2, badText1, badText2;
    void Start()
    {
        slider = gameObject.GetComponentInChildren<Slider>();
        reachedMax = false;

        goodText.gameObject.SetActive(false);
        okayText1.gameObject.SetActive(false);
        okayText2.gameObject.SetActive(false);
        badText1.gameObject.SetActive(false);
        badText2.gameObject.SetActive(false);

        slider.value = 0;
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
                slider.value += (.4f / 30);
            }

            if (slider.value >= slider.minValue && reachedMax)
            {
                slider.value -= (.4f / 30);
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
            goodRangeMet = true;
            goodText.gameObject.SetActive(true);
           
        }
        else if((slider.value<.80f && slider.value>.6f) ||(slider.value<.51&&slider.value>.25))
        {
            okRangeMet = true;

            if (slider.value < .80f && slider.value > .6f)
                okayText1.gameObject.SetActive(true);

            if(slider.value < .51 && slider.value > .25)
                okayText2.gameObject.SetActive(true);

        }
        else
        {
            if ((slider.value < 1f && slider.value > .80f))
            {
                badText2.gameObject.SetActive(true);
            }

            if ((slider.value < .25f && slider.value > 0f))
            {
                badText1.gameObject.SetActive(true);
            }

            Debug.Log("Incorrect timing!");
            goodRangeMet = false;
        }
        TimeManager.ResetAFKTimer();
        StartCoroutine(CreateIngredients());
    }

   IEnumerator CreateIngredients()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        TimeManager.ResetAFKTimer();

        if (okRangeMet)
        {
            slotIndex = 2;
            craftMan.CreateSlot(slotIndex);
        }

        if (goodRangeMet)
        {
            for (int i = 0; i < 2; i++)
            {
                slotIndex = i + 2;
                craftMan.CreateSlot(slotIndex);
            }
        }
                
    }

    private void OnEnable()
    {
        goodText.gameObject.SetActive(false);
        okayText1.gameObject.SetActive(false);
        okayText2.gameObject.SetActive(false);
        badText1.gameObject.SetActive(false);
        badText2.gameObject.SetActive(false);
        stop = false;
        goodRangeMet = false;
        okRangeMet = false;
       
        slider.value = 0;
    }
}
