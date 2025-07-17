using UnityEngine;

public class CoverPointScript : MonoBehaviour
{
    bool inPlayerRange = false;
    public bool inLineOfSight = false;

    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("RotationalPoint"))
        {
            inPlayerRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("RotationalPoint"))
        {
            inPlayerRange = false;
        }
    }

    private void FixedUpdate()
    {
        if (inPlayerRange)
        {
            int layerMask = LayerMask.GetMask("Player", "Obstacles");

            RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 100f, layerMask);

            if(ray.collider != null)
            {

                if (ray.collider.CompareTag("Player"))
                {
                    inLineOfSight = true;
                }
                else
                {
                    inLineOfSight = false;
                }
                
                if (inLineOfSight)
                {
                    Debug.DrawLine(transform.position, player.transform.position, Color.green);
                }
                else
                {
                    Debug.DrawLine(transform.position, player.transform.position, Color.red);
                }
            }
        }
    }
}
