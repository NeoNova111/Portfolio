using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchButtonAnimator : MonoBehaviour
{
    [SerializeField] private Material glitchButtonMaterial;
    [SerializeField] private bool normalButtonAtStart = true;
    [SerializeField] private float hintGlitchStrenght = 0.1f;
    [SerializeField] private float hintGlitchSpeed = 0.1f;
    [SerializeField] private float glitchAnimationSpeed = 0.1f;

    private bool isAnimatingHintGlitch = false;
    private bool isAnimatingHintGlitchBack = false;
    private bool isAnimatingFullGlitch = false;

    private float progress = 1;
    private float progressBeforeAnimStart;

    // Start is called before the first frame update
    void Start()
    {
        if (normalButtonAtStart)
        {
            glitchButtonMaterial.SetFloat("Vector1_GlitchProgress", 0);
            progress = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimatingFullGlitch)
        {
            progress += glitchAnimationSpeed * Time.deltaTime;
            if(progress >= 1) {
                progress = 1;
                isAnimatingFullGlitch = false;
            }
            glitchButtonMaterial.SetFloat("Vector1_GlitchProgress", progress);
        }
        else if(isAnimatingHintGlitch)
        {
            progress += hintGlitchSpeed * Time.deltaTime;
            if(progress >= hintGlitchStrenght)
            {
                progress = hintGlitchStrenght;
                isAnimatingHintGlitch = false;
                isAnimatingHintGlitchBack = true;
            }
            glitchButtonMaterial.SetFloat("Vector1_GlitchProgress", progress);
        }
        else if (isAnimatingHintGlitchBack)
        {
            progress -= hintGlitchSpeed * Time.deltaTime;
            if (progress <= 0)
            {
                progress = 0;
                isAnimatingHintGlitchBack = false;
            }
            glitchButtonMaterial.SetFloat("Vector1_GlitchProgress", progress);
        }
    }

    public void HintGlitch()
    {
        if (!isAnimatingFullGlitch)
        {
            isAnimatingHintGlitch = true;
            progressBeforeAnimStart = progress;
        }
    }

    public void FullGlitch()
    {
        isAnimatingHintGlitch = false;
        isAnimatingFullGlitch = true;
    }

}
