using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public RectTransform pointers;
    public GameEvent buttonPressed;
    public GameEvent pointerJump;

    public void UpdatePointerPos()
    {
        pointers.gameObject.SetActive(true);
        Vector2 newPointerPos = new Vector2(0, GetComponent<RectTransform>().anchoredPosition.y);
        if (newPointerPos != pointers.anchoredPosition)
        {
        pointers.anchoredPosition = newPointerPos;
        pointerJump.Raise();
        }
    }

    public void OnPressed()
    {
        buttonPressed.Raise();
    }
}
