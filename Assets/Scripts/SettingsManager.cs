using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float critPointLifeTime = 5.0f;
    [SerializeField] private float maxHitDistance = 0.4f;

    [Header("UI")]
    [SerializeField] private Slider critPointLifetimeSlider;
    [SerializeField] private Slider maxHitDistanceSlider;

    [SerializeField] private TextMeshProUGUI lifeTimeVal;
    [SerializeField] private TextMeshProUGUI maxDistVal;


    private List<MarchingCubes> listeners = new List<MarchingCubes>();

    private void Start()
    {
        if (critPointLifetimeSlider != null)
        {
            critPointLifetimeSlider.value = critPointLifeTime;
            lifeTimeVal.text = "critpoint lifetime: " + critPointLifeTime; 
            critPointLifetimeSlider.onValueChanged.AddListener(OnCritLifetimeChanged);
        }

        if (maxHitDistanceSlider != null)
        {
            maxHitDistanceSlider.value = maxHitDistance;
            maxDistVal.text = "critpoint hit distance: " + maxHitDistance;
            maxHitDistanceSlider.onValueChanged.AddListener(OnMaxHitDistChanged);
        }

        NotifyAll();
    }

    private void OnCritLifetimeChanged(float value)
    {
        critPointLifeTime = value;
        lifeTimeVal.text = "critpoint lifetime: " + value + "s";
        NotifyAll();
    }

    private void OnMaxHitDistChanged(float value)
    {
        maxHitDistance = value;
        maxDistVal.text = "critpoint hit distance: " + value;
        NotifyAll();
    }

    public void RegisterListener(MarchingCubes mc)
    {
        if (!listeners.Contains(mc))
        {
            listeners.Add(mc);
            mc.OnSettingsUpdated(critPointLifeTime, maxHitDistance);
        }
    }

    private void NotifyAll()
    {
        foreach (var mc in listeners)
        {
            if (mc != null)
                mc.OnSettingsUpdated(critPointLifeTime, maxHitDistance);
        }
    }
}
