using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
    private Color messageColor;
    private string message;

    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Image errorImage;

    public void Setup(Color c, string m)
    {
        messageColor = c;
        message = m;

        errorText.text = message;
        errorText.color = messageColor;
        errorImage.color = messageColor;
    }
}
