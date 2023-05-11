using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotRender : MonoBehaviour
{
    private Image slotImage;
    [SerializeField]
    private enums.slotColor slotColor;
    public enums.slotColor SlotColor { get => slotColor;}

    [System.Serializable]
    public struct slotColorSt
    {
        public enums.slotColor slotColor;
        public Color color;
    }
    public slotColorSt[] slotColorStArray;
    private Dictionary<enums.slotColor, Color> slotColorDic = new Dictionary<enums.slotColor, Color>();


    private void Awake()
    {
        slotImage = this.GetComponent<Image>();
        saveStructArrayToDictionary();
    }

    public void RenderSlotColor(enums.slotColor newColor)
    {
        slotImage.color = slotColorDic[newColor];
        slotColor = newColor;

    }
    private void saveStructArrayToDictionary()
    {
        for (int i = 0; i < (int)enums.slotColor.COUNT; i++)
        {
            if (!slotColorDic.ContainsKey(slotColorStArray[i].slotColor))
            {
                slotColorDic.Add(slotColorStArray[i].slotColor, slotColorStArray[i].color);
            }

        }
    }









}
