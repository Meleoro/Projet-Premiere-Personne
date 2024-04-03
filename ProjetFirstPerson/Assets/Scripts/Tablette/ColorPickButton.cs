using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InfoGamerHubAssets {
    public class ColorPickButton : MonoBehaviour
    {
    public UnityEvent<Color> ColorPickerEvent;
    public Texture2D colorChart;
    public RectTransform colorChartRect;

    public RectTransform cursor;
    [SerializeField] Image button;

    public void PickColor(BaseEventData data)
    {
        PointerEventData pointer = data as PointerEventData;

        cursor.position = pointer.position;
        Vector2 cursorRealPosition = new Vector2(colorChartRect.rect.width/2 + cursor.localPosition.x, colorChartRect.rect.height/2 + cursor.localPosition.y);
        
        Color pickedColor = colorChart.GetPixel((int)(cursorRealPosition.x * (colorChart.width / colorChartRect.rect.width)), (int)(cursorRealPosition.y * (colorChart.height / colorChartRect.rect.height)));

        button.color = pickedColor;

        ColorPickerEvent.Invoke(pickedColor);


    }
    }
}