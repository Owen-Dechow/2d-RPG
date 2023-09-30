using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public BattleUnit Unit { get; set; }
    public bool Bump { get; set; }

    [SerializeField] Slider hpSlider;
    [SerializeField] TMPro.TextMeshProUGUI hpText;

    [SerializeField] Slider magicSlider;
    [SerializeField] TMPro.TextMeshProUGUI psiText;

    [SerializeField] TMPro.TextMeshProUGUI panelName;
    [SerializeField] Image background;
    [SerializeField] Color deathColor;

    Vector2 startPosition;

    public void InstantiateToUnit()
    {
        panelName.text = Unit.data.title;

        hpSlider.value = GetLifePercent();
        hpText.text = $"{GetClampedLifeReading()}/{Unit.data.maxLife}";

        magicSlider.value = GetMagicPercent();
        psiText.text = $"{GetClampedMagicReading()}/{Unit.data.maxMagic}";

        background.color = Unit.data.life > 0 ? Color.black : deathColor;

        startPosition = transform.position;
        Bump = false;
    }

    public void DisplayUnitGradual(float step)
    {
        hpSlider.value += (GetLifePercent() - hpSlider.value) * step;
        hpText.text = $"{GetClampedLifeReading()}/{Unit.data.maxLife}";

        magicSlider.value += (GetMagicPercent() - magicSlider.value) * step;
        psiText.text = $"{GetClampedMagicReading()}/{Unit.data.maxMagic}";

        Color targetColor = Unit.data.life > 0 ? Color.black : deathColor;
        float r = background.color.r + ((targetColor.r - background.color.r) * step);
        float g = background.color.g + ((targetColor.g - background.color.g) * step);
        float b = background.color.b + ((targetColor.b - background.color.b) * step);
        float a = background.color.a + ((targetColor.a - background.color.a) * step);
        background.color = new Color(r, g, b, a);

        if (Bump)
        {
            float bumpHeight = 20f;
            transform.position = startPosition + Vector2.up * bumpHeight;
        }
        else
        {
            transform.position = startPosition;
        }
    }

    private float GetClampedLifeReading() => Mathf.Clamp(Unit.data.life, 0, Unit.data.maxLife);
    private float GetLifePercent() => Mathf.Clamp(GetClampedLifeReading() / Unit.data.maxLife, 0, 1);

    private float GetClampedMagicReading() => Mathf.Clamp(Unit.data.magic, 0, Unit.data.maxMagic);
    private float GetMagicPercent() => Mathf.Clamp(GetClampedMagicReading() / Unit.data.maxMagic, 0, 1);
}
