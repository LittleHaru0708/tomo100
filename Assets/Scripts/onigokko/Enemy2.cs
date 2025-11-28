using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy2 : MonoBehaviour
{
    public static Enemy2 currentIt = null;
    public bool isIt = false;

    private NavMeshAgent agent;
    private bool isSwitchingIt = false;
    private bool isCooldown = false;
    private float switchDelay = 2f;
    private float tagCooldown = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateColor();

        // ランダムで最初の鬼を設定
        if (isIt) currentIt = this;
    }

    void Update()
    {
        UpdateColor();

        if (isIt && !isSwitchingIt)
            ChaseTarget();
        else if (!isIt && !isSwitchingIt)
            Wander();
    }

    void UpdateColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = isIt ? Color.red : Color.white;
    }

    void ChaseTarget()
    {
        Transform target = FindNearestTarget();
        if (target == null) return;

        agent.SetDestination(target.position);

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 1.5f)
        {
            StartCoroutine(TagTarget(target));
        }
    }

    Transform FindNearestTarget()
    {
        List<Transform> candidates = new List<Transform>();

        PlayerController2[] players = FindObjectsOfType<PlayerController2>();
        foreach (var p in players)
            if (!p.isIt) candidates.Add(p.transform);

        Enemy2[] enemies = FindObjectsOfType<Enemy2>();
        foreach (var e in enemies)
            if (!e.isIt && e != this) candidates.Add(e.transform);

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    IEnumerator TagTarget(Transform newIt)
    {
        // クールタイム中は何もしない
        if (isCooldown) yield break;
        isCooldown = true;

        // 現在の鬼は停止
        isSwitchingIt = true;
        yield return new WaitForSeconds(switchDelay);
        isSwitchingIt = false;

        // 新しい鬼を設定
        PlayerController2 p = newIt.GetComponent<PlayerController2>();
        if (p != null && p.CanBeTagged())
        {
            p.isIt = true;
            PlayerController2.currentPlayerIt = p;
            StartCoroutine(p.HandleSwitchDelay());
            StartCoroutine(p.StartCooldown());
        }
        else
        {
            Enemy2 e = newIt.GetComponent<Enemy2>();
            if (e != null)
            {
                e.isIt = true;
                StartCoroutine(e.HandleSwitchDelay());
                StartCoroutine(e.StartCooldown());
            }
        }

        this.isIt = false;
        currentIt = null;

        // クールタイム
        yield return new WaitForSeconds(tagCooldown);
        isCooldown = false;
    }

    IEnumerator HandleSwitchDelay()
    {
        isSwitchingIt = true;
        yield return new WaitForSeconds(switchDelay);
        isSwitchingIt = false;
    }

    IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(tagCooldown);
        isCooldown = false;
    }

    void Wander()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 5f;
            randomPos.y = transform.position.y;
            agent.SetDestination(randomPos);
        }
    }
}
