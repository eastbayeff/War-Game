using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Patrol,
        Investigate,
        Cover,
        LocatePlayer,
        Attack,
        Dead,
        Flee,
    }

    private AIPath aiPath;

    public float Health = 100;

    [SerializeField]
    float movespeed;

    [SerializeField]
    Transform target;

    [SerializeField]
    float stopDistanceThreshold;
    private float distanceToTarget;

    SpriteRenderer rend;
    bool Hit = false;

    [SerializeField]
    bool isPointBased = false;

    [SerializeField]
    List<GameObject> points;

    [SerializeField]
    int pointcount = 0;

    private EnemyState enemyState;

    [SerializeField] bool isTakingCover = false;
    [SerializeField] float shootDistance = 10f;

    private GameObject Player;
    public float coverWaittime;

    [SerializeField]
    ObjectPoolNew poolNew;

    [SerializeField]float shootTime;

    [SerializeField] float radius = 2f;

    private bool _atShootPosition = false;

    void ResetInvestigationTimes()
    {
        pause = Random.Range(1f, 2f);
        investigateTime = Random.Range(8f, 10f);
    }

    public bool AtShootPosition
    {
        get { return _atShootPosition; }
        set { if (_atShootPosition != value) { 
                _atShootPosition = value;
                ResetInvestigationTimes();
            }; }
    }


    [SerializeField] float pause;
    [SerializeField] float investigateTime;

    bool HasInvestigatedRandomTarget = false;


    Vector3 playerpos;
    private void Start()
    {
        poolNew = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPoolNew>();

        coverWaittime = Random.Range(3f, 5f);
        shootTime = 1f;
        Player = GameObject.FindGameObjectWithTag("Player");

        rend = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();

        EnemyState enemyState = EnemyState.Patrol;

        if (isPointBased)
        {
            foreach (Transform sibling in transform)
            {
                if (sibling.CompareTag("Point"))
                {
                    points.Add(sibling.gameObject);
                }
            }
        }

        if (points.Count > 0)
        {
            target = points[0].transform;
        }
    }




    void RandomPosition()
    {
        target.position = transform.position + Random.insideUnitSphere * radius;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Hit = true;
            Health -= 10;
        }
    }

    Transform playerposition;
    Vector3 LastKnownPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("SoundArea"))
        {
            playerposition = collision.gameObject.transform;
            enemyState = EnemyState.Investigate;
            LastKnownPosition = playerposition.transform.position;
            AtShootPosition = false;
        }
    }

    private void Update()
    {
        int layerMask = LayerMask.GetMask("Player", "Obstacles");

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, shootDistance, layerMask);

        if (isTakingCover)
        {
            enemyState = EnemyState.Cover;
        }

        Debug.Log(enemyState);
        aiPath.destination = target.position;
        aiPath.maxSpeed = movespeed;

        distanceToTarget = Vector2.Distance(target.position, transform.position);

        switch (enemyState)
        {
            case EnemyState.Patrol:
                if (isPointBased)
                {
                    target = points[pointcount].transform;

                    if (distanceToTarget < stopDistanceThreshold)
                    {
                        pointcount += 1;
                    }
                    else
                    {
                        aiPath.destination = target.position;
                    }

                    if (pointcount >= points.Count)
                    {
                        pointcount = 0;
                    }
                }
                break;

            case EnemyState.Investigate:
                target.position = LastKnownPosition;


                if (target.position == LastKnownPosition && distanceToTarget < stopDistanceThreshold)
                {
                    AtShootPosition = true;
                }


                if(AtShootPosition)
                {


                    pause -= Time.deltaTime;
                    if(pause <= 0)
                    {
                        investigateTime -= Time.deltaTime;

          
                        if (!HasInvestigatedRandomTarget)
                        {
                            RandomPosition();
                        }
                        else if(investigateTime >= 0 && distanceToTarget < stopDistanceThreshold)
                        {
                            HasInvestigatedRandomTarget = true;
                        }
                    }


                }

                break;

            case EnemyState.Cover:
                GameObject coverpoint = findClosestCoverPoint();
                target = coverpoint.transform;

                coverWaittime -= Time.deltaTime;
                if(coverWaittime <= 0)
                {
                    enemyState = EnemyState.LocatePlayer;
                    coverWaittime = Random.Range(3f, 5f);
                }
                break;
            case EnemyState.LocatePlayer:
                target = Player.transform;

                GetComponent<AIPath>().enableRotation = true;
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Hit!");
                        target = transform;

                        enemyState = EnemyState.Attack;
                    }
                }
                break;
            case EnemyState.Attack:

                playerpos = Player.transform.position;

                GetComponent<AIPath>().enableRotation = false;

                Vector3 rotation = playerpos - transform.position;

                float angle =
                    Mathf.Atan2(playerpos.y - transform.position.y, playerpos.x - transform.position.x)
                    * Mathf.Rad2Deg;

                gameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                shootTime -= Time.deltaTime;

                if(shootTime <= 0) { 
                                GameObject bullet = poolNew.enemyPool.Get();
                bullet.transform.position = transform.Find("Front").position;
                    shootTime = 1f;
                }

           

                if (hit.collider != null)
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        enemyState = EnemyState.LocatePlayer;
                    }
                }

                break;

            case EnemyState.Dead:
                break;

            case EnemyState.Flee:
                break;

            default:
                break;
        }
    }

    private GameObject findClosestCoverPoint()
    {
        GameObject[] objs = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().coverPoints;


        GameObject closestEnemy = null;
        float closestDistance = 0f;
        bool first = true;
        foreach (var obj in objs)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if(obj.GetComponent<CoverPointScript>().inLineOfSight == false)
            {
                if (first)
                {
                    closestDistance = distance;

                    first = false;
                }
                else if (distance < closestDistance)
                {
                    closestEnemy = obj;
                    closestDistance = distance;
                }
            }
        }
        return closestEnemy;
    }
}
