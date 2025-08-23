using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] coverPoints;
    private void Start()
    {
        coverPoints = GameObject.FindGameObjectsWithTag("CoverPoint");

        Application.targetFrameRate = 60; // or any number you want
        QualitySettings.vSyncCount = 1;   // 0 = off, 1 = every VBlank
    }
}
