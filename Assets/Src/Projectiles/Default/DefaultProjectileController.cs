using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultProjectileController : MonoBehaviour
{

    public Vector3 target;
    public float speed = 0.05f;
    // Update is called once per frame
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
        if (transform.position == target) {
            Blow();
        }
    }

    private void Blow() {
        Destroy(this.gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col != null && col.gameObject != null && col.gameObject.tag == "EnemyTag")
        {
            Blow();
        }
    }
}
