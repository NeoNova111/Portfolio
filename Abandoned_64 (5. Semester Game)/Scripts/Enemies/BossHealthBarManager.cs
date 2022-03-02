using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarManager : MonoBehaviour
{

    #region Singelton

    public static BossHealthBarManager instance;

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

    [SerializeField] private Material healthBarFillMat; //Vector1_Health
    [SerializeField] private float FirstHeartPortion = 0.43f;
    [SerializeField] private float SecondHeartPortion = 0.43f;
    private float ThirdHeartPortion;

    private Vector3 heartHealths = new Vector3(1,1,1);
    private HeartEnemy [] heartEnemies = new HeartEnemy[3];

    // Start is called before the first frame update
    void Start()
    {
        if(FirstHeartPortion+SecondHeartPortion > 1) { Debug.LogError("FirstHeartPortion + SecondHeartPortion needs to be smaller than or equal to 1"); }
        ThirdHeartPortion = 1 - FirstHeartPortion - SecondHeartPortion;
        healthBarFillMat.SetFloat("Vector1_Health", 1f);
    }

    public void setReferenceToHeart(HeartEnemy heart, int id)
    {
        heartEnemies[id] = heart;
    }

    public void updateHealthBar(int id, float health)
    {
        heartHealths[id] = health;
        healthBarFillMat.SetFloat("Vector1_Health", heartHealths[0] * FirstHeartPortion + heartHealths[1] * SecondHeartPortion + heartHealths[2] * ThirdHeartPortion);
    }
}
