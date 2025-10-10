using Unity.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField, ReadOnly] private string id;
    [SerializeField, ReadOnly] private string heroName;
    [SerializeField, ReadOnly] private int hp;
    [SerializeField, ReadOnly] private int power;
    [SerializeField, ReadOnly] private int[] selectedWeapons;
    
    public string Id => id;
    
    public void Init(HeroData data)
    {
        id = data.Id;
        heroName = data.Name;
        hp = data.Hp;
        power = data.Power;
        selectedWeapons = data.SelectedWeapons.ToArray();
    }
}