using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    Vector2 movement;
    Rigidbody2D rb;

    [SerializeField]
    float movespeed = 100f;

    [SerializeField]
    float scrollSpeed = 1f;

    [SerializeField]
    CinemachineCamera camera;

    [SerializeField]
    private Image staminabar;

    [SerializeField]
    float movementMultiplier = 1f;

    [SerializeField]
    float staminadecreaseamount = 1f;

    [SerializeField]
    float staminaincreaseamount = 1f;
    bool canSprint = true;
    public float threshold = 0.01f;
    public float GunFocusSpeed = 0.5f;
    public float MaxAccuracyVariance = 1f;
    public float GunUnFocusSpeed = 0.5f;

    [Header("CrossHair Pieces")]
    [SerializeField]
    GameObject top;

    [SerializeField]
    GameObject bottom;

    [SerializeField]
    GameObject left;

    [SerializeField]
    GameObject right;
    public float crosshairmovmentAmount;

    Inventory inventory;
    private Guns weapon;

    public float stamina = 100f;
    private float staminastart;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        staminastart = stamina;
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        weapon = inventory.currentWeapon;

        GunFocusSpeed = weapon.GunFocusSpeed;
        GunUnFocusSpeed = weapon.GunUnFocusSpeed;
        MaxAccuracyVariance = weapon.MaxAccuracyVariance;

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement = Vector2.ClampMagnitude(movement, 1f);

        rb.linearVelocity = movement * movespeed * movementMultiplier * Time.deltaTime;

        camera.Lens.OrthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        camera.Lens.OrthographicSize = Mathf.Clamp(camera.Lens.OrthographicSize, 4f, 13f);

        staminabar.fillAmount = stamina / 100;

        stamina = Mathf.Clamp(stamina, 0f, staminastart);

        if (canSprint)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementMultiplier = 1.5f;
                stamina -= staminadecreaseamount * Time.deltaTime;
            }
        }

        if (stamina < 100)
        {
            movementMultiplier = 1f;
            stamina += staminaincreaseamount * Time.deltaTime;
        }

        if (stamina <= 0)
        {
            canSprint = false;
            staminabar.color = Color.red;
        }
        if (stamina > 10)
        {
            canSprint = true;
            staminabar.color = Color.yellow;
        }

        if (rb.linearVelocity.magnitude > threshold)
        {
            crosshairmovmentAmount += Time.deltaTime * GunUnFocusSpeed;
        }
        else
        {
            crosshairmovmentAmount -= Time.deltaTime * GunFocusSpeed;
        }

        crosshairmovmentAmount = Mathf.Clamp(crosshairmovmentAmount, 0.2f, MaxAccuracyVariance);

        Vector3 ttemp = top.transform.localPosition;
        ttemp.y = crosshairmovmentAmount;
        top.transform.localPosition = ttemp;

        Vector3 btemp = bottom.transform.localPosition;
        btemp.y = -crosshairmovmentAmount;
        bottom.transform.localPosition = btemp;

        Vector3 ltemp = left.transform.localPosition;
        ltemp.x = -crosshairmovmentAmount;
        left.transform.localPosition = ltemp;

        Vector3 rtemp = right.transform.localPosition;
        rtemp.x = crosshairmovmentAmount;
        right.transform.localPosition = rtemp;
    }
}
