using UnityEngine;

public class EnemyBulletController : MonoBehaviour
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

    private void Start()
    {
        poolNew = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPoolNew>();
    }

    public void InitializeEnemy(Guns gun, float movementamount)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        mouseposition.z = 0;

        gameObject.GetComponent<TrailRenderer>().enabled = true;

        Quaternion randomRotation = Quaternion.Euler(
            0,
            0,
            Random.Range(-movementamount * 12, movementamount * 12)
        );
        
        Vector3 direction = (player.transform.position - transform.position).normalized;

        Vector3 rotatedVector = randomRotation * direction;

        GetComponent<Rigidbody2D>().linearVelocity = rotatedVector * bulletSpeed * Time.deltaTime;


        lifetime = 3.0f;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            poolNew.DestroyEnemeyBullet(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        poolNew.DestroyEnemeyBullet(gameObject);
    }
}
