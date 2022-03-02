using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Singeton

    private static UIManager instance;

    public static UIManager Instance { get => instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    [Header("Menu")]
    [SerializeField] private PauseMenuManager pauseMenuObject;
    public bool IsPauseMenuActive { get => pauseMenuObject.MenuIsActive; }

    [Header("Inspect Dev Comment")]
    [SerializeField] private GameObject developerCommentPanel;
    [SerializeField] private Text developerComment;
    //[SerializeField] private TextMeshProUGUI developerComment;

    [Header("Coordinate Display")]
    [SerializeField] private Text coords;

    [Header("Player Stats")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Text playerHealthText;

    [Header("Inventory")]
    [SerializeField] private Text collectibleCountText;
    [SerializeField] private TextMeshProUGUI collectibleUI;
    [SerializeField] private Text keyItemCountText;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI currentContextPromptText;
    [SerializeField] private TextMeshProUGUI prevContextPromptText;
    [SerializeField] private Animator contextAnimator;
    public Animator ContextAnimator { get => contextAnimator; }
    public string DispayedContext { get => currentContextPromptText.text;}
    private bool contextUIAnimating = false;
    //public bool ContextUIAnimating { get => contextUIAnimating; set => contextUIAnimating = value; }
    public bool ContextUIAnimating { get => contextAnimator.GetCurrentAnimatorStateInfo(0).IsName("Twirl"); }
    [SerializeField] private GameObject targeIndicator;
    [SerializeField] private TargetBlackBars bars;

    [Header("Glorified Signs")]
    [SerializeField] private GameObject signPanel;
    //[SerializeField] private Text signText;
    [SerializeField] private TextMeshProUGUI signText;
    [SerializeField] private TextMeshProUGUI signDebugText;
    //[SerializeField] private Text signDebugText;
    [SerializeField] private GameObject dialoguePanel;
    //[SerializeField] private Text dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject hud;
    public GameObject HUD { get => hud; }

    public string DeveloperCommentText { get => developerComment.text; }
    public bool DeveloperCommentPanelIsActive { get => developerCommentPanel.activeSelf; }

    private CameraController playerCameraController;
    private DebugModeManager debugModeManager;

    private void Start()
    {
        playerCameraController = CameraController.Instance;
        debugModeManager = DebugModeManager.Instance;
        UpdateCollectibleCount();
    }

    private void Update()
    {
        if (debugModeManager && debugModeManager.DebugModeActive)
        {
            UpdateCoordText();
        }

        //if(contextUIAnimating && !contextAnimator.GetCurrentAnimatorStateInfo(0).IsName("Twirl"))
        //{
        //    contextUIAnimating = false;
        //}
    }

    public void UpdateContextPrompt(string currentContextText, string prevContextText)
    {
        currentContextPromptText.text = currentContextText;
        prevContextPromptText.text = prevContextText;
    }

    //public void UpdateContextPrompt(ContextPrompt currentPrompt, ContextPrompt previousPrompt)
    //{
    //    if(previousPrompt != null)
    //        UpdateContextPrompt(currentPrompt.type, previousPrompt.type);
    //    else
    //        UpdateContextPrompt(currentPrompt.type, "");
    //}

    //public void UpdateContextPrompt(string contextText, ContextPrompt previousPrompt)
    //{
    //    if (previousPrompt != null)
    //        UpdateContextPrompt(contextText, previousPrompt.type);
    //    else
    //        UpdateContextPrompt(contextText, "");
    //}

    //public void UpdateContextPrompt(ContextPrompt contextPrompt, string previousContext)
    //{

    //}

    public void UpdateCollectibleCount()
    {
        collectibleCountText.text = "Collectible count: " + playerStats.collectibleCount;
        collectibleUI.text = $"{playerStats.collectibleCount}";
    }

    public void UpdatekeyItemCount()
    {
        keyItemCountText.text = "Key item count: " + playerStats.keyCount;
    }

    public void UpdatePlayerHealthText()
    {
        playerHealthText.text = "Health: " + (((float)playerStats.CurrentHealth / (float)playerStats.maxHealth) * 100).ToString("F3") + "%";
    }

    public void UpdateDeveloperComment(string newDevComment)
    {
        developerCommentPanel.SetActive(true);
        developerComment.text = newDevComment;
    }

    public void HideDeveloperComment()
    {
        developerCommentPanel.SetActive(false);
        developerComment.text = "";
    }
    
    public void UpdateCoordText()
    {
        Vector3 pos = playerCameraController.transform.position;
        if (playerCameraController.LockedOn)
        {
            coords.text = "XYZ: " + pos.x.ToString("F3") + " / <color=red>" + pos.y.ToString("F3") + " locked</color> / " + pos.z.ToString("F3");
        }
        else
        {
            coords.text = "XYZ: " + pos.x.ToString("F3") + " / " + pos.y.ToString("F3") + " / " + pos.z.ToString("F3");
        }
    }

    public void DispalySign(string text, string debugText, bool display)
    {
        signText.text = text;
        signDebugText.text = debugText;
        signPanel.SetActive(display);
    }

    public void DispalySign( bool display)
    {
        signPanel.SetActive(display);
    }

    //public void SetHUDActive(bool active)
    //{
    //    if (hud.activeSelf == active)
    //        return;

    //    hud.SetActive(active);
    //}

    public void ToggleTargetingUI(bool active)
    {
        if (active == targeIndicator.activeSelf)
            return;

        targeIndicator.SetActive(!targeIndicator.activeSelf);
        if(active)
        {
            bars.gameObject.SetActive(true);
        }
        else
        {
            bars.Growing = active;
        }
    }
}
