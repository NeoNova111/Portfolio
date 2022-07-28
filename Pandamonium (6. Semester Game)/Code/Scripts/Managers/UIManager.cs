using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get => instance; }

    public PlayerHeadUI PlayerHeadUI { get => playerHeadUI; }
    public TempBuffTimerHolder BuffTimerHolder { get => buffTimerHolder; }
    public BossHealthBar BossHP { get => bossHP; }
    public ScaleImpulse HitMarker { get => hitmarkerScaler; }

    [SerializeField] private GameObject tooltip;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [SerializeField] private TextMeshProUGUI interactText;

    [SerializeField] private PlayerHeadUI playerHeadUI;
    [SerializeField] private TempBuffTimerHolder buffTimerHolder;

    [SerializeField] private BossHealthBar bossHP;
    [SerializeField] private ScaleImpulse hitmarkerScaler;

    [SerializeField] private TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        UpdateInteractText();
        UpdateMoney();
    }

    public void ToolTipPopUp(float time)
    {
        StartCoroutine(PopUpTimer(tooltip, time));
    }

    public void ToolTipPopUp(float time, string description)
    {
        tooltipText.text = description;
        StartCoroutine(PopUpTimer(tooltip, time));
    }

    private IEnumerator PopUpTimer(GameObject obj, float time)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    public void UpdateInteractText()
    {
        PlayerController player = PlayerController.Instance;
        if (player.ClosestInteractable != null) interactText.text = player.ClosestInteractable.InteractText;
        else interactText.text = "";
    }

    public void UpdateMoney()
    {
        moneyText.text = "" + PlayerController.Instance.money.Money;
    }
}
