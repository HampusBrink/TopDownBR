using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ItemVisuals : MonoBehaviour
{
    public Color highlightColor;
    public float intensity = 1f;
    public float highlightFadeTime = 0.5f;
    public Sprite borderSprite;
    private VisualEffect _borderVFX;
    [System.NonSerialized] public Color defaultColor;

    private void Awake()
    {
        _borderVFX = GetComponent<VisualEffect>();
        defaultColor = _borderVFX.GetVector4("Color");
    }

    private Coroutine currentFadeHighlight;
    
    IEnumerator FadeHighlight(Vector4 desiredColor)
    {
        Vector4 currentColor = _borderVFX.GetVector4("Color");
        float elapsedTime = 0f;

        while (elapsedTime < highlightFadeTime)
        {
            elapsedTime += Time.deltaTime;
            float elapsed01 = elapsedTime / highlightFadeTime;
            _borderVFX.SetVector4("Color", Vector4.Lerp(currentColor, desiredColor, elapsed01));
            yield return null;
        }
        _borderVFX.SetVector4("Color", desiredColor);
        currentFadeHighlight = null;
    }
    
    public void Highlight()
    {
        if (currentFadeHighlight != null)
            StopCoroutine(currentFadeHighlight);
        
        currentFadeHighlight = StartCoroutine(FadeHighlight(highlightColor * intensity));
    }
    
    public void StopHighlight()
    {
        if (currentFadeHighlight != null)
            StopCoroutine(currentFadeHighlight);
        
        currentFadeHighlight = StartCoroutine(FadeHighlight(defaultColor));
    }
}
