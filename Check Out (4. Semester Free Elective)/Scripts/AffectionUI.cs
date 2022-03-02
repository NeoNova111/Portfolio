using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectionUI : MonoBehaviour
{
    public static AffectionUI instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject[] hearts;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    private void OnEnable()
    {
        SetHearts();
    }

    public void SetHearts()
    {
        DeactivateHearts();
        for(int i = 0; i < GameManager.instance.currentCharacter.tempAffection; i++)
        {
            hearts[i].SetActive(true);
        }
    }

    public void DeactivateHearts()
    {
        foreach(GameObject heart in hearts)
        {
            heart.SetActive(false);
        }
    }

    public void OnAffectionChange()
    {
        if(gameManager.currentCharacter.tempAffection > CurrentAffection())
        {
            ActivateHeart();
        }
        else if(gameManager.currentCharacter.tempAffection < CurrentAffection())
        {
            DeactivateHeart();
        }
    }

    void ActivateHeart()
    {
        hearts[CurrentAffection()].SetActive(true);
    }

    void DeactivateHeart()
    {
        hearts[CurrentAffection() - 1].SetActive(false);
    }

    int CurrentAffection()
    {
        int ca = 0;
        foreach (GameObject heart in hearts)
        {
            if (heart.activeSelf) ca++;
        }
        return ca;
    }
}
