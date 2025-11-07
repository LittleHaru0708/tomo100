using UnityEngine;

public class MovePointGenerate : MonoBehaviour
{
    [SerializeField] private int _generateNum = 30;
    [SerializeField] private Vector2 _range = new Vector2(50, 50);

    void Start()
    {
        var parent = new GameObject("MovePoints");

        for (int i = 0; i < _generateNum; i++)
        {
            var movePoint = new GameObject($"MovePoint({i})");
            movePoint.tag = "MovePoint";
            movePoint.layer = LayerMask.NameToLayer("MovePoint");
            movePoint.transform.parent = parent.transform;

            // コライダー追加（OverlapSphere検出用）
            var sphere = movePoint.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 0.5f;

            var rb = movePoint.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            movePoint.transform.position = new Vector3(
                Random.Range(-_range.x, _range.x),
                0,
                Random.Range(-_range.y, _range.y)
            );
        }
    }
}
