using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;

    [SerializeField] private float shakeForceAmount = 1f;
    [SerializeField] private CinemachineImpulseListener impulseListener;

    private CinemachineImpulseDefinition impulseDefinition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(shakeForceAmount);
    }

    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        SetUpScreenShakeSettings(profile, impulseSource);

        impulseSource.GenerateImpulseWithForce(profile.impulseForce);
    }

    private void SetUpScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        impulseDefinition = impulseSource.ImpulseDefinition;

        impulseDefinition.ImpulseDuration = profile.impulseTime;
        impulseSource.DefaultVelocity = profile.defaultVelocity;
        impulseDefinition.CustomImpulseShape = profile.impulseCurve;

        impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
        impulseListener.ReactionSettings.Duration = profile.listenerDuration;
        impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
    }
}

