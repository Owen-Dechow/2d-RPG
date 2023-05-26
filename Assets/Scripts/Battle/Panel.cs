using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [HideInInspector] public BattleUnit unit;


    [SerializeField] Slider hpSlider;
    [SerializeField] TMPro.TextMeshProUGUI hpText;

    [SerializeField] Slider magicSlider;
    [SerializeField] TMPro.TextMeshProUGUI psiText;

    [SerializeField] TMPro.TextMeshProUGUI panelName;
    [SerializeField] Image background;
    [SerializeField] Color deathColor;

    public void DisplayUnit()
    {
        panelName.text = unit.title;

        hpSlider.value = GetLifePercent();
        hpText.text = $"{GetClampedLifeReading()}/{unit.maxLife}";

        magicSlider.value = GetMagicPercent();
        psiText.text = $"{GetClampedMagicReading()}/{unit.maxMagic}";

        background.color = unit.life > 0 ? Color.black : deathColor;
    }

    public void DisplayUnitGradual(float step)
    {
        panelName.text = unit.title;

        hpSlider.value += (GetLifePercent() - hpSlider.value) * step;
        hpText.text = $"{GetClampedLifeReading()}/{unit.maxLife}";

        magicSlider.value += (GetMagicPercent() - magicSlider.value) * step;
        psiText.text = $"{GetClampedMagicReading()}/{unit.maxMagic}";

        Color targetColor = unit.life > 0 ? Color.black : deathColor;
        float r = background.color.r + ((targetColor.r - background.color.r) * step);
        float g = background.color.g + ((targetColor.g - background.color.g) * step);
        float b = background.color.b + ((targetColor.b - background.color.b) * step);
        float a = background.color.a + ((targetColor.a - background.color.a) * step);
        background.color = new Color(r, g, b, a);
    }

    private float GetClampedLifeReading() => Mathf.Clamp(unit.life, 0, unit.maxLife);
    private float GetLifePercent() => Mathf.Clamp(GetClampedLifeReading() / unit.maxLife, 0, 1);

    private float GetClampedMagicReading() => Mathf.Clamp(unit.magic, 0, unit.maxMagic);
    private float GetMagicPercent() => Mathf.Clamp(GetClampedMagicReading() / unit.maxMagic, 0, 1);
}
