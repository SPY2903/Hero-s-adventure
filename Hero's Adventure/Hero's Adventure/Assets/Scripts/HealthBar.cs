using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    public void SetMaxHeath(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color =  gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
