using UnityEngine.UI;
using UnityEngine;

public class Colors : MonoBehaviour
{
    public ColorBlock validColorBlock = ColorBlock.defaultColorBlock;
    public ColorBlock invalidColorBlock = ColorBlock.defaultColorBlock;
    public ColorBlock inactiveColorBlock = ColorBlock.defaultColorBlock;

    public static Color32 lightRed = new(255, 200, 200, 255);
    public static Color32 darkRed = new(255, 150, 150, 255);
    public static Color32 lightGreen = new(200, 255, 200, 255);
    public static Color32 darkGreen = new(150, 255, 150, 255);
    public static Color32 inactiveGray = new Color32(77, 77, 77, 255);

    void Start() 
    {
        invalidColorBlock.normalColor = lightRed;
        invalidColorBlock.highlightedColor = lightRed;
        invalidColorBlock.pressedColor = darkRed;
        invalidColorBlock.selectedColor = lightRed;

        validColorBlock.normalColor = lightGreen;
        validColorBlock.highlightedColor = lightGreen;
        validColorBlock.pressedColor = darkGreen;
        validColorBlock.selectedColor = lightGreen;

        inactiveColorBlock.normalColor = inactiveGray;
        inactiveColorBlock.highlightedColor = inactiveGray;
        inactiveColorBlock.pressedColor = inactiveGray;
        inactiveColorBlock.selectedColor = inactiveGray;
    }

    void Update() 
    {

    }
}