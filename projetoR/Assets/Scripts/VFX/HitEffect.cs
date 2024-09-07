using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq.Expressions;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    private int hitEffectAmout = Shader.PropertyToID("_HitEffectAmount");
    private SpriteRenderer spriteRenderer;
    private Material material;

    private float lerpAmount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    void Update()
    {
    
    }

    public void HitBlinkEffect()
    {
        lerpAmount = 0;
        DOTween.To(GetLerpValue, SetLerpValue, 1f, duration).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete);
    }

    private void OnLerpUpdate()
    {
        material.SetFloat(hitEffectAmout, GetLerpValue());
    }

    private void OnLerpComplete()
    {
        DOTween.To(GetLerpValue, SetLerpValue, 0f, duration).OnUpdate(OnLerpUpdate);
    }

    private float GetLerpValue()
    {
        return lerpAmount;
    }

    private void SetLerpValue(float newValue)
    {
        lerpAmount = newValue;
    }
}
