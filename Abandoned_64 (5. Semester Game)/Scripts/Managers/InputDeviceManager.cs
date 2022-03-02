using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum DeviceType { MK, Controller }

public class InputDeviceManager : MonoBehaviour
{
    private static InputDeviceManager instance;
    public static InputDeviceManager Instance { get => instance; }

    private InputDevice activeDevice;

    private DeviceType activeDeviceType;
    public DeviceType ActiveDeviceType { get => activeDeviceType; }

    [SerializeField] private GameEvent switchedToMouseAndKeyboard;
    [SerializeField] private GameEvent switchedToGamepad;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void RegisterDevice(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var inputAction = (InputAction)obj;
            var lastControl = inputAction.activeControl;

            if (activeDevice == null ||
            activeDevice.displayName == "Mouse" && lastControl.device.displayName != "Keyboard" && lastControl.device.displayName != "Mouse" ||
            activeDevice.displayName == "Keyboard" && lastControl.device.displayName != "Mouse" && lastControl.device.displayName != "Keyboard")
            {
                activeDevice = lastControl.device;
                DevicechangeEvents();
            }
            else if (activeDevice.displayName != lastControl.device.displayName && activeDevice.displayName != "Mouse" && activeDevice.displayName != "Keyboard")
            {
                activeDevice = lastControl.device;
                DevicechangeEvents();
            }
        }
    }

    private void DevicechangeEvents()
    {
        Debug.Log("switch");
        if (activeDevice.displayName == "Mouse" || activeDevice.displayName == "Keyboard")
        {
            activeDeviceType = DeviceType.MK;
            StartCoroutine(RaiseEvent(switchedToMouseAndKeyboard));
        }
        else
        {
            activeDeviceType = DeviceType.Controller;
            StartCoroutine(RaiseEvent(switchedToGamepad));
        }
    }

    //for some reason I get a nullpointer if I don't delay the event raise by a frame, so don't touch this, even if it seems redundant
    IEnumerator RaiseEvent(GameEvent e)
    {
        yield return new WaitForEndOfFrame();
        e.Raise();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += RegisterDevice;   
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= RegisterDevice;           
    }
}
