using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_AI : BaseEntity
{
    Rigidbody2D rb;
    public float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector = new Vector2(speed * Time.deltaTime, rb.velocity.y);
        rb.velocity = moveVector;
    }
}
