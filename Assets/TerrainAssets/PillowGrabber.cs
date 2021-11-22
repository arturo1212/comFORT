using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowGrabber : MonoBehaviour
{
    // Start is called before the first frame update
    public bool agarrado_L;
    public bool agarrado_R;
    Rigidbody leRigidBody;
    MeshCollider leCollider;
    GameObject agarrador;
    void Start()
    {
        leRigidBody = GetComponent<Rigidbody>();
        leCollider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( agarrado_L && !FindObjectOfType<GestureDetector>().hand_left.agarrando)
        {
            leRigidBody.isKinematic = false;
            transform.parent = null;
            agarrado_L = false;
        }
        if (agarrado_R && !FindObjectOfType<GestureDetector>().hand_right.agarrando)
        {
            leRigidBody.isKinematic = false;
            transform.parent = null;
            agarrado_R = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name.Contains("Hand"))
        {
            agarrador = collision.transform.parent.parent.parent.gameObject;
            bool agarrandoRight = FindObjectOfType<GestureDetector>().hand_right.agarrando;
            bool agarrandoLeft = FindObjectOfType<GestureDetector>().hand_left.agarrando;
            if (agarrador.name.Contains("Left") && agarrandoLeft)
            {
                leRigidBody.isKinematic = true;
                transform.parent = agarrador.transform;
                agarrado_L = true;
            }
            if (agarrador.name.Contains("Right") && agarrandoRight)
            {
                leRigidBody.isKinematic = true;
                transform.parent = agarrador.transform;
                agarrado_R = true;
            }
        }
    }
}
