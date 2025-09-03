using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    public void SetHealth(int current, int max)
    {
        slider.value = Mathf.Clamp01((float)current / max);
        if (text != null)
            text.text = $"Health: {current}/{max}";
    }
}
