using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Investigate,
        Cover,
        LocatePlayer,
        Attack,
        Dead,
        Flee,
    }

    public EnemyState enemyState;

    private AIPath aiPath;

    public float Health = 100;

    [Header("Movement")]
    [SerializeField]
    float stopDistanceThreshold;
    private float distanceToTarget;

    [SerializeField]
    float movespeed;

    [SerializeField]
    Vector3 target;

    [SerializeField]
    List<GameObject> points;

    [SerializeField]
    int pointcount = 0;

    SpriteRenderer rend;

    [Header("Bools")]
    bool Hit = false;

    [SerializeField]
    bool isPointBased = false;

    [SerializeField]
    bool isTakingCover = false;

    private bool _atShootPosition = false;

    private bool Agro = false;
    bool Locating = false;

    [Header("Shooting")]
    [SerializeField]
    float shootDistance = 10f;

    [SerializeField]
    float shootTime;

    [SerializeField]
    float radius = 2f;

    [SerializeField]
    float attackLength;

    [Header("Cover")]
    private GameObject Player;
    public float coverWaittime;

    [SerializeField]
    ObjectPoolNew poolNew;

    float LocateTime;

    Vector3 LastKnownPosition;

    [SerializeField]
    Guns weapon;

    float crosshairmovmentAmount;
    float MaxAccuracyVariance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SoundArea"))
        {
            LastKnownPosition = collision.transform.position;
            AtShootPosition = false;
            randomInvestigationPosition = Vector3.zero;
            enemyState = EnemyState.Investigate;
        }
    }

    void ResetInvestigationTimes()
    {
        pause = Random.Range(1f, 2f);
        investigateTime = Random.Range(8f, 10f);
    }

    public void AgroEnemy()
    {
        Agro = true;
    }

    public void DeAgroEnemy()
    {
        Agro = false;
    }

    public bool AtShootPosition
    {
        get { return _atShootPosition; }
        set
        {
            if (_atShootPosition != value)
            {
                _atShootPosition = value;
                ResetInvestigationTimes();
            }
            ;
        }
    }

    [SerializeField]
    float pause;

    [SerializeField]
    float investigateTime;

    Vector3 randomInvestigationPosition = Vector3.zero;

    Vector3 playerpos;

    private void Start()
    {
        poolNew = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPoolNew>();

        coverWaittime = Random.Range(3f, 5f);
        shootTime = 1f;
        Player = GameObject.FindGameObjectWithTag("Player");
        attackLength = Random.Range(2f, 3f);

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
            target = points[0].transform.position;
        }

        MaxAccuracyVariance = weapon.MaxAccuracyVariance;
    }

    Vector3 GetRandomPositionAroundSound()
    {
        return LastKnownPosition + Random.insideUnitSphere * radius;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Hit = true;
            Health -= 10;
        }
    }

    private void Update()
    {
        crosshairmovmentAmount = Random.Range(0f, MaxAccuracyVariance);

        crosshairmovmentAmount = Mathf.Clamp(crosshairmovmentAmount, 0.2f, MaxAccuracyVariance);

        int layerMask = LayerMask.GetMask("Player", "Obstacles");

        if (Agro && !Locating)
        {
            RaycastHit2D visionhit = Physics2D.Raycast(
                gameObject.transform.position,
                Player.transform.position - transform.position,
                7f,
                layerMask
            );

            if (visionhit.collider != null)
            {
                if (visionhit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player Spotted!");
                    enemyState = EnemyState.LocatePlayer;
                    LocateTime = Random.Range(8, 12);
                    target = Player.transform.position;
                    AtShootPosition = false;
                    randomInvestigationPosition = Vector3.zero;

                    Debug.DrawRay(
                        transform.position,
                        Player.transform.position - transform.position,
                        Color.green
                    );
                }
            }
            else
            {
                Debug.DrawRay(
                    transform.position,
                    Player.transform.position - transform.position,
                    Color.red
                );
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(
            gameObject.transform.position,
            gameObject.transform.up,
            shootDistance,
            layerMask
        );

        if (isTakingCover)
        {
            enemyState = EnemyState.Cover;
        }

        Debug.Log(enemyState);
        aiPath.destination = target;
        aiPath.maxSpeed = movespeed;

        distanceToTarget = Vector2.Distance(target, transform.position);

        switch (enemyState)
        {
            case EnemyState.Patrol:
                Locating = false;

                if (isPointBased)
                {
                    target = points[pointcount].transform.position;

                    if (distanceToTarget < stopDistanceThreshold)
                    {
                        pointcount += 1;
                    }
                    else
                    {
                        aiPath.destination = target;
                    }

                    if (pointcount >= points.Count)
                    {
                        pointcount = 0;
                    }
                }
                break;

            case EnemyState.Investigate:

                Locating = false;
                // set our target based on whether we're randomly investigating or headed to the last known position
                target =
                    randomInvestigationPosition == Vector3.zero
                        ? LastKnownPosition
                        : randomInvestigationPosition;

                var arrived = distanceToTarget < stopDistanceThreshold;

                // did we get to the place we heard the shot?
                AtShootPosition = arrived && target == LastKnownPosition;

                // did we get to the random point we were investigating?
                if (arrived && target == randomInvestigationPosition)
                {
                    randomInvestigationPosition = GetRandomPositionAroundSound();
                    if (
                        Physics2D.OverlapPoint(
                            randomInvestigationPosition,
                            LayerMask.GetMask("Obstacles")
                        )
                    )
                    {
                        randomInvestigationPosition = GetRandomPositionAroundSound();
                    }
                    target = randomInvestigationPosition;
                }

                // if we're investgating, reduce the investigate timer
                if (randomInvestigationPosition != Vector3.zero)
                    investigateTime -= Time.deltaTime;

                if (investigateTime <= 0)
                {
                    randomInvestigationPosition = Vector3.zero;
                    enemyState = EnemyState.Patrol;
                    Debug.Log("Done Investigating");
                }

                if (AtShootPosition)
                {
                    pause -= Time.deltaTime;

                    // once the pause timer has expired, we're no longer at the shoot position, so we start randomly investigating
                    if (pause <= 0)
                    {
                        AtShootPosition = false;
                        randomInvestigationPosition = GetRandomPositionAroundSound();
                        target = randomInvestigationPosition;
                    }
                }

                break;

            case EnemyState.Cover:
                GameObject coverpoint = findClosestCoverPoint();
                target = coverpoint.transform.position;

                coverWaittime -= Time.deltaTime;
                if (coverWaittime <= 0)
                {
                    enemyState = EnemyState.LocatePlayer;
                    LocateTime = Random.Range(8, 12);
                    coverWaittime = Random.Range(3f, 5f);
                }

                Locating = false;
                break;
            case EnemyState.LocatePlayer:
                target = Player.transform.position;

                Locating = true;

                GetComponent<AIPath>().enableRotation = true;
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Hit!");
                        target = transform.position;

                        enemyState = EnemyState.Attack;
                    }
                }

                LocateTime -= Time.deltaTime;

                if (LocateTime <= 0 && Agro == false)
                {
                    enemyState = EnemyState.Patrol;
                    LocateTime = Random.Range(8, 12);
                }

                break;
            case EnemyState.Attack:

                Locating = false;

                playerpos = Player.transform.position;

                GetComponent<AIPath>().enableRotation = false;

                Vector3 rotation = playerpos - transform.position;

                float angle =
                    Mathf.Atan2(
                        playerpos.y - transform.position.y,
                        playerpos.x - transform.position.x
                    ) * Mathf.Rad2Deg;

                gameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                shootTime -= Time.deltaTime;
                attackLength -= Time.deltaTime;

                if (shootTime <= 0)
                {
                    GameObject bullet = poolNew.enemyPool.Get();
                    bullet.transform.position = transform.Find("Front").position;
                    bullet
                        .GetComponent<EnemyBulletController>()
                        .InitializeEnemy(weapon, crosshairmovmentAmount);
                    shootTime = 1f;
                }

                if (attackLength <= 0)
                {
                    enemyState = EnemyState.Cover;
                    attackLength = Random.Range(2f, 3f);
                }

                if (hit.collider != null)
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        enemyState = EnemyState.LocatePlayer;
                        LocateTime = Random.Range(8, 12);
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

    GameObject findClosestCoverPoint()
    {
        GameObject[] objs = GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponent<GameManager>()
            .coverPoints;

        GameObject closestEnemy = null;
        float closestDistance = 0f;
        bool first = true;
        foreach (var obj in objs)
        {
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            if (obj.GetComponent<CoverPointScript>().inLineOfSight == false)
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
