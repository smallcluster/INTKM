using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PierreBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public Image fill;

    public Gradient gradient;

    public void SetMaxValue(int val)
    {
        slider.maxValue = val;
        fill.color = gradient.Evaluate(1f);
    }
    public void SetValue(int val)
    {
        slider.value = val;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetVisible(bool b)
    {
        gameObject.SetActive(b);
    }
}
