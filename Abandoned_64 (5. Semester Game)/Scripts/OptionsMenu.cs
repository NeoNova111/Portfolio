using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
struct INPUTSLIDER
{
    public InputField input;
    public Slider slider;
    public float maxValue;
    [HideInInspector] public float value;
}

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private Selectable first;
    [SerializeField] private DeviceType correspondingDeviceType;
    [SerializeField] private INPUTSLIDER horizontal;
    [SerializeField] private INPUTSLIDER vertical;
    private SaveManager saveManager;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        horizontal.slider.maxValue = horizontal.maxValue;
        vertical.slider.maxValue = vertical.maxValue;
    }

    public void HorizontalSliderChanged(float value) //returns look weird but are needed, because the dynamic sliders call this on startup even if the object is disabled
    {
        if (!gameObject.activeInHierarchy)
            return;

        horizontal.input.text = value.ToString();
        horizontal.value = value;
    }

    public void VerticalSliderChanged(float value)
    {
        if (!gameObject.activeInHierarchy)
            return;

        vertical.input.text = value.ToString();
        vertical.value = value;
    }

    public void HorizontalInputChanged(string text)
    {
        if (!gameObject.activeInHierarchy)
            return;

        horizontal.value = float.Parse(text);
        horizontal.slider.value = horizontal.value;
    }

    public void VerticallInputChanged(string text)
    {
        if (!gameObject.activeInHierarchy)
            return;

        vertical.value = float.Parse(text);
        vertical.slider.value = vertical.value;
    }

    public void InputEnded()
    {
        horizontal.value = Mathf.Clamp(horizontal.value, 0, horizontal.maxValue);
        horizontal.input.text = horizontal.value.ToString();

        vertical.value = Mathf.Clamp(vertical.value, 0, vertical.maxValue);
        vertical.input.text = vertical.value.ToString();
    }

    public void ApplyChanges()
    {
        switch (correspondingDeviceType)
        {
            case DeviceType.Controller:
                settings.controllerSensitivity = new Vector2(horizontal.value, vertical.value);
                break;
            case DeviceType.MK:
                settings.keyboardSensitivity = new Vector2(horizontal.value, vertical.value);
                break;
        }

        settings.settingsChanged.Raise();
        saveManager.SaveSettingsData();
    }

    private void OnEnable()
    {
        first.Select();

        switch (correspondingDeviceType)
        {
            case DeviceType.Controller:
                horizontal.slider.value = settings.controllerSensitivity.x;
                horizontal.input.text = settings.controllerSensitivity.x.ToString();
                vertical.slider.value = settings.controllerSensitivity.y;
                vertical.input.text = settings.controllerSensitivity.y.ToString();
                break;
            case DeviceType.MK:
                horizontal.slider.value = settings.keyboardSensitivity.x;
                horizontal.input.text = settings.keyboardSensitivity.x.ToString();
                vertical.slider.value = settings.keyboardSensitivity.y;
                vertical.input.text = settings.keyboardSensitivity.y.ToString();
                break;
        }
    }
}

