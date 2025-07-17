using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolNew : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject enemybulletPrefab;
    private GameObject bulletIns;
    public ObjectPool<GameObject> pool;
    public ObjectPool<GameObject> enemyPool;

    [SerializeField]
    int BulletAmount = 50;

    [SerializeField]
    private PlayerScript playerScript;

    Vector3 mousepos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pool = new ObjectPool<GameObject>(
            () =>
            {
                return Instantiate(bulletPrefab);
            },
            GameObject =>
            {
                GameObject.SetActive(true);
                GameObject.GetComponent<BulletController>().mouseposition = mousepos;
                GameObject.GetComponent<BulletController>().playerS = playerScript;
                GameObject.GetComponent<BulletController>().Initialise();
            },
            GameObject =>
            {
                GameObject.SetActive(false);
            },
            GameObject =>
            {
                Destroy(GameObject.gameObject);
            },
            true,
            10,
            100
        );

        enemyPool = new ObjectPool<GameObject>(
            () =>
            {
                return Instantiate(enemybulletPrefab);
            },
            GameObject =>
            {
                GameObject.SetActive(true);
                GameObject.GetComponent<EnemyBulletController>().playerS = playerScript;
                GameObject.GetComponent<EnemyBulletController>().InitializeEnemy();
            },
            GameObject =>
            {
                GameObject.SetActive(false);
            },
            GameObject =>
            {
                Destroy(GameObject.gameObject);
            },
            true,
            10,
            100
        );

        maincamera = Camera.main;
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < BulletAmount; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
        }
        for (int i = 0; i < BulletAmount; i++)
        {
            GameObject obj = Instantiate(enemybulletPrefab);
            obj.SetActive(false);
        }
    }

    public void DestroyBullet(GameObject bullet)
    {
        pool.Release(bullet);
    }

    public void DestroyEnemeyBullet(GameObject bullet)
    {
        enemyPool.Release(bullet);
    }

    Camera maincamera;

    private void Update()
    {
        mousepos = maincamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
