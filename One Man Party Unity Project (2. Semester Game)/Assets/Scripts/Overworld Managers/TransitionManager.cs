using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Animator transition;
    public float transitionTime = 1f;

    public void LoadLevelStart(string scene)
    {
        //StartCoroutine(Loadlevel(scene));
        if (SceneManager.GetActiveScene().name.Contains("Dungeon"))
            StartCoroutine(Loadlevel("BattleDungeon"));
        else
        {
            StartCoroutine(Loadlevel("BattleOverworld"));
        }
    }

    IEnumerator Loadlevel(string scene)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(scene);
        AudioManager.instance.SetMusic(AudioManager.instance.battleTheme);
        AudioManager.instance.SyncMusic();
    }
}
