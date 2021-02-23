using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCrosshairBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BattleSystem battleSystem;

    private void Start()
    {
        battleSystem = BattleSystem.instance;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (battleSystem.targetedEnemyUnit != null)
        {
            transform.position = battleSystem.targetedEnemyUnit.gameObject.transform.position;
            if (spriteRenderer.enabled == false)
                spriteRenderer.enabled = true;
        }
        else
        {
            if (spriteRenderer.enabled == true)
                spriteRenderer.enabled = false;
        }
    }
}
