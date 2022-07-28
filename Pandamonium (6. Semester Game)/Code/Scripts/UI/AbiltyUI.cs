using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbiltyUI : MonoBehaviour
{
    [System.Serializable] public struct AbilityIcon
    {
        public Image iconImage;
        public Image cooldown;
        public GameObject countBG;
        public TextMeshProUGUI countText;
        public GameObject disabledAbility;
        public Animator abilityIconAnimator;
    }

    [System.Serializable] public struct AbilityContainer
    {
        public Ability ability;
        public AbilityIcon abilityIcon;
    }

    public AbilityContainer[] abilities;

    private void Start()
    {
        //setup
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i].ability.maxUseCount > 1)
            {
                abilities[i].abilityIcon.countBG.SetActive(true);
            }
            else
            {
                abilities[i].abilityIcon.countBG.SetActive(false);
            }

            abilities[i].abilityIcon.iconImage.sprite = abilities[i].ability.icon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < abilities.Length; i++)
        {
            abilities[i].abilityIcon.cooldown.fillAmount = abilities[i].ability.CurrentCooldown / abilities[i].ability.cooldown;
            abilities[i].abilityIcon.countText.text = ""+abilities[i].ability.CurrentUses;
            if (!abilities[i].abilityIcon.disabledAbility.activeSelf && abilities[i].ability.CurrentUses == 0)
            {
                abilities[i].abilityIcon.disabledAbility.SetActive(true);
            }
            else if(abilities[i].abilityIcon.disabledAbility.activeSelf && abilities[i].ability.CurrentUses > 0)
            {
                abilities[i].abilityIcon.disabledAbility.SetActive(false);
            }

            if (abilities[i].ability.usedThisFrame) abilities[i].abilityIcon.abilityIconAnimator.SetTrigger("AbilityUse");

            abilities[i].ability.usedThisFrame = false;
        }
    }
}
