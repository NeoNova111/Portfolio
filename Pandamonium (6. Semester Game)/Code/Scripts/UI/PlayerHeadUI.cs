using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeadUI : MonoBehaviour
{
    private Sprite currentEmotion;
    private Image spriteImage;
    private Dictionary<Sprite, GameObject> objectToVisualsDict = new Dictionary<Sprite, GameObject>();

    private void Start()
    {
        spriteImage = GetComponent<Image>();
        currentEmotion = spriteImage.sprite;
    }

    public void AddItemVisual(Sprite itemVisual)
    {
        GameObject copy = Instantiate(gameObject, transform);
        copy.GetComponent<Image>().sprite = itemVisual;
        Destroy(copy.GetComponent<PlayerHeadUI>());
        RectTransform rt = copy.GetComponent<RectTransform>();
        rt.localEulerAngles = Vector3.zero;
        rt.anchoredPosition = Vector3.zero;
        objectToVisualsDict.Add(itemVisual, copy);
    }

    public void RemoveItemVisual(Sprite itemVisual)
    {
        if (!objectToVisualsDict.ContainsKey(itemVisual)) return;

        GameObject removed = objectToVisualsDict[itemVisual];
        objectToVisualsDict.Remove(itemVisual);
        Destroy(removed);
    }
}
