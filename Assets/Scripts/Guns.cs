using UnityEngine;

[CreateAssetMenu(fileName = "Guns", menuName = "Scriptable Objects/Guns")]
public class Guns : ScriptableObject
{
    public new string Name;
    public string description;

    public Sprite art;
    public GameObject prefab;

    public float GunFocusSpeed;
    public float MaxAccuracyVariance;
    public float GunUnFocusSpeed;
    public float GunRecoil;

    public float Damage;

    public bool Automatic;
    public float fireRate;
    public float reloadSpeed;
    public int currentAmmo;
    public int MaxAmmo;

    public float SoundAreaMultiplier;
}
