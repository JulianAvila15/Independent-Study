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
     [SerializeField] private IntroProgressiveDisclosureHandler pdHandler;
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

    [SerializeField] private GameObject mainGame;

    [SerializeField] private AbilityTutorialProgressiveDisclosureHandler abilityPDTutorialManager;

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
        if(pressed1)
        TriggerAbility(ref pressed1, ref isCoolDown1,"Messenger", ability1,0,spawnPosition,ref abilityImage1,coolDown1);
        //penguin
        if (pressed2) 
            TriggerAbility(ref pressed2, ref isCoolDown2, "Penguin", ability2, 1, spawnPosition, ref abilityImage2, coolDown2);
        
        //dragon
        if(pressed3)
        TriggerAbility(ref pressed3, ref isCoolDown3, "Dragon", ability3, 2, dragonPosition, ref abilityImage3, coolDown3);
    }

    void TriggerAbility(ref bool pressed, ref bool isCoolDown, string abilityName, KeyCode abilityKeyCode, int powerUpIndex, Vector3 spawnPosition, ref Image abilityImage,float coolDown)
    {
        if (pressed && isCoolDown == false)
        {
            isCoolDown = true;
            ExecuteAbility(powerUps[powerUpIndex], abilityImage, isCoolDown, spawnPosition);

            if (!Helper.penguinHasShownIngredient&&CraftingManager.helperName == "Penguin")
                craftingMan.penguinItemSuccessfullyDropped = false;

            if (abilityPDTutorialManager!=null&&AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                abilityPDTutorialManager.OnAbilityButtonClicked();

        }

        if (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || abilityPDTutorialManager.completedTutorials.Contains(abilityName)&&abilityPDTutorialManager.completedTutorials.Count>0 || abilityPDTutorialManager.CheckIfCoolDownEnabled())
        {
            if (isCoolDown == true)
            {
                if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
                    CoolDown(abilityImage, coolDown, ref isCoolDown, ref pressed);
            }
        }

    }

    private void ExecuteAbility(GameObject powerUp,Image abilityImage, bool isCoolDown, Vector3 spawnPosition)
    {
        abilityImage.fillAmount = 1;
        createdObject = Instantiate(powerUp, spawnPosition, Quaternion.identity);
        createdObject.transform.parent = mainGame.transform;
        createdObject.gameObject.SetActive(true);
    }

    private void CoolDown(Image abilityImage, float coolDown, ref bool isCoolDown, ref bool pressed)
    {

        if (!GameManager.pause)
        {
            abilityImage.fillAmount -= 1 / coolDown * Time.deltaTime;

            if (abilityImage.fillAmount <= 0)
            {
                abilityImage.fillAmount = 0;
                isCoolDown = false;
                pressed = false;
            }
        }
    }

    public void Pressed(string objName)
    {
        if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
        {

            switch (objName)
            {
                case "Messenger":
                    if (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || (abilityPDTutorialManager.currentAbilityTutorialData != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && abilityPDTutorialManager.currentAbilityTutorialData.abilityName == "Messenger"))
                    {
                        pressed1 = true;
                        if (isCoolDown1 == false)
                            DataMiner.abilityandEventCount[0]++;
                    }
                    break;
                case "Penguin":
                    if (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && abilityPDTutorialManager.currentAbilityTutorialData.abilityName == "Penguin")
                    {
                        pressed2 = true;
                        CraftingManager.helperName = "Penguin";
                        if (isCoolDown2 == false)
                            DataMiner.abilityandEventCount[1]++;
                    }
                    break;
                case "Dragon":
                    if (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && abilityPDTutorialManager.currentAbilityTutorialData.abilityName == "Dragon")
                    {
                        pressed3 = true;
                        if (isCoolDown3 == false)
                            DataMiner.abilityandEventCount[2]++;
                    }
                    break;
                default:
                    break;
            }



          





        }

        TimeManager.ResetAFKTimer();

    }
}
