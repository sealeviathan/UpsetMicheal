using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemMain : MonoBehaviour
{
    float amplitude = 1.5f;
    public int gasInc = 0;
    public int healthInc = 0;
    BoxCollider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        if(col == null)
        {
            gameObject.AddComponent(typeof(BoxCollider2D));
        }
        col.isTrigger = true;
        if(gameObject.tag != "Item")
        {
            gameObject.tag = "Item";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float curHeightAdd = Mathf.Sin(Time.time) * Time.deltaTime * amplitude;
        transform.position = new Vector2(transform.position.x, transform.position.y + curHeightAdd);
    }
}
