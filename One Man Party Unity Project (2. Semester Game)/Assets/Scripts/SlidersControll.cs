using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersControll : MonoBehaviour
{
    public GameObject playerSlider;
    public GameObject[] enemySliders;

    private void Start()
    {
        //CreateEnemySliders(BattleSystem.instance.enemies);
    }

    public void CreateEnemySliders(Unit[] enemies)
    {
        int length = enemies.Length;
        enemySliders = new GameObject[length];

        for(int i = 0; i < length; i++)
        {
            enemySliders[i] = Instantiate(playerSlider, gameObject.transform);
            enemySliders[i].GetComponent<SliderInfo>().SetImage(enemies[i].race.turnIcon);
        }
    }

    public void UpdateTurnBar(Unit player, Unit[] enemies)
    {
        playerSlider.GetComponent<Slider>().value = player.GetTurnPos();
        for(int i = 0; i < enemies.Length; i++)
        {
            enemySliders[i].GetComponent<Slider>().value = enemies[i].GetTurnPos();
        }
    }

    public void StartSpeed(Unit player, Unit[] enemies)
    {
        playerSlider.GetComponent<Slider>().value = player.job.speed + Random.Range(-5, 5);
        player.SetTurnPos(playerSlider.GetComponent<Slider>().value);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemySliders[i].GetComponent<Slider>().value = enemies[i].job.speed + Random.Range(-5, 5);
            enemies[i].SetTurnPos(enemySliders[i].GetComponent<Slider>().value);
        }
    }

    public void DeactivateSlider(int i)
    {
        enemySliders[i].SetActive(false);
    } 
}
