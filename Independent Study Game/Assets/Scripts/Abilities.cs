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
    public ProgressiveDisclosureHandler pdHandler;
    public Image abilityImage1;
    public Image abilityImage2;
    public Image abilityImage3;

    public GameObject newLevelAlert,powerUpTutorialPanel;

    public GameObject[] powerUps;
    public GameObject createdObject;
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

    bool firstTimeClicked=false, secondTimeClicked=false;
   

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
        abilityImage1.fillAmount = abilityImage2.fillAmount = abilityImage3.fillAmount = 0;

        
        spawnPosition =new Vector3(-12.13f, -2.4f, 0);
        
        dragonPosition = new Vector3(12.13f, -2f, 0);
        
    }

    // Update is called once per frame
    void Update()
    {


            //messenger
            TriggerAbility1();
            //penguin
            TriggerAbility2();
            //dragon
            TriggerAbility3();   
    }


    void TriggerAbility1()
    {
        
        
            if ((Input.GetKey(ability1) || pressed1) && isCoolDown1 == false)
            {
            isCoolDown1 = true;
            ExecuteAbility(powerUps[0], abilityImage1, isCoolDown1, spawnPosition);

             }


            if (isCoolDown1 == true)
            {
            if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
                CoolDown(abilityImage1, coolDown1, ref isCoolDown1, ref pressed1);
            }
        
    }

    void TriggerAbility2()
    {
        if ((Input.GetKey(ability2) || pressed2) && isCoolDown2 == false)
        {
            isCoolDown2 = true;
            ExecuteAbility(powerUps[1], abilityImage2, isCoolDown2, spawnPosition);

            if (firstTimeClicked)
            {
                Debug.Log("summon set");
                Helper setHelper = createdObject.GetComponent<Helper>();
                pdHandler.SetSummon(ref setHelper);
                pdHandler.AdvanceAbilityStep();
                Debug.Log("clicked for the first time");
            }

        }

        if (pdHandler.completedTutorials.Contains("Penguin"))
        {
            if (isCoolDown2 == true)
            {
                if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
                    CoolDown(abilityImage2, coolDown2, ref isCoolDown2, ref pressed2);
            }
        }
    }

    private void TriggerAbility3()
    {
        if ((Input.GetKey(ability3) || pressed3) && isCoolDown3 == false)
        {
            isCoolDown3 = true;
            ExecuteAbility(powerUps[2], abilityImage3, isCoolDown3, dragonPosition);
        }

        if (isCoolDown3 == true)
        {
            if( !(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
            CoolDown(abilityImage3, coolDown3, ref isCoolDown3, ref pressed3);
        }
    }

    private void ExecuteAbility(GameObject powerUp,Image abilityImage, bool isCoolDown, Vector3 spawnPosition)
    {
        abilityImage.fillAmount = 1;

        createdObject = Instantiate(powerUp, spawnPosition, Quaternion.identity);
        createdObject.transform.parent = GameObject.FindGameObjectWithTag("Main Game").transform;
        createdObject.gameObject.SetActive(true);
    }

    private void CoolDown(Image abilityImage, float coolDown, ref bool isCoolDown, ref bool pressed)
    {
        abilityImage.fillAmount -= 1 / coolDown * Time.deltaTime;

        if(abilityImage.fillAmount<=0)
        {
            abilityImage.fillAmount = 0;
            isCoolDown = false;
            pressed = false;
        }
    }

    public void Pressed(string objName)
    {
        if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
        {
            switch (objName)
            {
                case "Messenger":
                    pressed1 = true;
                    if (isCoolDown1 == false)
                        DataMiner.abilityandEventCount[0]++;

                   
                        break;
                case "Penguin":
                    pressed2 = true;
                    CraftingManager.helperName = "Penguin";
                    if (isCoolDown2 == false)
                        DataMiner.abilityandEventCount[1]++;
                    if (gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure && (DataMiner.abilityandEventCount[1] <= 2&&DataMiner.abilityandEventCount[1]>0))
                    {
                        if (DataMiner.abilityandEventCount[1] <= 1)
                            firstTimeClicked = true;
                        else
                            secondTimeClicked = true;

                    }
                    break;
                case "Dragon":
                    pressed3 = true;
                    if (isCoolDown3 == false)
                        DataMiner.abilityandEventCount[2]++;
                    break;
                default:
                    break;
            }



          





        }

        TimeManager.ResetAFKTimer();

    }
}
