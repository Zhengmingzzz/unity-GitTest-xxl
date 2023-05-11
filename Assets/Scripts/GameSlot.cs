using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSlot : MonoBehaviour,IPointerClickHandler
{
    private int x;
    private int y;
    private enums.soltType slotType;
    private SlotMove moveComponent;
    private SlotRender renderComponent;


    public int X { get => x; }
    public int Y { get => y; }
    public enums.soltType SlotType { get => slotType;}
    public SlotMove MoveComponent { get => moveComponent;}
    public SlotRender RenderComponent { get => renderComponent; }

    public void Init(int x, int y, enums.soltType slotType)
    {
        this.x = x;
        this.y = y;
        this.slotType = slotType;
    }

    private void Awake()
    {
        moveComponent = this.GetComponent<SlotMove>();
        renderComponent = this.GetComponent<SlotRender>();
    }
    public bool CanMove()
    {
        return moveComponent != null;
    }
    public bool CanRender()
    {
        return RenderComponent != null;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventHandler.CallUpWhenClick(this);
    }
}
