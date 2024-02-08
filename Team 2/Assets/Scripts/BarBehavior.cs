using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarBehavior : MonoBehaviour
{
    public Slider slider;
 
    public void SetMaxBar(int bar)
    {
        slider.maxValue = bar;
        slider.value = bar;
    }

    public void SetBar(int bar)
    {
        slider.value = bar;
    }

}
