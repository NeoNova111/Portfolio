using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{
    private float blinking = 0;
    [SerializeField] private float Speed = 5;
    [SerializeField] private GameObject Rendering;
    public bool HasBeenHit;
    private float Timer = 0;
    [SerializeField] private float BlinkTime = 2;
    [SerializeField] SkinnedMeshRenderer skinmeshRend;
    private float dmgColorStrength=1;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (HasBeenHit)
        {
           
            dmgColorStrength = Mathf.Clamp01(dmgColorStrength)-Time.deltaTime;
            
            for (int i = 0; i < skinmeshRend.materials.Length; i++) {

                skinmeshRend.materials[i].SetFloat("Vector1_fad0febd136f4f4591520c731c6f9a46", dmgColorStrength);

            }
            blinking += Time.deltaTime * Speed;

            if (blinking <= 1f)
            {
                if (blinking <= 0.5f)
                {

                    Rendering.SetActive(false);
                }
                else
                {

                    Rendering.SetActive(true);
                }
            }
            else
            {
                blinking = 0;
               
            }
            Timer += Time.deltaTime;
            if(Timer> BlinkTime)
            {
                HasBeenHit = false;
                Timer = 0;
                Rendering.SetActive(true);
                dmgColorStrength = 1;
                
            }
        }
            
    }
  
}
