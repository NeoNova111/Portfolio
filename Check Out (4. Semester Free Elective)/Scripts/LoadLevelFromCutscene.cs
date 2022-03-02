using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelFromCutscene : MonoBehaviour
{
    //this scrript was created under time pressure

    void Start()
    {
        StartCoroutine(LoadLevelSelectAfterSeconds(45f));
    }

    IEnumerator LoadLevelSelectAfterSeconds(float s)
    {
        yield return new WaitForSeconds(s);
        LoadLevelSelect();
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("1_LevelSelect");
    }
}
