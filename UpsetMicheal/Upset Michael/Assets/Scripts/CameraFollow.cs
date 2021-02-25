using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    public GameObject bg;
    public float camLift = 0.3f;
    public Transform carPos;
    [Range(0,1)]
    public float speed = 0.7f;
    void Start()
    {
        cam = Camera.main;   
    }

    // Update is called once per frame
    void Update()
    {
        if(carPos != null)
        {
            Vector2 alignVector = new Vector2(0, ((carPos.position.y - cam.transform.position.y) * speed) + camLift);
            cam.transform.Translate(alignVector);
            bg.transform.Translate(alignVector);
        }
    }
}
