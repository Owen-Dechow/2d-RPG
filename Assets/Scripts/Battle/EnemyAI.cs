using UnityEngine;

[System.Serializable]
public class EnemyAI
{
    public int attack = 20;
    public int defend = 20;
    public int item = 20;
    public int magic = 20;
    public int run = 20;
    public DataStatus dataStatus;
    [HideInInspector] public DataStatus lastCheck;

    public enum DataStatus
    {
        [Tooltip("Data values do not add up to 100. To auto correct using current values switch to Data Status Valid. Otherwise data will correct on game play (May cause unexpected behaviors).")]
        Invalid,
        Valid
    }

    public int GetTotal()
    {
        int total = 0;
        total += attack;
        total += defend;
        total += item;
        total += magic;
        total += run;
        return total;
    }

    public void CorrectData()
    {
        float total = GetTotal();
        attack = Mathf.RoundToInt(attack / total * 100);
        defend = Mathf.RoundToInt(defend / total * 100);
        item = Mathf.RoundToInt(item / total * 100);
        magic = Mathf.RoundToInt(magic / total * 100);
        run = Mathf.RoundToInt(run / total * 100);

        attack += 100 - GetTotal();

        ClampData();
    }

    public void ClampData()
    {
        attack = Mathf.Clamp(attack, 0, 100);
        defend = Mathf.Clamp(defend, 0, 100);
        item = Mathf.Clamp(item, 0, 100);
        magic = Mathf.Clamp(magic, 0, 100);
        run = Mathf.Clamp(run, 0, 100);

    }

}
