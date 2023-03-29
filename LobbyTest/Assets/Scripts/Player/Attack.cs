using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private GameController gc;
    
    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        if (go != null)
        {
            gc = go.GetComponent<GameController>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Other"))
        {
            //Rigidbody rb = collider.attachedRigidbody;
            Debug.Log("Hit enemy");
            Destroy(collider.gameObject);
            //gc.UpdateScore(10);
        }
    }
    
    
}
