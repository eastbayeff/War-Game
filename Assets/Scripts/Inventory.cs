using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public Guns currentWeapon;

    [SerializeField]
    GameObject obj;

    private bool isInPickUpZone;

    GunRotation gunRotation;

    [SerializeField] GameObject PromptSystemObject;

    private void Start()
    {
        gunRotation = GetComponentInChildren<GunRotation>();
        PromptSystemObject.SetActive(false);
    }

    private void Update()
    {

        if (isInPickUpZone && Input.GetKeyDown(KeyCode.F) && obj != null)
        {
            Debug.Log("Pressed");
            if (currentWeapon == null)
            {
                int CA = obj.GetComponent<GunSO>().currentAmmo;
                obj.GetComponent<GunSO>().PickUp();
                gunRotation.ResetWeapon(CA);
            }
            if (currentWeapon != null)
            {
                Drop();
                int CA = obj.GetComponent<GunSO>().currentAmmo;
                obj.GetComponent<GunSO>().PickUp();
                gunRotation.ResetWeapon(CA);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gun"))
        {
            isInPickUpZone = true;
            obj = collision.gameObject;
            PromptSystemObject.SetActive(true);
            PromptSystemObject.GetComponent<PromptSystem>().weapon = obj.GetComponent<GunSO>().Guns;
            PromptSystemObject.GetComponent<PromptSystem>().gunSO = obj.GetComponent<GunSO>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gun"))
        {
            isInPickUpZone = false;
            obj = null;
            PromptSystemObject.SetActive(false);
        }
    }

    void Drop()
    {
        GameObject clone = Instantiate(currentWeapon.prefab);
        GunSO gunSO = clone.GetComponent<GunSO>();
        gunSO.currentAmmo = gunRotation.ammo;
    }
}
