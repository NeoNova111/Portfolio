using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    #region Singelton

    public static TransitionManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one inventory instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    #region vars

    //public Animator transition;
    public GameObject transitionImageObject;
    public Image transitionImage;

    public GameObject transitionAdjustableObject;
    public Text dateText;
    public Text depthText;

    public GameEvent Teleport;
    public GameEvent TeleportSound;

    FadeOutAfterEnable[] adjustableFadeOutComponents;

    #endregion

    private void Start()
    {
        adjustableFadeOutComponents = transitionAdjustableObject.GetComponentsInChildren<FadeOutAfterEnable>();
    }

    private void Update()
    {

    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        TeleportSound.Raise();
        Teleport.Raise();
    }

    #region Teleport

    public void LoadNewSceneStart(string scene, float transitionTime, float fadeOutTime,  GameObject obj)
    {
        StartCoroutine(LoadNewScene(scene, transitionTime, fadeOutTime, obj));
    }

    IEnumerator LoadNewScene(string scene, float transitionTime, float fadeOutTime, GameObject obj)
    {
        SceneManager.LoadScene(scene);
        TeleportSound.Raise();

        yield return new WaitForSeconds(transitionTime + fadeOutTime);

        obj.SetActive(false);
        Teleport.Raise();
    }

    public void TpToDestinationStart(Vector3 coords, float transitionTime, float fadeOutTime, GameObject obj)
    {
        StartCoroutine(TpToDestinaion(coords, transitionTime, fadeOutTime, obj));
    }


    IEnumerator TpToDestinaion(Vector3 coords, float transitionTime, float fadeOutTime, GameObject obj)
    {
        submarineMovement.instance.transform.position = coords; //maybe pass transform down from collision in the Teleport class
        TeleportSound.Raise();

        yield return new WaitForSeconds(transitionTime + fadeOutTime);

        obj.SetActive(false);
        Teleport.Raise();
    }

    #endregion

    #region HardCut

    public void HardCut(float transitionTime, Sprite transitionImage, string sceneName)
    {
        this.transitionImage.sprite = transitionImage;
        transitionImageObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, 0f, transitionImageObject);
    }

    public void HardCut(float transitionTime, Sprite transitionImage, Vector3 coords)
    {
        this.transitionImage.sprite = transitionImage;
        transitionImageObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, 0f, transitionImageObject);
    }

    public void HardCut(float transitionTime, string dateText, string depthText, string sceneName)
    {
        this.dateText.text = dateText;
        this.depthText.text = depthText;
        transitionAdjustableObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, 0f, transitionAdjustableObject);
    }

    public void HardCut(float transitionTime, string dateText, string depthText, Vector3 coords)
    {
        this.dateText.text = dateText;
        this.depthText.text = depthText;
        transitionAdjustableObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, 0f, transitionAdjustableObject);
    }

    public void HardCut(float transitionTime, Vector3 coords)
    {
        transitionImageObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, 0f, transitionImageObject);
    }

    public void HardCut(float transitionTime, string sceneName)
    {
        transitionImageObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, 0f, transitionAdjustableObject);
    }

    #endregion

    #region FadeOut

    public void FadeOut(float transitionTime, float fadeOutTime, Sprite transitionImage, string sceneName)
    {
        this.transitionImage.sprite = transitionImage;
        transitionImageObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, fadeOutTime, transitionImageObject);
    }

    public void FadeOut(float transitionTime, float fadeOutTime, Sprite transitionImage, Vector3 coords)
    {
        this.transitionImage.sprite = transitionImage;
        transitionImageObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, fadeOutTime, transitionImageObject);
    }

    public void FadeOut(float transitionTime, float fadeOutTime, string dateText, string depthText, string sceneName)
    {
        this.dateText.text = dateText;
        this.depthText.text = depthText;

        foreach (FadeOutAfterEnable comp in adjustableFadeOutComponents)
        {
            comp.displayTime = transitionTime;
            comp.fadeOutTime = fadeOutTime;
        }

        transitionAdjustableObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, fadeOutTime, transitionAdjustableObject);
    }

    public void FadeOut(float transitionTime, float fadeOutTime, string dateText, string depthText, Vector3 coords)
    {
        this.dateText.text = dateText;
        this.depthText.text = depthText;

        foreach (FadeOutAfterEnable comp in adjustableFadeOutComponents)
        {
            comp.displayTime = transitionTime;
            comp.fadeOutTime = fadeOutTime;
        }

        transitionAdjustableObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, fadeOutTime, transitionAdjustableObject);
    }

    public void FadeOut(float transitionTime, float fadeOutTime, Vector3 coords)
    {
        transitionImageObject.SetActive(true);
        TpToDestinationStart(coords, transitionTime, fadeOutTime, transitionImageObject);
    }

    public void FadeOut(float transitionTime, float fadeOutTime, string sceneName)
    {
        transitionImageObject.SetActive(true);
        LoadNewSceneStart(sceneName, transitionTime, fadeOutTime, transitionAdjustableObject);
    }

    #endregion

    public void EnableFadeOut()
    {
        foreach(FadeOutAfterEnable comp in adjustableFadeOutComponents)
        {
            comp.enabled = true;
        }
    }

    public void DisableFadeOut()
    {
        foreach (FadeOutAfterEnable comp in adjustableFadeOutComponents)
        {
            comp.enabled = false;
        }
    }
}
