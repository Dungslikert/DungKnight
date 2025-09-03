using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    public void SetMana(int current, int max)
    {
        slider.value = Mathf.Clamp01((float)current / max);
        if (text != null)
            text.text = $"Mana: {current}/{max}";
    }
}
