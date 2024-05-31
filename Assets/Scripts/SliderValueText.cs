using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;

    private void Update()
    {
        text.text = slider.value.ToString("0.###");
    }
}
