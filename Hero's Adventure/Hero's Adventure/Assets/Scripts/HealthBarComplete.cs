using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarComplete : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] Image fill;
    public void SetMaxHeath(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
