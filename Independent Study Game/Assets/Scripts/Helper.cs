using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Helper : MonoBehaviour
{
    public Transform helperTransform;
    Animator helperAnimator;
    float movementSpeed= 1f;
    public GameObject[] slotTriggers,slots;
    public GameObject dragonTrigger;
    GameObject selectedTile,selectedTile2;

 
    
    Transform tilePosition;
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public ProgressiveDisclosureHandler pdManager;
    int slotIndex;
   public int rightBound = 13, leftBound=-14;
    private Vector3 originPosition,dragonTriggerPosition;

    float ceil;

    public GameObject fireBall;
    public GameObject fireBallSpawn;
    public Image fireImage;

    public static bool penguinHasShownIngredient = false;
    bool hasShot = false;
  private static  Color iceBlue = new Color(0.69f, 1f, 1f);

    public bool summonedReachedPDTutorialDestination=false;
    public bool summonedCanContinuePDTutorial = false;

    public Abilities abilityManager;
    

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Messenger" || gameObject.tag == "Penguin")
        {


            if (gameObject.tag == "Messenger")
            {
                slotIndex = 0;
                selectedTile = slotTriggers[slotIndex].gameObject;

                
                    tilePosition = selectedTile.GetComponent<Transform>();
                    selectedTile.SetActive(true);
            }
            else
            {
                slotIndex = 1;
                penguinHasShownIngredient = false;
                CraftingManager.penguinSlot = slotIndex;
            }


            originPosition = new Vector3(-13.6f, -2.4f, 0);
        }
        else {
            originPosition = new Vector3(13f, -2.4f, 0);
           
            dragonTriggerPosition = dragonTrigger.GetComponent<Transform>().position;
            
        }

        



        helperTransform = gameObject.GetComponent<Transform>().transform;
        helperAnimator = gameObject.GetComponent<Animator>();
        ceil = helperTransform.position.y + 2f;



    }

    // Update is called once per frame
    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //Depending upon the name, they will do something specific

        switch(gameObject.tag)
        {
            case "Messenger":
                Messenger();
                break;
            case "Penguin":
                Penguin();
                break;
            case "Dragon":
                Dragon();
                break;
            default:
                break;
        }



        if ((gameObject.tag == "Messenger" || gameObject.tag == "Penguin") && helperTransform.position.x >= rightBound+10|| helperTransform.position.x <= leftBound-10)
        {

            if (!ProgressiveDisclosureHandler.abilityTutorialTriggered)
                Destroy(gameObject);
            else
            {
                helperTransform.position = originPosition;
                summonedCanContinuePDTutorial = false;
            }

            if(dragonTrigger!=null&&dragonTrigger.activeInHierarchy==false)
            {
                dragonTrigger.SetActive(true);
            }


        }



        }
    //Dragon Script
    private void Dragon()
    {
        movementSpeed = -3.5f;
        //If the fireball has been shot, bring the dragon back down to the floor
        if (dragonTrigger.activeInHierarchy == false)
        {

            //fireball has been released and destroyed
            if (!GameObject.FindGameObjectWithTag("Fireball"))
            {
                if (hasShot == false)
                {

                    for (int i = 0; i < 2; i++)
                        craftingManager.CreateSlot(i+2);

                    hasShot = true;
                }
            }
            if (helperTransform.position.y > originPosition.y+.4)
            helperTransform.position += new Vector3(0, movementSpeed * Time.deltaTime, 0);
            else
            {
                helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                //Disable the animations
                helperAnimator.SetBool("Fire", false);
                helperAnimator.SetBool("Jump", false);
                
            }
        }

        //If the dragon is at the position to fire
        if ((helperTransform.position.x <= dragonTriggerPosition.x+.1))
        {
           

            //Shoot if it has reached the max height
            if (helperTransform.position.y >= ceil)
            {
                helperTransform.position = helperTransform.position;
                if (!GameObject.FindGameObjectWithTag("Fireball"))
                    Instantiate(fireBall, fireBallSpawn.transform);

                if (fireBall.GetComponent<Transform>().position.x <= slots[2].GetComponent<Transform>().position.x + 233.85001)
                {

                    slots[2].GetComponent<Image>().sprite = fireImage.sprite;
                }
                if (fireBall.GetComponent<Transform>().position.x <= slots[3].GetComponent<Transform>().position.x + 233.85001)
                {
                    slots[3].GetComponent<Image>().sprite = fireImage.sprite;
                }
            }
            else if( dragonTrigger.activeInHierarchy == true) //Move upward if not yet reached to the ceiling
            {
                helperTransform.position += new Vector3(0, -movementSpeed * Time.deltaTime, 0);
                helperAnimator.SetBool("Jump", true);
            }
            StartCoroutine(PauseSeconds());
        }
      
        else
        {
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }


    }

    //PenguinScript
    private void Penguin()
    {

            movementSpeed = 6f;


        if (craftingManager.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || (summonedCanContinuePDTutorial|| !summonedReachedPDTutorialDestination)|| pdManager.completedTutorials.Contains("Penguin"))
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
       


        if (helperTransform.position.x >= rightBound-10)
        {


            //in progressive disclosure tutorial
            if (craftingManager.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !summonedReachedPDTutorialDestination)
            {
                Debug.Log("summon reached destination");
                summonedReachedPDTutorialDestination = true;
                Debug.Log(summonedReachedPDTutorialDestination);
            }

            //Display the item set the in item set 
            craftingManager.setIndex = (int)Mathf.Floor(((craftingManager.ingredientsList.IndexOf(orderManager.listOfOrder[slotIndex])) / (craftingManager.setsOfIngredients.Length-1)));
            foreach (GameObject setOfAllIngredientsAccessible in craftingManager.setsOfIngredients)
            {
                setOfAllIngredientsAccessible.SetActive(false);
            }

          

            craftingManager.setsOfIngredients[craftingManager.setIndex].SetActive(true);

            craftingManager.craftingSlots[slotIndex].GetComponent<Image>().color =iceBlue;

            GameObject.Find(orderManager.listOfOrder[slotIndex].name).GetComponent<Image>().color = iceBlue;
            CraftingManager.helperIsActive = false;
            penguinHasShownIngredient = true;



            ResetToNormalColor();


        }

    }

    private void ResetToNormalColor()
    {
        if (craftingManager.craftingSlots[slotIndex].item!=null && craftingManager.craftingSlots[slotIndex].item.itemName==orderManager.listOfOrder[slotIndex].itemName)
        {
            craftingManager.craftingSlots[slotIndex].GetComponent<Image>().color = new Color(255, 255, 255);
            GameObject.Find(orderManager.listOfOrder[slotIndex].name).GetComponent<Image>().color = new Color(255, 255, 255);
            
        }
    
  
    }

    //MessengerBoy Script
    private void Messenger()
    {
        float movementSpeed = 4f;
        //movement of helper character
        if ((helperTransform.position.x >= tilePosition.position.x) && selectedTile.activeInHierarchy)
        {
            helperTransform.position = helperTransform.position;

            if (craftingManager.craftingSlots[0].item == orderManager.listOfOrder[0])
            {
                craftingManager.craftingSlots[0].item.GetComponent<Image>().color = new Color(18.8f, 83.5f, 78.4f);
            }
            StartCoroutine(PauseSeconds());

        }
        else if (helperTransform.position.x < tilePosition.position.x)
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);

        else
        {
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }
    }

    IEnumerator PauseSeconds()
    {
        if (gameObject.tag == "Messenger")
        {
            helperAnimator.SetBool("AtSlot", true);
            yield return new WaitForSeconds(.8f);
                craftingManager.CreateSlot(slotIndex);
            helperAnimator.SetBool("AtSlot", false);
            selectedTile.SetActive(false);
        }

        if (gameObject.tag == "Dragon")
        {
            yield return new WaitForSeconds(0.5f);
            helperAnimator.SetBool("Fire", true);

            yield return new WaitForSeconds(0.5f);
            helperAnimator.SetBool("Fire", false);
            dragonTrigger.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        CraftingManager.helperIsActive = false;
       
        
    }
}
