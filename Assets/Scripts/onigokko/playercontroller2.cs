using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        if (move.sqrMagnitude > 0.01f)
        {
            transform.Translate(move.normalized * speed * Time.deltaTime, Space.World);
        }
    }
}
