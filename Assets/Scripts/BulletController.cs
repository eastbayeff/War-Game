using UnityEngine;

public class BulletController : MonoBehaviour
{
    float lifetime = 3.0f;

    [SerializeField]
    GameObject gunRotation;

    [SerializeField]
    float bulletSpeed;
    public Vector3 mouseposition;
    public PlayerScript playerS;
    Vector3 rotationAxis = Vector3.up;

    GameObject player;
    private ObjectPoolNew poolNew;

    public void Initialise()
    {
        mouseposition.z = 0;

        transform.position = GameObject.Find("Gun").transform.position;
        gameObject.GetComponent<TrailRenderer>().enabled = true;

        Quaternion randomRotation = Quaternion.Euler(
            0,
            0,
            Random.Range(-playerS.crosshairmovmentAmount * 12, playerS.crosshairmovmentAmount * 12)
        );

        Vector3 direction = (mouseposition - transform.position).normalized;

        Vector3 rotatedVector = randomRotation * direction;

        GetComponent<Rigidbody2D>().linearVelocity = rotatedVector * bulletSpeed * Time.deltaTime;

        poolNew = GameObject.Find("ObjectPool").GetComponent<ObjectPoolNew>();

        lifetime = 3.0f;    
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            poolNew.DestroyBullet(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        poolNew.DestroyBullet(gameObject);
    }
}
