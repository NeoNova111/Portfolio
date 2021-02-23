using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//REALLY Don't like this approach, would probably redo this using inheritance if I had the time in this project (messy but has to do for now)
public class FadeOutAfterEnable : MonoBehaviour
{
    public bool fadeOut = true;
    public bool deactivateAfterFade = true;
    public float displayTime = 1;
    public float fadeOutTime = 1;

    Text textComponent;
    Color textStartColor;
    Color textEndColor;

    Image imageComponent;
    Color imageStartColor;
    Color imageEndColor;

    float timeSinceDisplay;
    float timeSinceFade;
    bool fading;

    private void OnEnable()
    {
        textComponent = gameObject.GetComponent<Text>();
        imageComponent = gameObject.GetComponent<Image>();

        fading = false;
        if (textComponent)
        {
            textStartColor = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);
            textEndColor = new Color(textStartColor.r, textStartColor.g, textStartColor.b, 0);
        }

        if (imageComponent)
        {
            imageStartColor = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1);
            imageEndColor = new Color(imageStartColor.r, imageStartColor.g, imageStartColor.b, 0);
        }

        if (fadeOut)
            StartFade();
        else
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (textComponent)
            textComponent.color = textStartColor;

        if(imageComponent)
            imageComponent.color = imageStartColor;
    }

    private void Update()
    {
        if (fading)
            if(textComponent)
                FadeOutText();

            if (imageComponent)
                FadeOutImage();
    }

    void StartFade()
    {
        if (textComponent)
            textComponent.color = textStartColor;

        if (imageComponent)
            imageComponent.color = imageStartColor;

        timeSinceDisplay = 0;
        timeSinceFade = 0;
        fading = true;
    }

    void FadeOutText()
    {
        if (timeSinceDisplay <= displayTime)
        {
            timeSinceDisplay += Time.deltaTime;
        }
        else if (timeSinceFade < fadeOutTime)
        {
            textComponent.color = Color.Lerp(textStartColor, textEndColor, timeSinceFade / fadeOutTime);
            timeSinceFade += Time.deltaTime;
        }
        else if (timeSinceFade >= fadeOutTime && textComponent.color.a != 0)
        {
            textComponent.color = textEndColor;
            fading = false;

            if (deactivateAfterFade)
                gameObject.SetActive(false);
        }
    }

    void FadeOutImage()
    {
        if (timeSinceDisplay <= displayTime)
        {
            timeSinceDisplay += Time.deltaTime;
        }
        else if (timeSinceFade < fadeOutTime)
        {
            imageComponent.color = Color.Lerp(imageStartColor, imageEndColor, timeSinceFade / fadeOutTime);
            timeSinceFade += Time.deltaTime;
        }
        else if (timeSinceFade >= fadeOutTime && imageComponent.color.a != 0)
        {
            imageComponent.color = imageEndColor;
            fading = false;

            if(deactivateAfterFade)
                gameObject.SetActive(false);
        }
    }
}
