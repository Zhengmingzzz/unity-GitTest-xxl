using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    public static event Action<GameSlot> ClickEvent;
    public static void CallUpWhenClick(GameSlot gameSlot)
    {
        ClickEvent.Invoke(gameSlot);
    }


}
