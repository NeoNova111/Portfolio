using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType { Message, Waring, Error }

[System.Serializable]
public struct ErrorMessageInformation
{
    public MessageType type;
    [TextArea(2,5)]public string message;
}

public class ErrorMessageHandeler : MonoBehaviour
{
    [SerializeField] private GameObject errorPrefab;
    [SerializeField] private Color messageColor;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color  errorColor;

    [SerializeField] private ErrorMessageInformation[] errorMessages;

    [SerializeField] private float bugstartTimer = 10f;
    [SerializeField] private float errorStartDelay = 20f;
    [SerializeField] private float errorStartBuffer = 10f;
    private Queue<GameObject> instantiatedMessages;

    private float currentErrorBuffer;
    private int messageIndex = 0;
    private int messageCount = 0;

    private IEnumerator clutterRoutine;

    private void Awake()
    {
        instantiatedMessages = new Queue<GameObject>();
    }

    public void StartClutter()
    {
        messageCount = 0;
        messageIndex = 0;
        currentErrorBuffer = errorStartBuffer;

        clutterRoutine = Clutter();
        StartCoroutine(clutterRoutine);
    }

    public void StopClutter()
    {
        //good faith
        StopCoroutine(clutterRoutine);
        instantiatedMessages.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void InstantiateNextError()
    {
        GameObject obj = Instantiate(errorPrefab, transform);
        instantiatedMessages.Enqueue(obj);

        ErrorMessage msg = obj.GetComponent<ErrorMessage>();
        string errorMessageText = $"[{messageCount}]: {errorMessages[messageIndex].message}";

        switch (errorMessages[messageIndex].type)
        {
            case MessageType.Error:
                msg.Setup(errorColor, errorMessageText);
                break;
            case MessageType.Message:
                msg.Setup(messageColor, errorMessageText);
                break;
            case MessageType.Waring:
                msg.Setup(warningColor, errorMessageText);
                break;
            default:
                break;
        }
        
        if(messageIndex < errorMessages.Length - 1)
        {
            messageIndex++;
        }

        if(instantiatedMessages.Count > 6)
        {
            Destroy(instantiatedMessages.Dequeue());
        }

        messageCount++;
    }

    IEnumerator Clutter()
    {
        yield return new WaitForSeconds(bugstartTimer);
        InstantiateNextError();

        yield return new WaitForSeconds(errorStartDelay);
        InstantiateNextError();

        while (true)
        {
            yield return new WaitForSeconds(currentErrorBuffer);
            InstantiateNextError();
            currentErrorBuffer = Mathf.Clamp(currentErrorBuffer / 2, 0.1f, errorStartBuffer);
        }
    }

    private void OnEnable()
    {
        StartClutter();
    }

    private void OnDisable()
    {
        StopClutter();
    }
}
