using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunRotation : MonoBehaviour
{
    private Camera maincamera;
    public Vector3 mousepos;
    public BulletController controller;

    [SerializeField]
    Transform gun;

    [SerializeField]
    float bulletSpeed;

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject BulletTrail;
    public float angle;
    public float range = 15f;
    private PlayerScript playerScript;
    Inventory inventory;
    float nextFire = 0f;

    [SerializeField]
    TextMeshProUGUI ammoText;

    [SerializeField]
    TextMeshProUGUI maxAmmoText;

    [Header("Current Weapon")]
    public bool automatic;
    public float fireRate = 0.2f;
    public bool CanShoot = true;
    public int ammo;
    public int maxAmmo;
    public Image selectedImage;

    GameObject SoundArea;
    CircleCollider2D soundAreaCollider;

    [SerializeField]
    Guns weapon;

    [SerializeField]
    ObjectPoolNew poolNew;

    public void ResetWeapon(int currentA)
    {
        weapon = inventory.currentWeapon;
        maxAmmo = weapon.MaxAmmo;
        ammo = currentA;
        selectedImage.sprite = weapon.art;
    }

    private void Start()
    {
        maincamera = Camera.main;
        playerScript = player.GetComponent<PlayerScript>();
        inventory = gameObject.transform.GetComponentInParent<Inventory>();
        weapon = inventory.currentWeapon;
        maxAmmo = weapon.MaxAmmo;
        ammo = weapon.currentAmmo;
        selectedImage.sprite = weapon.art;
        SoundArea = gameObject.transform.Find("SoundArea").gameObject;
        soundAreaCollider = SoundArea.GetComponent<CircleCollider2D>();

        SoundArea.GetComponent<CircleCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        maxAmmoText.text = maxAmmo.ToString();
        ammoText.text = ammo.ToString();
        weapon = inventory.currentWeapon;
        Vector3 mousepos = maincamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousepos - transform.position;

        angle =
            Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x)
            * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        automatic = weapon.Automatic;
        fireRate = weapon.fireRate;

        soundAreaCollider.radius = 8 * weapon.SoundAreaMultiplier;


        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo = maxAmmo;
        }

        if (ammo == 0)
        {
            CanShoot = false;
        }
        else
        {
            CanShoot = true;
        }

        if (CanShoot)
        {
            if (weapon.Automatic)
            {
                if (Input.GetMouseButton(0) && Time.time > nextFire)
                {
                    poolNew.pool.Get();
                    SoundArea.GetComponent<CircleCollider2D>().enabled = true;
                    ammo -= 1;
                    playerScript.crosshairmovmentAmount += weapon.GunRecoil;
                    nextFire = Time.time + fireRate;
                    StartCoroutine(disableCollider());
                }
            }

            if (weapon.Automatic == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    poolNew.pool.Get();
                    ammo -= 1;
                    playerScript.crosshairmovmentAmount += weapon.GunRecoil;
                    SoundArea.GetComponent<CircleCollider2D>().enabled = true;
                    StartCoroutine(disableCollider());
                }
            }
        }

    }

    IEnumerator disableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        SoundArea.GetComponent<CircleCollider2D>().enabled = false;
    }
}
