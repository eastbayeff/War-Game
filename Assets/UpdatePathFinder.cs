using Pathfinding;
using UnityEngine;

public class UpdatePathFinder : MonoBehaviour
{
    public float updateInterval = 1f;
    public float updateRadius = 30f;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateGraphAroundObject), 0f, updateInterval);
    }

    void UpdateGraphAroundObject()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.one * updateRadius);
        GraphUpdateObject obj = new GraphUpdateObject(bounds);
        AstarPath.active.UpdateGraphs(obj);
    }
}

