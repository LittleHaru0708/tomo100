using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float targetDistance = 10f;
    public float searchRadius = 20f;
    public LayerMask mask;

    private NavMeshAgent agent;
    private float escapeTime;
    private enum EnemyState { NORMAL, TENSION }
    private EnemyState currentState = EnemyState.NORMAL;

    private List<Collider> foundList = new List<Collider>();
    private float searchCosTheta = Mathf.Cos(90 * Mathf.Deg2Rad);

    private Coroutine escapeRoutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (currentState == EnemyState.NORMAL)
        {
            float distance = (transform.position - player.position).magnitude;
            if (distance < targetDistance)
            {
                RunAway();
            }
        }
    }

    public void RunAway()
    {
        if (escapeRoutine != null)
            StopCoroutine(escapeRoutine);

        escapeRoutine = StartCoroutine(EscapeRoutine());
    }

    private IEnumerator EscapeRoutine()
    {
        currentState = EnemyState.TENSION;
        escapeTime = 0f;
        agent.angularSpeed = 200;

        // プレイヤーと逆方向を向く
        Vector3 diff = (transform.position - player.position).normalized;
        diff.y = 0;
        transform.forward = diff;

        // 最初の目的地
        agent.SetDestination(GetNextPosition());

        while (currentState == EnemyState.TENSION)
        {
            // 次の目的地へ
            if (!agent.pathPending && agent.remainingDistance < 2f)
            {
                agent.SetDestination(GetNextPosition());
            }

            // プレイヤーと距離が離れたら通常へ戻す
            float dist = (transform.position - player.position).sqrMagnitude;
            if (dist > targetDistance * targetDistance)
            {
                escapeTime += Time.deltaTime;
                if (escapeTime >= 10f)
                {
                    Usual();
                    yield break;
                }
            }

            yield return null;
        }
    }

    void Usual()
    {
        currentState = EnemyState.NORMAL;
        agent.ResetPath();
    }

    public Vector3 GetNextPosition()
    {
        foundList.Clear();
        foundList.AddRange(Physics.OverlapSphere(transform.position, searchRadius, mask));

        if (foundList.Count == 0)
        {
            foundList.AddRange(Physics.OverlapSphere(transform.position, 40f, mask));
        }
        else
        {
            for (int i = foundList.Count - 1; i >= 0; i--)
            {
                if (!CheckFoundObject(foundList[i].gameObject))
                    foundList.RemoveAt(i);
            }
        }

        if (foundList.Count == 0)
            return transform.position;

        return foundList[Random.Range(0, foundList.Count)].transform.position;
    }

    private bool CheckFoundObject(GameObject target)
    {
        Vector3 myXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetXZ = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 toTargetDir = (targetXZ - myXZ).normalized;

        if (toTargetDir.sqrMagnitude <= Mathf.Epsilon)
            return true;

        return Vector3.Dot(transform.forward, toTargetDir) >= searchCosTheta;
    }
}
