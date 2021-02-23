using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text classText;
    public Text levelText;
    public Text armorValue;
    public Slider hpSlider;
    public Text hpSliderVAlueText;
    public Image hpFill;
    public Slider speedGauge;
    public GameObject statusBase;
    public Text statusText;

    public void SetHUD(Unit unit, float speed)
    {
        nameText.text = unit.race.name;
        classText.text = unit.job.name;
        levelText.text = "Lvl " + unit.level;
        hpSlider.maxValue = unit.maxHealth;
        hpSlider.value = unit.health;
        hpSliderVAlueText.text = ""+(int)hpSlider.value;
        armorValue.text = "" + unit.armor;
        speedGauge.value = speed;
    }

    //change to damage later
    public void SetHP(int hp)
    {
        StartCoroutine(CountDownHP(hp));
    }

    IEnumerator CountDownHP(int toValue)
    {
        int hp = (int)hpSlider.value;
        while (hp != toValue)
        {
            if (toValue > hp)
            {
                hp++;
                hpSliderVAlueText.text = "" + hp;
            }
            else
            {
                hp--;
                hpSliderVAlueText.text = "" + hp;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SetSpeedGauge(float gaugeValue)
    {
        speedGauge.value = gaugeValue;
    }

    public void SetArmor(int armor)
    {
        armorValue.text = "" + armor;
    }

    public void SetStatusEffect()
    {
        statusBase.SetActive(false);
    }

    public void SetStatusEffect(StatusEffect effect)
    {
        statusBase.SetActive(true);
        statusBase.GetComponent<Image>().color = effect.color;
        statusText.text = effect.name;
    }

    public void FlashHP()
    {
        StartCoroutine(FlashHPCoroutine());
    }

    IEnumerator FlashHPCoroutine()
    {
        Color color = hpFill.color;
        hpFill.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        hpFill.color = new Color(200, 0, 0);
        yield return new WaitForSeconds(0.2f);
        hpFill.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        hpFill.color = new Color(200, 0, 0);
        yield return new WaitForSeconds(0.3f);
        hpFill.color = color;
    }

    //IEnumerator LerpHP()
    //{

    //}
}
