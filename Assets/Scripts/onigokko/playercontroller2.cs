using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour
{
    public float speed = 5f;
    public bool isIt = false;
    public static PlayerController2 currentPlayerIt;

    private Rigidbody rb;
    private bool isSwitchingIt = false;  // 鬼交代中停止用
    private bool isCooldown = false;     // クールタイム用
    private float switchDelay = 2f;      // 鬼交代停止時間
    private float tagCooldown = 1f;      // 鬼交代後の触れたクールタイム

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (isIt) currentPlayerIt = this;
        UpdateColor();
    }

    void Update()
    {
        UpdateColor();
        if (!isIt && !isSwitchingIt) PlayerMove();
    }

    void UpdateColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = isIt ? Color.red : Color.white;
    }

    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized;
        if (move.sqrMagnitude > 0.01f)
            transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    public IEnumerator HandleSwitchDelay()
    {
        isSwitchingIt = true;
        yield return new WaitForSeconds(switchDelay);
        isSwitchingIt = false;
    }

    public IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(tagCooldown);
        isCooldown = false;
    }

    public bool CanBeTagged()
    {
        return !isSwitchingIt && !isCooldown;
    }
}
