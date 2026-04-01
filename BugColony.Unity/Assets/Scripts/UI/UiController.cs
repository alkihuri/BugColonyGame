using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [Header("uGUI Texts")]
    public TextMeshProUGUI aliveCountText;
    public TextMeshProUGUI deadWorkerCountText;
    public TextMeshProUGUI deadPredatorCountText;


    private void Awake()
    {
        if (aliveCountText == null)
            Debug.LogWarning("Alive Count Text is not assigned in the inspector.");
        if (deadWorkerCountText == null)
            Debug.LogWarning("Dead Worker Count Text is not assigned in the inspector.");
        if (deadPredatorCountText == null)
            Debug.LogWarning("Dead Predator Count Text is not assigned in the inspector.");
    }
}