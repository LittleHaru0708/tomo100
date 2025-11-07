using UnityEngine;
using UnityEngine.AI;
using Unity;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float targetDistance = 10f;
    public float _searchRadius = 20f;
    public LayerMask mask;

    private NavMeshAgent agent;
    private float escapeTime;
    private enum EnemyState { NORMAL, TENSION }
    private EnemyState currentState = EnemyState.NORMAL;

    private List<Collider> m_foundList = new List<Collider>();
    private float m_searchCosTheta = Mathf.Cos(90 * Mathf.Deg2Rad);

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = (transform.position - player.position).magnitude;
        if (currentState == EnemyState.NORMAL && distance < targetDistance)
        {
            RunAway();
        }
    }

    public void RunAway()
    {
        currentState = EnemyState.TENSION;
        escapeTime = 0;
        agent.angularSpeed = 200;

        // プレイヤーと逆方向を向く
        var diff = (transform.position - player.position).normalized;
        diff.y = 0;
        transform.forward = diff;

        // 逃げ開始
        agent.SetDestination(GetNextPosition());

        // 次の目的地に着いたら次を探す
        Observable.EveryUpdate()
            .TakeWhile(_ => currentState == EnemyState.TENSION)
            .Where(_ => agent.remainingDistance < 2.0f)
            .Subscribe(_ =>
            {
                agent.SetDestination(GetNextPosition());
            }).AddTo(this);

        // 一定距離離れたら通常状態に戻す
        Observable.EveryUpdate()
            .TakeWhile(_ => currentState == EnemyState.TENSION)
            .Subscribe(_ =>
            {
                float dist = (transform.position - player.position).sqrMagnitude;
                if (dist > targetDistance * targetDistance)
                {
                    escapeTime += Time.deltaTime;
                    if (escapeTime >= 10f)
                        Usual();
                }
            }).AddTo(this);
    }

    void Usual()
    {
        currentState = EnemyState.NORMAL;
        agent.ResetPath();
    }

    public Vector3 GetNextPosition()
    {
        m_foundList.Clear();
        m_foundList.AddRange(Physics.OverlapSphere(transform.position, _searchRadius, mask));

        if (m_foundList.Count == 0)
        {
            // 近くにない場合は広範囲検索
            m_foundList.AddRange(Physics.OverlapSphere(transform.position, 40f, mask));
        }
        else
        {
            // プレイヤー方向にあるものを除外
            for (int i = m_foundList.Count - 1; i >= 0; i--)
            {
                if (!CheckFoundObject(m_foundList[i].gameObject))
                    m_foundList.RemoveAt(i);
            }
        }

        if (m_foundList.Count == 0) return transform.position;
        return m_foundList[Random.Range(0, m_foundList.Count)].transform.position;
    }

    private bool CheckFoundObject(GameObject target)
    {
        var myXZ = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        var targetXZ = Vector3.Scale(target.transform.position, new Vector3(1, 0, 1));
        var toTargetDir = (targetXZ - myXZ).normalized;

        if (toTargetDir.sqrMagnitude <= Mathf.Epsilon) return true;
        return Vector3.Dot(transform.forward, toTargetDir) >= m_searchCosTheta;
    }
}
