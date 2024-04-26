using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Helper : MonoBehaviour
{
    Transform helperTransform;
    Animator helperAnimator;
    float movementSpeed= 1f;
    public GameObject[] slotTriggers,slots;
    GameObject selectedTile,selectedTile2;

    Transform tilePosition,tilePosition2;
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    int slotIndex;
    int rightBound = 13;
    private Vector3 originPosition ;

    // Start is called before the first frame update
    void Start()
    {
        originPosition= new Vector3(-13.6f, -3.410614f,0);
        helperTransform = gameObject.GetComponent<Transform>().transform;
        helperAnimator = gameObject.GetComponent<Animator>();
        SelectRandomTile();
         tilePosition = selectedTile.GetComponent<Transform>();
        tilePosition2 = selectedTile2.GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //Depending upon the name, they will do something specific

        switch(gameObject.name)
        {
            case "MessengerBoy":
                Messenger();
                break;
            case "Penguin":
                Penguin();
                break;
        }
            

        if (gameObject.name == "MessengerBoy")
        {
            Messenger();
        }

        if (gameObject.name=="Penguin")
        {

        }



        if (helperTransform.position.x >= rightBound)
        {
            helperTransform.position = originPosition;
            gameObject.SetActive(false);
        }

    }

    private void Penguin()
    {
        movementSpeed = 5f;
        if (helperTransform.position.x < tilePosition.position.x || helperTransform.position.x < tilePosition2.position.x)
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        else
        {
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void Messenger()
    {
        float movementSpeed = 3f;
        //movement of helper character
        if ((helperTransform.position.x >= tilePosition.position.x || helperTransform.position.x >= tilePosition2.position.x) && selectedTile.activeInHierarchy)
        {
            helperTransform.position = helperTransform.position;
            // Debug.Log("true");
            StartCoroutine(PauseSeconds());
        }
        else if (helperTransform.position.x < tilePosition.position.x || helperTransform.position.x < tilePosition2.position.x)
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);

        else
        {
            helperTransform.position = helperTransform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }
    }

    IEnumerator PauseSeconds()
    {
        helperAnimator.SetBool("AtSlot",true);
        yield return new WaitForSeconds(3.0f);
        CreateSlot();
        helperAnimator.SetBool("AtSlot", false);
        selectedTile.SetActive(false);
    }

    void CreateSlot()
    {
        craftingManager.finalOrderList[slotIndex] = orderManager.listOfOrder[slotIndex];
        slots[slotIndex].GetComponent<Image>().sprite = orderManager.listOfOrder[slotIndex].GetComponent<Image>().sprite;
    }

    void SelectRandomTile()
    {
        slotIndex = Random.Range(0, slotTriggers.Length);
       selectedTile =  slotTriggers[slotIndex];
        slotIndex = Random.Range(0, slotTriggers.Length);

        selectedTile2 = slotTriggers[slotIndex];
        while (selectedTile2!=selectedTile)
        {
            slotIndex = Random.Range(0, slotTriggers.Length);
            selectedTile2 = slotTriggers[slotIndex];
        }
       
    }
}
