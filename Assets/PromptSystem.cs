using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class PromptSystem : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI currentAmmo;
    [SerializeField] TextMeshProUGUI maxAmmo;

    public Guns weapon;
    public GunSO gunSO;

    private void Update()
    {
        name.text = weapon.name;
        description.text = weapon.description;
        currentAmmo.text = gunSO.currentAmmo.ToString();
        maxAmmo.text = weapon.MaxAmmo.ToString();
    }
}
