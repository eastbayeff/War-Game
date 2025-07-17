using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] coverPoints;
    private void Start()
    {
        coverPoints = GameObject.FindGameObjectsWithTag("CoverPoint");
    }
}
