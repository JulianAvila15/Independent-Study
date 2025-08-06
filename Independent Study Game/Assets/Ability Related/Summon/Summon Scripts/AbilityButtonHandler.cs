using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class AbilityButtonHandler : MonoBehaviour
{
    Color abilityButtonColor;
   [SerializeField] private Button abilityButton;
  [SerializeField]  private Image abilityButtonImage,abilityImage;

    [SerializeField] int[] abilitySlotNum;

   [SerializeField]  private ManagerofManagers managerHub;

    int firstChosenSlotIndex = 0, secondChosenSlotIndex = 1;
    bool canDisable=false;
    // Start is called before the first frame update
    void Start()
    {
        abilityButtonImage = abilityButton.GetComponent<Image>();
        abilityButtonColor = abilityButtonImage.color;
    }

    // Update is called once per frame
    void Update()
    {

        if ((!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || managerHub.abilityPDManager.GetStepTutorialType() != TutorialStepType.FlashButton))
        {

            HandleSummonButtonEnableorDisable();
        }
    }

    private void HandleSummonButtonEnableorDisable()
    {

        canDisable = (abilitySlotNum.Length >= 2) ? (CheckIfNeedToBeDisabled(abilitySlotNum[firstChosenSlotIndex], abilitySlotNum[secondChosenSlotIndex])) : CheckIfNeedToBeDisabled(abilitySlotNum[firstChosenSlotIndex]);

        if (canDisable)
        {
            abilityButton.interactable = false;
            abilityImage.color = abilityButtonImage.color = abilityButton.colors.disabledColor;
        }
        else
        {
            abilityButton.interactable = true;
           abilityButtonImage.color = abilityButtonColor;
            abilityImage.color = Color.white;
        }
    }

    bool CheckIfNeedToBeDisabled(int slot1, int slot2Index=-1)
    {
        bool needToBeDisabled = false;
       needToBeDisabled= !managerHub.craftingManager.NeedToFillSlot(slot1);

        if(slot2Index>0)
          needToBeDisabled &= !managerHub.craftingManager.NeedToFillSlot(slot2Index);

        return needToBeDisabled;

    }
}
