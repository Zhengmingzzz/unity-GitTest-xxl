using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct slotPrefabSt
    {
        public enums.soltType slotType;
        public GameObject targetSlotPre;
    }
    public slotPrefabSt[] slotPreStructs;
    private Dictionary<enums.soltType, GameObject> soltPrefabDic = new Dictionary<enums.soltType, GameObject>();

    

    private GameSlot[,] slots = new GameSlot[6, 6];
    public Transform slotParent;

    private GameSlot[] clickSlots = new GameSlot[2];
    private int clickSlotsNum = 0;


    private void Awake()
    {
        saveStructArrayToDictionary();
    }
    private void Start()
    {
        InitSlotWhenBegin();
        StartCoroutine(AllFill());

    }
    private void OnEnable()
    {
        EventHandler.ClickEvent += GetClickSlotInfo;
    }

    private void OnDisable()
    {
        EventHandler.ClickEvent -= GetClickSlotInfo;
    }
    /// <summary>
    /// 将方格类型 方格颜色的结构体数组存入字典
    /// </summary>
    private void saveStructArrayToDictionary()
    {
        for (int i = 0; i < (int)enums.soltType.COUNT; i++)
        {
            if (!soltPrefabDic.ContainsKey(slotPreStructs[i].slotType))
            {
                soltPrefabDic.Add(slotPreStructs[i].slotType, slotPreStructs[i].targetSlotPre);
            }
        }
        
    }


    private void InitSlotWhenBegin()
    {
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                //WorkFlow:
                slots[x, y] = CreateNewSlot(x, y, enums.soltType.EMPTY);
            }
        }
    }

    public GameSlot CreateNewSlot(int x, int y, enums.soltType slotType)
    {
        GameObject newSlot =  Instantiate(soltPrefabDic[slotType], SetBGSlot.BGSlotPos[x, y], Quaternion.identity);

        newSlot.transform.SetParent(slotParent);

        slots[x, y] = newSlot.GetComponent<GameSlot>();
        slots[x, y].Init(x, y, slotType);

        if (slots[x, y].CanRender())
        {
            slots[x, y].RenderComponent.RenderSlotColor((enums.slotColor)Random.Range(0, (int)enums.slotColor.COUNT));
        }

        return slots[x, y];
    }

    public GameSlot CreateNewSlot(int x, enums.soltType slotType)
    {
        GameObject newSlot = Instantiate(soltPrefabDic[slotType], new Vector3(SetBGSlot.BGSlotPos[x, 6 - 1].x, SetBGSlot.BGSlotPos[x, 6 - 1].y + 30), Quaternion.identity);

        newSlot.transform.SetParent(slotParent);

        slots[x, 6 - 1] = newSlot.GetComponent<GameSlot>();
        slots[x, 6 - 1].Init(x, 6 - 1, slotType);

        if (slots[x, 6 - 1].CanRender())
        {
            slots[x, 6 - 1].RenderComponent.RenderSlotColor((enums.slotColor)Random.Range(0, (int)enums.slotColor.COUNT));

        }

        return slots[x, 6 - 1];
    }

    private IEnumerator AllFill()
    {
        while (Fill())
        {
            yield return new WaitForSeconds(0.1f);
        }

    }

    private bool Fill()
    {
        bool haveDoFill = false;
        for (int y = 1; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                GameSlot thisSlot = slots[x, y];
                GameSlot targetSlot = slots[x, y - 1];

                if (thisSlot.CanMove())
                {

                    if (targetSlot.SlotType == enums.soltType.EMPTY && thisSlot.SlotType != enums.soltType.EMPTY)
                    {
                        thisSlot.MoveComponent.MoveToNewPos(x, y - 1);
                        targetSlot.MoveComponent.MoveToNewPos(x, y);
                        thisSlot.Init(x, y - 1, enums.soltType.NORMAL);
                        targetSlot.Init(x, y, enums.soltType.EMPTY);

                        slots[x, y] = targetSlot;
                        slots[x, y - 1] = thisSlot;

                        haveDoFill = true;
                    }
                }
            }
        }


        //最上面一行的填充
        for (int x = 0; x < 6; x++)
        {
            if (slots[x, 6 - 1].SlotType == enums.soltType.EMPTY)
            {
                Destroy(slots[x, 6 - 1].gameObject);
                GameSlot newGameSlot = CreateNewSlot(x, enums.soltType.NORMAL);
                newGameSlot.MoveComponent.MoveToNewPos(x, 6 - 1);
                newGameSlot.RenderComponent.RenderSlotColor((enums.slotColor)Random.Range(0, (int)enums.slotColor.COUNT));
                haveDoFill = true;
            }
        }

        return haveDoFill;
    }
    private void GetClickSlotInfo(GameSlot gameSlot)
    {
        Debug.Log(1);
        clickSlots[clickSlotsNum] = gameSlot;
        clickSlotsNum++;
        if (clickSlotsNum == 2)
        {
            clickSlotsNum = 0;

            //判断二者位置是否相邻
            if (isAdjacent(clickSlots[0], clickSlots[1]))
            {
                int x0 = clickSlots[0].X;
                int y0 = clickSlots[0].Y;
                int x1 = clickSlots[1].X;
                int y1 = clickSlots[1].Y;

                //判断能否交换---交换后能否消除
                List<GameSlot> SignSlots1 = SignSlotAfterClick(clickSlots[0], x1, y1);
                List<GameSlot> SignSlots2 = SignSlotAfterClick(clickSlots[1], x0, y0);

                //  可以消除
                //1 交换位置
                //2 消除方块
                if (SignSlots1.Count > 0 || SignSlots2.Count > 0)
                {
                    //1
                    List<GameSlot> FinishedSignSlotsList = new List<GameSlot>();

                    for (int i = 0; i < SignSlots1.Count; i++)
                    {
                        if(!FinishedSignSlotsList.Contains(SignSlots1[i]))
                            FinishedSignSlotsList.Add(SignSlots1[i]);
                    }
                    for (int i = 0; i < SignSlots2.Count; i++)
                    {
                        if(!FinishedSignSlotsList.Contains(SignSlots2[i]))
                            FinishedSignSlotsList.Add(SignSlots2[i]);
                    }


                    clickSlots[0].MoveComponent.MoveToNewPos(x1, y1);
                    clickSlots[1].MoveComponent.MoveToNewPos(x0, y0);

                    var t = slots[x0, y0];
                    slots[x0, y0] = slots[x1, y1];
                    slots[x1, y1] = t;


                    //2
                    for (int i = 0; i < FinishedSignSlotsList.Count; i++)
                    {
                        int x = FinishedSignSlotsList[i].X;
                        int y = FinishedSignSlotsList[i].Y;
                        Destroy(FinishedSignSlotsList[i].gameObject);
                        CreateNewSlot(x, y, enums.soltType.EMPTY);
                    }

                }
                else
                {
                    return;
                }

                StartCoroutine(AllFill());


            }
        }
    }
    private bool isAdjacent(GameSlot slot1, GameSlot slot2)
    {
        bool adj = false;

        int adjX = Mathf.Abs(slot1.X - slot2.X);
        int adjY = Mathf.Abs(slot1.Y - slot2.Y);

        if (adjX == 0 && adjY == 1 || adjX == 1 && adjY == 0)
        {
            adj = true;
        }

        return adj;
    }


    private List<GameSlot> SignSlotAfterClick(GameSlot slot1,int newX,int newY)
    {
        List<GameSlot> FinishedList = new List<GameSlot>();
        List<GameSlot> LineList = new List<GameSlot>();
        List<GameSlot> RowList = new List<GameSlot>();

        LineList.Add(slot1);
        RowList.Add(slot1);

        //行检测
        for (int i = 0; i < 2; i++)
        {
            for (int xDistance = 1; xDistance < 6; xDistance++)
            {
                int t_x = newX;

                //i = 0 向左
                if (i == 0)
                {
                    t_x -= xDistance;
                }
                else
                {
                    t_x += xDistance;
                }

                if (t_x < 0 || t_x >= 6)
                {
                    break;
                }

                if (slots[t_x, newY].RenderComponent.SlotColor == slot1.RenderComponent.SlotColor)
                {
                    LineList.Add(slots[t_x, newY]);
                }
                else
                {
                    break;
                }
            }
        }

        //列检测
        for (int i = 0; i < 2; i++)
        {
            for (int yDistance = 1; yDistance < 6; yDistance++)
            {
                int t_y = newY;

                //i = 0 向下
                if (i == 0)
                {
                    t_y -= yDistance;
                }
                else
                {
                    t_y += yDistance;
                }

                if (t_y < 0 || t_y >= 6)
                {
                    break;
                }

                if (slots[newX, t_y].RenderComponent.SlotColor == slot1.RenderComponent.SlotColor)
                {
                    RowList.Add(slots[newX, t_y]);
                }
                else
                {
                    break;
                }
            }
        }


        if (LineList.Count > 2)
        {
            int LineListCount = LineList.Count;
            List<GameSlot> tempRowListInLineList = new List<GameSlot>();
            for (int i = 0; i < LineListCount; i++)
            {
                int x = 0;
                if (i == 0)
                {
                    x = newX;
                }
                else
                {
                    x = LineList[i].X;
                }
                tempRowListInLineList.Add(LineList[i]);

                //对每个元素进行列检测  ---  对y进行操作 x不变
                for (int j = 0; j < 2; j++)
                {
                    for (int yDistance = 1; yDistance < 6; yDistance++)
                    {
                        int y = 0;
                        if (i == 0)
                        {
                            y = newY;
                        }
                        else
                        {
                            y = LineList[i].Y;
                        }
                        //j=0向下
                        if (j == 0)
                        {
                            y -= yDistance;
                        }
                        else
                        {
                            y += yDistance;
                        }

                        if (y < 0 || y >= 6)
                        {
                            break;
                        }

                        if (slots[x, y].RenderComponent.SlotColor == LineList[i].RenderComponent.SlotColor)
                        {
                            tempRowListInLineList.Add(slots[x, y]);
                        }
                        else
                        {
                            break;
                        }

                    }
                }

                if (tempRowListInLineList.Count > 2)
                {
                    for (int ti = 0; ti < tempRowListInLineList.Count; ti++)
                    {
                        if (!LineList.Contains(tempRowListInLineList[ti]))
                            LineList.Add(tempRowListInLineList[ti]);
                    }
                }

                tempRowListInLineList.Clear();
            }
        }

        if (RowList.Count > 2)
        {
            List<GameSlot> tempLineListInRowList = new List<GameSlot>();
            int RowListCount = RowList.Count;

            for (int i = 0; i < RowListCount; i++)
            {
                int y = RowList[i].Y;
                if (i == 0)
                {
                    y = newY;
                }
                tempLineListInRowList.Add(RowList[i]);
                //在纵向列表中进行横向检测   x变y不变

                for (int j = 0; j < 2; j++)
                {

                    for (int xDistance = 1; xDistance < 6; xDistance++)
                    {
                        int x = RowList[i].X;
                        if (i == 0)
                        {
                            x = newX;
                        }
                        //j=0 向左
                        if (j == 0)
                        {
                            x -= xDistance;
                        }
                        else
                        {
                            x += xDistance;
                        }

                        if (x < 0 || x >= 6)
                        {
                            break;
                        }

                        if (slots[x, y].RenderComponent.SlotColor == RowList[i].RenderComponent.SlotColor)
                        {
                            tempLineListInRowList.Add(slots[x, y]);
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                if (tempLineListInRowList.Count > 2)
                {
                    for (int ti = 0; ti < tempLineListInRowList.Count; ti++)
                    {
                        if (!RowList.Contains(tempLineListInRowList[ti]))
                            RowList.Add(tempLineListInRowList[ti]);
                    }
                }
                tempLineListInRowList.Clear();
            }
            
        }


        //填入finishlist中
        if (LineList.Count > 2)
        {
            for (int i = 0; i < LineList.Count; i++)
            {
                if (!FinishedList.Contains(LineList[i]))
                    FinishedList.Add(LineList[i]);
            }
        }

        if (RowList.Count > 2)
        {
            for (int i = 0; i < RowList.Count; i ++)
            {
                if(!FinishedList.Contains(RowList[i]))
                    FinishedList.Add(RowList[i]);
            }
        }

        return FinishedList;

    }
    //private void switchSlotPos( GameSlot gameslot)
    //{
    //    clickSlots[clickNum] = gameslot;
    //    clickNum++;
    //    gameslot.isSelect = !gameslot.isSelect;
    //    gameslot.Animator.SetBool("isSelect", gameslot.isSelect);




    //    if (clickNum == 2)
    //    {
    //        clickSlots[0].isSelect = false;
    //        clickSlots[1].isSelect = false;
    //        clickSlots[0].Animator.SetBool("isSelect", false);
    //        clickSlots[1].Animator.SetBool("isSelect", false);



    //        clickNum = 0;

    //        int x0 = clickSlots[0].X;
    //        int y0 = clickSlots[0].Y;

    //        int x1 = clickSlots[1].X;
    //        int y1 = clickSlots[1].Y;


    //        if (!(Mathf.Abs(x0 - x1) == 0 && Mathf.Abs(y0 - y1) == 1 || Mathf.Abs(x0 - x1) == 1 && Mathf.Abs(y0 - y1) == 0))
    //            return;


    //        clickSlots[0].MoveComponent.MoveToNewPos(x1, y1);
    //        clickSlots[1].MoveComponent.MoveToNewPos(x0, y0);
    //        clickSlots[0].Init(x1, y1, enums.soltType.NORMAL);
    //        clickSlots[1].Init(x0, y0, enums.soltType.NORMAL);

    //        slots[x0, y0] = clickSlots[1];
    //        slots[x1, y1] = clickSlots[0];

    //        List<GameSlot> CheckGameSlots1 = MatchSlots(x0, y0);
    //        List<GameSlot> CheckGameSlots2 = MatchSlots(x1, y1);

    //        if (CheckGameSlots1 == null && CheckGameSlots2 == null)
    //        {
    //            clickSlots[0].MoveComponent.MoveToNewPos(x0, y0);
    //            clickSlots[1].MoveComponent.MoveToNewPos(x1, y1);
    //            clickSlots[0].Init(x0, y0, enums.soltType.NORMAL);
    //            clickSlots[1].Init(x1, y1, enums.soltType.NORMAL);

    //            slots[x0, y0] = clickSlots[0];
    //            slots[x1, y1] = clickSlots[1];
    //            return;
    //        }

    //        if (CheckGameSlots1 != null)
    //        {
    //            for (int i = 0; i < CheckGameSlots1.Count; i++)
    //            {
    //                if (CheckGameSlots1[i].CanDestroy())
    //                {
    //                    int slotX = CheckGameSlots1[i].X;
    //                    int slotY = CheckGameSlots1[i].Y;
    //                    CheckGameSlots1[i].SlotDestroyComponent.DestroySlot();
    //                    GameSlot newGameSlot = CreateNewSlot(slotX,slotY, enums.soltType.EMPTY);
    //                    newGameSlot.MoveComponent.MoveToNewPos(slotX, slotY);
    //                }
    //            }
    //        }
    //        if (CheckGameSlots2 != null)
    //        {
    //            for (int i = 0; i < CheckGameSlots2.Count; i++)
    //            {
    //                if (CheckGameSlots2[i].CanDestroy())
    //                {
    //                    int slotX = CheckGameSlots2[i].X;
    //                    int slotY = CheckGameSlots2[i].Y;
    //                    CheckGameSlots2[i].SlotDestroyComponent.DestroySlot();
    //                    GameSlot newGameSlot = CreateNewSlot(slotX, slotY, enums.soltType.EMPTY);
    //                    newGameSlot.MoveComponent.MoveToNewPos(slotX, slotY);
    //                }
    //            }
    //        }

    //        StartCoroutine(AllFill());
    //    }

    //}

    //public List<GameSlot> MatchSlots(int newX,int newY)
    //{
    //    List<GameSlot> CheckRowList = new List<GameSlot>();
    //    List<GameSlot> CheckLineList = new List<GameSlot>();
    //    List<GameSlot> FinishedList = new List<GameSlot>();
    //    GameSlot gameSlot = slots[newX, newY];
    //    CheckLineList.Add(gameSlot);
    //    CheckRowList.Add(gameSlot);



    //    for (int i = 0; i < 2; i++)
    //    {
    //        for (int xDistance = 1; xDistance < 6; xDistance++)
    //        {
    //            int x = newX;
    //            // i = 0 向左
    //            if (i == 0)
    //            {
    //                x -= xDistance;
    //            }
    //            // i = 1 向右
    //            else if (i == 1)
    //            {
    //                x += xDistance;
    //            }
    //            if (x < 0 || x >= 6)
    //            {
    //                break;
    //            }

    //            if (slots[x, newY].CanRender() && slots[x, newY].SlotRenderComponent.ColorType != gameSlot.SlotRenderComponent.ColorType)
    //            {
    //                break;
    //            }
    //            CheckLineList.Add(slots[x, newY]);

    //        }
    //    }
    //    for (int i = 0; i < 2; i++)
    //    {
    //        for (int yDistance = 1; yDistance < 6; yDistance++)
    //        {
    //            int y = newY;
    //            //i = 0向上
    //            if (i == 0)
    //            {
    //                y += yDistance;
    //            }
    //            //i = 1 向下
    //            if (i == 1)
    //            {
    //                y -= yDistance;
    //            }
    //            if (y < 0 || y >= 6)
    //            {
    //                break;
    //            }

    //            if (slots[newX, y].CanRender() && slots[newX, y].SlotRenderComponent.ColorType != gameSlot.SlotRenderComponent.ColorType)
    //            {
    //                break;
    //            }
    //            CheckRowList.Add(slots[newX, y]);

    //        }
    //    }

    //    if (CheckRowList.Count > 2)
    //    {
    //        List<GameSlot> tempLineGameSlot = new List<GameSlot>();
    //        int checkRowListCount = CheckRowList.Count;
    //        for (int i = 0; i < checkRowListCount; i++)
    //        {
    //            int Sy = CheckRowList[i].Y;
    //            tempLineGameSlot.Add(CheckRowList[i]);
    //            //对列列表中的每个元素进行行遍历
    //            for (int j = 0; j < 2; j++)
    //            {
    //                for (int xDistance = 1; xDistance < 6; xDistance++)
    //                {
    //                    int x = CheckRowList[i].X;

    //                    //0代表左
    //                    if (j == 0)
    //                    {
    //                        x -= xDistance;
    //                    }
    //                    else
    //                    {
    //                        x += xDistance;
    //                    }
    //                    if (x < 0 || x >= 6)
    //                    {
    //                        break;
    //                    }
    //                    if (slots[x, Sy].SlotRenderComponent.ColorType != CheckRowList[i].SlotRenderComponent.ColorType)
    //                    {
    //                        break;
    //                    }
    //                    tempLineGameSlot.Add(slots[x, Sy]);
    //                }
    //            }
    //            if (tempLineGameSlot.Count > 2)
    //            {
    //                for (int ti = 0; ti < tempLineGameSlot.Count; ti++)
    //                {
    //                    CheckRowList.Add(tempLineGameSlot[ti]);
    //                }
    //            }
    //            tempLineGameSlot.Clear();
    //        }
    //    }
    //    if (CheckLineList.Count > 2)
    //    {
    //        List<GameSlot> tempRowGameSlotList = new List<GameSlot>();
    //        int checkLineListCount = CheckLineList.Count;
    //        for (int i = 0; i < checkLineListCount; i++)
    //        {
    //            int Sx = CheckLineList[i].X;
    //            tempRowGameSlotList.Add(CheckLineList[i]);

    //            for (int j = 0; j < 2; j++)
    //            {
    //                for (int yDistance = 1; yDistance < 6; yDistance++)
    //                {
    //                    int y = CheckLineList[i].Y;

    //                    //0代表上
    //                    if (j == 0)
    //                    {
    //                        y += yDistance;
    //                    }
    //                    else
    //                    {
    //                        y -= yDistance;
    //                    }
    //                    if (y < 0 || y >= 6)
    //                    {
    //                        break;
    //                    }
    //                    if (CheckLineList[i].SlotRenderComponent.ColorType != slots[Sx, y].SlotRenderComponent.ColorType)
    //                    {
    //                        break;
    //                    }
    //                    tempRowGameSlotList.Add(slots[Sx, y]);

    //                }
    //            }
    //            if (tempRowGameSlotList.Count > 2)
    //            {
    //                for (int ti = 0; ti < tempRowGameSlotList.Count; ti++)
    //                {
    //                    CheckLineList.Add(tempRowGameSlotList[i]);
    //                }
    //            }
    //            tempRowGameSlotList.Clear();
    //        }
    //    }


    //    if (CheckLineList.Count > 2)
    //    {
    //        for (int i = 0; i < CheckLineList.Count; i++)
    //        {
    //            FinishedList.Add(CheckLineList[i]);
    //        }
    //    }

    //    if (CheckRowList.Count > 2)
    //    {
    //        for (int i = 0; i < CheckRowList.Count; i++)
    //        {
    //            FinishedList.Add(CheckRowList[i]);
    //        }
    //    }

    //    return FinishedList.Count == 0 ? null : FinishedList;
    //}

}
