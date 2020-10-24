using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLaserPointer2 : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    private GameObject leftLaser;
    private Material cubeMaterial;

    private bool leftHit;

    // Start is called before the first frame update
    void Start()
    {
        leftHit = false;
        leftLaser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftLaser.transform.localScale = new Vector3(0.01f, 0.01f, 10.00f);
        cubeMaterial = leftLaser.GetComponent<Renderer>().material;
        cubeMaterial.color = Color.magenta;
        leftLaser.transform.localRotation *= Quaternion.Euler(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        setLaser();
    }

    void setLaser()
    {
        leftLaser.transform.rotation = leftHand.localRotation;
        leftLaser.transform.position = leftHand.position;
        leftLaser.transform.localRotation *= Quaternion.Euler(90, 0, 0);
    }
}
