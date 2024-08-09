using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player Sound Effect")]
    [field: SerializeField] public EventReference Roll { get; private set; }
    [field: SerializeField] public EventReference attackSlash {  get; private set; }
    [field: SerializeField] public EventReference footSteps { get; private set; }
    [field: SerializeField] public EventReference missParry { get; private set; }
    [field: SerializeField] public EventReference parryStrike { get; private set; }
    [field: SerializeField] public EventReference climbingLadder { get; private set; }

    [field: Header("Coin Sound Effect")]
    [field: SerializeField] public EventReference coinCollected { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one FMODEvents instance in the scene");
        }

        instance = this;
    }
}
