using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Helper : MonoBehaviour
{
  [SerializeField]  ManagerofManagers managerHub;

    public Transform helperTransform;
    Animator helperAnimator;
    float movementSpeed= 1f;
    public GameObject[] slotTriggers;
    private Vector3[] slotPositions;
    public GameObject dragonTrigger;
    public GameObject selectedTile,selectedTile2;

 
    
    Transform tilePosition;
    int slotIndex;
   public int rightBound = 13, leftBound=-14;
   [SerializeField] public Vector3 originPosition,dragonTriggerPosition;

    float ceil;

    [SerializeField] private float dragonCeilOffset = .4f, dragonFirePositionOffset = .1f;
    public GameObject fireBall;
    public GameObject fireBallSpawn;
    public Image fireImage;

    public static bool penguinHasShownIngredient = false;
    public bool hasShot = false;
  [SerializeField]  private float penguinDistanceFromTilePosition = 10;

    public bool summonedReachedPDTutorialDestination=false;
    public bool summonedCanContinuePDTutorial = false;

   [SerializeField] private SummonManager abilityManager;
  public  bool summonReachedPDtutorialOnce=false;

    double fireBallOffSet = 233.85001;

    [SerializeField] float dragonAnimationTime=.5f, messengerAnimationTime=.8f;



    // Start is called before the first frame update
    void Start()
    {

        slotPositions = managerHub.craftingManager.craftingSlotPositions;
        CraftingManager.helperIsActive = true;
        if (gameObject.tag == "Messenger" || gameObject.tag == "Penguin")
        {


            if (gameObject.tag == "Messenger")
            {
                slotIndex = (int) CraftingManager.slotIndexes.firstSlotIndex;
                selectedTile = slotTriggers[slotIndex].gameObject;

                
                    tilePosition = selectedTile.GetComponent<Transform>();
                    selectedTile.SetActive(true);

                managerHub.summonManager.messengerIsOut = true;
            }
            else
            {
                slotIndex = (int)CraftingManager.slotIndexes.secondSlotIndex;
                penguinHasShownIngredient = false;

                managerHub.summonManager.penguinSlot = slotIndex;
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

        if (!GameManager.pause)
        {
            switch (gameObject.tag)
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
        }
        

        if (CheckIfSummonIsOutOfBounds())
        {

            if (managerHub.gameManager.tutorialType!=GameManager.TutorialType.progressiveDisclosure||!managerHub.abilityPDManager.summonPDHandler.NeedSummon())
                Destroy(gameObject);
            else
                summonedCanContinuePDTutorial = false;
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
            if (hasShot)
            {
                for (int i = managerHub.craftingManager.numberOfCraftingSlots-1; i > (int)CraftingManager.slotIndexes.secondSlotIndex; i--)
                { 
                    managerHub.craftingManager.FillInSlot(i,true);     
                }
            }
            if (helperTransform.position.y > originPosition.y + dragonCeilOffset)
            {
                helperTransform.position += new Vector3(0, movementSpeed * Time.deltaTime, 0);

              
            }
            else 
            {
                    helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                //Disable the animations
                helperAnimator.SetBool("Fire", false);
                helperAnimator.SetBool("Jump", false);

            }
        }

        //If the dragon is at the position to fire
        if ((helperTransform.position.x <= dragonTriggerPosition.x+dragonFirePositionOffset))
        {

            //Shoot if it has reached the max height
            if (helperTransform.position.y >= ceil)
            {
                helperTransform.position = helperTransform.position;
                if (!hasShot && !GameObject.FindGameObjectWithTag("Fireball"))
                {
                    Instantiate(fireBall, fireBallSpawn.transform);
                    hasShot = true;
                }

                if (CanFillThirdSlotDragon())
                   managerHub.craftingManager.imageCraftingSlots[(int)CraftingManager.slotIndexes.thirdSlotIndex].sprite = fireImage.sprite;

                if (CanFillFourthSlotDragon())
                   managerHub.craftingManager.imageCraftingSlots[(int)CraftingManager.slotIndexes.fourthSlotIndex].sprite = fireImage.sprite;

                

            }
            else if(dragonTrigger.activeInHierarchy == true) //Move upward if not yet reached to the ceiling
            {
               
                    helperTransform.position += new Vector3(0, -movementSpeed * Time.deltaTime, 0);
                helperAnimator.SetBool("Jump", true);
            }
            StartCoroutine(SummonPutItemInSlot());
        }
      
        else           
                helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);


    }

    //PenguinScript
    private void Penguin()
    {

        movementSpeed = 6f;


        if (CheckIfCanContiue(gameObject.tag) || !summonedReachedPDTutorialDestination)
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);



        if (helperTransform.position.x >= rightBound - 10 && !managerHub.summonManager.penguinItemSuccessfullyDropped)
        {
            if (!penguinHasShownIngredient && managerHub.craftingManager.NeedToFillSlot((int)SummonManager.summonIndex.penguin))
            {
                managerHub.craftingManager.RemoveItemFromSlot(slotIndex);
            }


            //in progressive disclosure tutorial
            if (managerHub.abilityPDManager.summonPDHandler.SummonHasRechedDestinationDuringPDTutorialOnce())
            {
                summonedReachedPDTutorialDestination = true;
                summonReachedPDtutorialOnce = true;
            }

            //Display the item set the in item set 

            if (managerHub.craftingManager.NeedToFillSlot(slotIndex))
            {

                managerHub.craftingManager.setIndex = ReturnPenguinSetIndex();
                foreach (GameObject setOfAllIngredientsAccessible in managerHub.craftingManager.setsOfIngredients)
                {
                    setOfAllIngredientsAccessible.SetActive(false);
                }
                managerHub.craftingManager.setsOfIngredients[managerHub.craftingManager.setIndex].SetActive(true);

                penguinHasShownIngredient = true;
                managerHub.orderManager.listOfOrder[slotIndex].imageOfItem.color = Color.cyan;
            }
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

            if (managerHub.craftingManager.craftingSlots[(int)CraftingManager.slotIndexes.firstSlotIndex].item == managerHub.orderManager.listOfOrder[(int)CraftingManager.slotIndexes.firstSlotIndex])
            {
                managerHub.craftingManager.craftingSlots[(int)CraftingManager.slotIndexes.firstSlotIndex].item.imageOfItem.color = new Color(18.8f, 83.5f, 78.4f);
            }
        

            if(AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
         managerHub.abilityPDManager.summonPDHandler.HandleSummonReachedDestinationForFirstTimeInPDTutorial();


            if(managerHub.craftingManager.NeedToFillSlot((int)SummonManager.summonIndex.messenger))
            StartCoroutine(SummonPutItemInSlot());
            else
            {
                selectedTile.SetActive(false);
            }

        }
        else if (helperTransform.position.x < tilePosition.position.x)
                helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);

        else
        {
            if (CheckIfCanContiue(gameObject.tag))
                helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }
    }



    IEnumerator SummonPutItemInSlot()
    {
        if (gameObject.tag == "Messenger")
        {
            helperAnimator.SetBool("AtSlot", true);
            yield return new WaitForSeconds(messengerAnimationTime);
                managerHub.craftingManager.FillInSlot(slotIndex,true);
            helperAnimator.SetBool("AtSlot", false);
            selectedTile.SetActive(false);
        }

        if (gameObject.tag == "Dragon")
        {
            yield return new WaitForSeconds(dragonAnimationTime);
            helperAnimator.SetBool("Fire", true);

            yield return new WaitForSeconds(dragonAnimationTime);
            helperAnimator.SetBool("Fire", false);

            dragonTrigger.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        CraftingManager.helperIsActive = false;

        switch (gameObject.tag)
        {
            case "Penguin":
                managerHub.summonManager.penguinItemSuccessfullyDropped = false;
                break;
            case "Messenger":
                managerHub.summonManager.messengerIsOut = false;
                break;
            case "Dragon":
                hasShot = false;
                break;
        }
        

    }

    int ReturnPenguinSetIndex()
    {
        return (int)Mathf.Floor(((managerHub.craftingManager.ingredientsList.IndexOf(managerHub.orderManager.listOfOrder[slotIndex])) / (managerHub.craftingManager.setsOfIngredients.Length - 1)));
    }

    bool CheckIfCanContiue(string name)
    {
        return managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || (summonedCanContinuePDTutorial || !summonedReachedPDTutorialDestination) || managerHub.abilityPDManager.completedTutorials.Contains(name);
    }

    private bool CheckIfSummonIsOutOfBounds()
    {
        return ((gameObject.tag == "Messenger" || gameObject.tag == "Penguin") && helperTransform.position.x >= rightBound + 5) || helperTransform.position.x <= leftBound - 5; //Messenger and penguin walk from left to right, dragon walks from right to left
    }

    bool CanFillThirdSlotDragon()
    {
        return (fireBall.GetComponent<Transform>().position.x <= slotPositions[(int)CraftingManager.slotIndexes.thirdSlotIndex].x + fireBallOffSet);
    }

    bool CanFillFourthSlotDragon()
    {
        return (fireBall.GetComponent<Transform>().position.x <= slotPositions[(int)CraftingManager.slotIndexes.fourthSlotIndex].x + fireBallOffSet);
    }

}
