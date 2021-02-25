using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Vehicle_Controller))]
[AddComponentMenu("SimpleVehicle2D/Vehicle Init")]
public class Vehicle_Init : MonoBehaviour
{
    /*
    Requires that you have set up an object with hierarchy as follows:
        v CarObject (Attach this script to this object)
           - Any number of wheel anchor points parented:
           Anchor_0
           Anchor_1
           Anchor_2
           ...

    Will set up for you, using the built in WheelJoint2D component, as many wheels with spring suspension as there are anchors.
    */
    Rigidbody2D rb;
    public string car_Name;
    SpriteRenderer spriteRenderer;
    [Tooltip("Put all of your positional anchor transforms in here. Order doesn't really matter.")]
    public Transform[] anchors;
    [Tooltip("The mass of each wheel's rigidbody.")]
    public float wheelWeight;
    public Sprite wheelSprite;
    [Tooltip("How quickly the tires will move back to their desired position. \n\nDefault: 2")]
    public float suspensionStrength = 2f;
    [Tooltip("How bouncy you want the suspension to be.\n\n0 for dramatic\n1 for old people")]
    [Range(0.0f, 1.0f)]
    public float suspensionDampening = 0.7f;
    Vehicle_Controller _Controller;    
    void Start()
    {
        _Controller = transform.GetComponent<Vehicle_Controller>();
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(car_Name == "")
        {
            gameObject.name = spriteRenderer.sprite.name;
        }
        else
        {
            gameObject.name = car_Name;
        }
        spriteRenderer.sortingOrder = 5;
        if(transform.GetComponent<PolygonCollider2D>() == null)
        {
            gameObject.AddComponent(typeof(PolygonCollider2D));
        }
        
        int i = 0;
        foreach(Transform anchor in anchors)
        {
            GameObject newWheel = new GameObject("Wheel" + i.ToString(),typeof(CircleCollider2D),typeof(Rigidbody2D), typeof(SpriteRenderer));
            newWheel.transform.SetParent(transform);
            Rigidbody2D newWheelRB = newWheel.GetComponent<Rigidbody2D>();
            SpriteRenderer wheelRenderer = newWheel.GetComponent<SpriteRenderer>();

            wheelRenderer.sortingOrder = 4;

            newWheelRB.mass = wheelWeight;
            newWheel.GetComponent<SpriteRenderer>().sprite = wheelSprite;
            newWheel.transform.position = anchor.position;
            
            WheelJoint2D wheelJoint = gameObject.AddComponent(typeof(WheelJoint2D)) as WheelJoint2D;
            JointSuspension2D wheelSus = wheelJoint.suspension;
            wheelJoint.connectedBody = newWheelRB;
            wheelJoint.anchor = anchor.localPosition;
            wheelSus.frequency = suspensionStrength;
            wheelSus.dampingRatio = suspensionDampening;
            wheelJoint.suspension = wheelSus;

            //_Controller.Wheels[i] = newWheel;
            i++;
        }
        
    }
    
}
