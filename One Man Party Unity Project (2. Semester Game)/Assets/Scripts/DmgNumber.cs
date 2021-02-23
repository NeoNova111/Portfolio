using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DmgColor {HEALTH, ARMOR, CRIT, HEAL };

public class DmgNumber : MonoBehaviour
{
    public TextMeshPro dmgText;
    public RangedFloat travelDistance;
    public float travelDelta = 0.02f;
    public float dissapearSpeed = 3f;
    public Color healthColor;
    public Color armorColor;
    public Color critColor;
    public Color healColor;

    private Vector3 target;
    private Color textColor;

    private void Start()
    {
        textColor = dmgText.color;
        target = transform.position + RandomDirection() * Random.Range(travelDistance.minValue, travelDistance.maxValue);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, travelDelta * Time.deltaTime);

        if(Vector3.Distance(target, transform.position) <= 1)
        {
            textColor.a -= dissapearSpeed * Time.deltaTime;
            dmgText.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDmgText(int value, DmgColor c)
    {
        switch (c)
        {
            case DmgColor.ARMOR:
                textColor = armorColor;
                break;
            case DmgColor.HEALTH:
                textColor = healthColor;
                break;
            case DmgColor.CRIT:
                textColor = critColor;
                break;
            case DmgColor.HEAL:
                textColor = healColor;
                break;
            default:
                Debug.LogWarning("invalid dmg color");
                break;
        }
        dmgText.color = textColor;

        dmgText.text = "" + value;
    }

    Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(0.3f, 1f)).normalized;
    }
}
