using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private void Start()
    {
        slider.value = StaticData.musicValue;
    }
    public float GetSLiderValue()
    {
        return slider.value;
    }
    private void Update()
    {
        StaticData.musicValue = slider.value;
    }
}
