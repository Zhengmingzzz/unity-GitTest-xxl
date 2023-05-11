using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBGSlot : MonoBehaviour
{
    public GameObject BGSlotPre;
    public static Vector3[,] BGSlotPos = new Vector3[6, 6];

    private void Awake()
    {
        SetBGSlots();
    }

    private void SetBGSlots()
    {
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                GameObject newBGSlot = Instantiate(BGSlotPre, CorrectPosByXY(x, y), Quaternion.identity) ;
                newBGSlot.transform.SetParent(this.transform);
                BGSlotPos[x, y] = newBGSlot.transform.position;
            }
        }
    }


    private Vector3 CorrectPosByXY(int x,int y)
    {
        return new Vector3(this.transform.position.x-(6*50)/2 + (x*50), this.transform.position.y - (6 * 50) / 2 + (y * 50), 0);
    }
}
