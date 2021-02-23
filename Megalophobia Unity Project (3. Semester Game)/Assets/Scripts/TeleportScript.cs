using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType { HARDCUT, FADEOUT }
public enum DestinationChangeType { CHANGESCENE, TELEPORT }
public enum TransitionDisplayType { IMAGE, ADJUSTABLETEXT }

//things commented out might be implemented later so I'm keeping them for now, for the convinience of future me
public class TeleportScript : MonoBehaviour
{
    //all public for custom editor script
    public DestinationChangeType changeDestination;
    public string sceneName;
    public Vector3 destination;

    public bool destinationHasNewMusic = false;
    public AudioClip newMusic;

    public TransitionType transitionType = TransitionType.HARDCUT;
    public float transitionTime = 1f;
    public float fadeOutTime = 1f;

    public TransitionDisplayType transitionDisplay;
    public Sprite transitionImage;
    public string dateText;
    public string depthText;

    TransitionManager transitionmanagerInstance;

    private void Start()
    {
        transitionmanagerInstance = TransitionManager.instance;
    }

    IEnumerator ChangeToDestination(GameObject objectToTp)
    {
        while (DialogueControll.instance.HasActiveDialogue())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (transitionmanagerInstance == null)
        {
            Debug.LogWarning("No TransitionManager instance in scene, so teleport/ transitions do not work");
            yield break;
        }

        if (destinationHasNewMusic && newMusic != null)
        {
            AudioManager audioManagerInstance = AudioManager.instance;

            if (audioManagerInstance == null)
                Debug.LogWarning("No AudioManager instance to set music to");
            else
                audioManagerInstance.SetMusic(newMusic);
        }

        switch(transitionType)
        {
            case TransitionType.HARDCUT:
                HardCutTransition();
                break;
            case TransitionType.FADEOUT:
                FadeOutTransition();
                break;
        }
    }

    void HardCutTransition()
    {
        transitionmanagerInstance.DisableFadeOut();

        switch (changeDestination)
        {
            case DestinationChangeType.CHANGESCENE:
                if (transitionDisplay == TransitionDisplayType.ADJUSTABLETEXT)
                    transitionmanagerInstance.HardCut(transitionTime, dateText, depthText, sceneName);
                else
                    transitionmanagerInstance.HardCut(transitionTime, transitionImage, sceneName);
                break;
            case DestinationChangeType.TELEPORT:
                if (transitionDisplay == TransitionDisplayType.ADJUSTABLETEXT)
                    transitionmanagerInstance.HardCut(transitionTime, dateText, depthText, destination);
                else
                    transitionmanagerInstance.HardCut(transitionTime, transitionImage, destination);
                break;
        }
    }

    void FadeOutTransition()
    {
        transitionmanagerInstance.EnableFadeOut();

        switch (changeDestination)
        {
            case DestinationChangeType.CHANGESCENE:
                if (transitionDisplay == TransitionDisplayType.ADJUSTABLETEXT)
                    transitionmanagerInstance.FadeOut(transitionTime, fadeOutTime, dateText, depthText, sceneName);
                else
                    transitionmanagerInstance.FadeOut(transitionTime, fadeOutTime, transitionImage, sceneName);
                break;
            case DestinationChangeType.TELEPORT:
                if (transitionDisplay == TransitionDisplayType.ADJUSTABLETEXT)
                    transitionmanagerInstance.FadeOut(transitionTime, fadeOutTime, dateText, depthText, destination);
                else
                    transitionmanagerInstance.FadeOut(transitionTime, fadeOutTime, transitionImage, destination);
                break;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            StopAllCoroutines();
            StartCoroutine(ChangeToDestination(collision.gameObject));
        }
    }
}
