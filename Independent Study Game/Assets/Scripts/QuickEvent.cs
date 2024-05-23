using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class QuickEvent : MonoBehaviour
{
    public GameObject timing;
    public GameObject collectingGame;

    public Image quickEventImage1;
    public Image quickEventImage2;

    public GameObject[] powerUps;

    public CraftingManager craftingMan;
    public GameManager gameManager;

    [Header("quickEvent1")]
    public float coolDown1;
    bool isCoolDown1 = false;
    public KeyCode quickEvent1;
    bool pressed1 = false;

    [Header("quickEvent2")]
    public float coolDown2;
    bool isCoolDown2 = false;
    public KeyCode quickEvent2;
    bool pressed2 = false;

    public DataMiner miner;

    // Start is called before the first frame update
    void Start()
    {
        quickEventImage1.fillAmount = quickEventImage2.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        QuickEvent1();
        QuickEvent2();
    }

    private void QuickEvent1()
    {
        if ((Input.GetKey(quickEvent1) || pressed1) && isCoolDown1 == false)
        {
            isCoolDown1 = true;
            quickEventImage1.fillAmount = 1;
            timing.SetActive(true);
            DataMiner.quickTimeCount[0]++;
        }


        if (isCoolDown1 == true)
        {
            quickEventImage1.fillAmount -= 1 / coolDown1 * Time.deltaTime;

            if (quickEventImage1.fillAmount <= 0)
            {
                quickEventImage1.fillAmount = 0;
                isCoolDown1 = false;
                pressed1 = false;
            }
        }
    }

    public void TimingGameEnabled()
    {
            pressed1 = true;
    }

    private void QuickEvent2()
    {
        if ((Input.GetKey(quickEvent2) || pressed2) && isCoolDown2 == false)
        {
            isCoolDown2 = true;
            quickEventImage2.fillAmount = 1;
            collectingGame.SetActive(true);
            DataMiner.abilityCount[1]++;
        }


        if (isCoolDown2 == true)
        {
            quickEventImage2.fillAmount -= 1 / coolDown1 * Time.deltaTime;

            if (quickEventImage2.fillAmount <= 0)
            {
                quickEventImage2.fillAmount = 0;
                isCoolDown2 = false;
                pressed2 = false;
            }
        }
    }

    public void CollectingGameEnabled()
    {
            pressed2 = true;
    }
}
