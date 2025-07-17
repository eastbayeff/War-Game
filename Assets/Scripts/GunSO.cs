using UnityEngine;

public class GunSO : MonoBehaviour
{
    public Guns Guns;

    Inventory inventory;

    public int currentAmmo;
    public int maxAmmo;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Guns.art;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        maxAmmo = Guns.MaxAmmo;
    }

    public void PickUp()
    {
        inventory.currentWeapon = Guns;
        Destroy(gameObject);
    }
}
