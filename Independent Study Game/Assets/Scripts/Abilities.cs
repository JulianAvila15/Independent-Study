using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Abilities : MonoBehaviour
{

    public CraftingManager craftingMan;
    public GameManager gameManager;
    public Image abilityImage1;
    public Image abilityImage2;
    public Image abilityImage3;

    public GameObject[] powerUps;
    GameObject createdObject;
    [Header("Ability1")]
    public float coolDown1;
    bool isCoolDown1 = false;
    public KeyCode ability1;
    bool pressed1 = false;

    [Header("Ability2")]
    public float coolDown2;
    bool isCoolDown2 = false;
    public KeyCode ability2;
    bool pressed2 = false;

    [Header("Ability3")]
    public float coolDown3;
    bool isCoolDown3 = false;
    public KeyCode ability3;
    bool pressed3 = false;

    public DataMiner miner;
    Vector3 spawnPosition,dragonPosition;

    private void Awake()
    {
        for(int i=0; i<powerUps.Length;i++)
        {
            powerUps[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;

        
        spawnPosition =new Vector3(-12.13f, -2.4f, 0);
        
        dragonPosition = new Vector3(12.13f, -2f, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
            //messenger
            Ability1();
            //penguin
            Ability2();
            //dragon
            Ability3();   
    }


    void Ability1()
    {
        
        
            if ((Input.GetKey(ability1) || pressed1) && isCoolDown1 == false)
            {
                isCoolDown1 = true;
                abilityImage1.fillAmount = 1;
                createdObject = Instantiate(powerUps[0], spawnPosition, Quaternion.identity);
                createdObject.transform.parent = GameObject.FindGameObjectWithTag("Main Game").transform;
                createdObject.gameObject.SetActive(true);
                DataMiner.abilityCount[0]++;
            }


            if (isCoolDown1 == true)
            {
                abilityImage1.fillAmount -= 1 / coolDown1 * Time.deltaTime;

                if (abilityImage1.fillAmount <= 0)
                {
                    abilityImage1.fillAmount = 0;
                    isCoolDown1 = false;
                    pressed1 = false;
                }
            }
        
    }

    void Ability2()
    {
        if ((Input.GetKey(ability2) || pressed2) && isCoolDown2 == false)
        {
            isCoolDown2 = true;
            abilityImage2.fillAmount = 1;
            createdObject = Instantiate(powerUps[1], spawnPosition, Quaternion.identity);
            createdObject.transform.parent = GameObject.FindGameObjectWithTag("Main Game").transform;
            createdObject.gameObject.SetActive(true);
            DataMiner.abilityCount[1]++;
        }


        if (isCoolDown2 == true)
        {
            abilityImage2.fillAmount -= 1 / coolDown1 * Time.deltaTime;

            if (abilityImage2.fillAmount <= 0)
            {
                abilityImage2.fillAmount = 0;
                isCoolDown2 = false;
                pressed2 = false;
                
            }
        }
    }

    private void Ability3()
    {
        if ((Input.GetKey(ability3) || pressed3) && isCoolDown3 == false)
        {
            isCoolDown3 = true;
            abilityImage3.fillAmount = 1;
        
            createdObject = Instantiate(powerUps[2], dragonPosition, Quaternion.identity);
            createdObject.transform.parent = GameObject.FindGameObjectWithTag("Main Game").transform;
            createdObject.gameObject.SetActive(true);
            DataMiner.abilityCount[2]++;
        }


        if (isCoolDown3 == true)
        {
            abilityImage3.fillAmount -= 1 / coolDown1 * Time.deltaTime;

            if (abilityImage3.fillAmount <= 0)
            {
                abilityImage3.fillAmount = 0;
                isCoolDown3 = false;
                pressed3 = false;

            }
        }
    }

    public void Pressed(string objName)
    {
            switch (objName)
            {
                case "Messenger":
                    pressed1 = true;
                    break;
                case "Penguin":
                    pressed2 = true;
                    CraftingManager.helperName = "Penguin";
                    break;
                case "Dragon":
                        pressed3 = true;
                    break;
                default:
                    break;
            }
        
       

    }
}
