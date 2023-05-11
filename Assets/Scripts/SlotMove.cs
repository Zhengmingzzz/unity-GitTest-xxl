using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlotMove : MonoBehaviour
{
    private GameSlot gameSlot;
    private Transform slotTransfrom;

    private void Awake()
    {
        gameSlot = this.GetComponent<GameSlot>();
        slotTransfrom = this.GetComponent<Transform>();
    }
    public void MoveToNewPos(int newX, int newY)
    {
        
        slotTransfrom.DOMove(SetBGSlot.BGSlotPos[newX, newY], 0.1f);
        gameSlot.Init(newX, newY, gameSlot.SlotType);
        //slotTransfrom.position = SetBGSlot.BGSlotPos[newX, newY];
    }

    

  
}
