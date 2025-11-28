using UnityEngine;

public class MovePointGenerate : MonoBehaviour
{
    [SerializeField] private int generateNum = 30;
    [SerializeField] private Vector2 range = new Vector2(50, 50);

    void Start()
    {
        var parent = new GameObject("MovePoints");
        parent.transform.SetParent(transform);

        for (int i = 0; i < generateNum; i++)
        {
            var movePoint = new GameObject($"MovePoint({i})");
            movePoint.transform.parent = parent.transform;
            movePoint.transform.position = new Vector3(
                Random.Range(-range.x, range.x),
                0,
                Random.Range(-range.y, range.y)
            );

            var sphere = movePoint.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 0.5f;

            var rb = movePoint.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
