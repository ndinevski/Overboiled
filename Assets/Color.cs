using UnityEngine;


[CreateAssetMenu(fileName = "Bubble", menuName = "Bubbles")]

public class Color : ScriptableObject
{
    public Color32 BubbleColor;
    public bool IsPoppable;
}
