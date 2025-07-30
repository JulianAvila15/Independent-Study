using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    Transform transformFireBall;
    float movementSpeed = 300;
    // Start is called before the first frame update
    void Start()
    {
        transformFireBall = gameObject.GetComponent<Transform>();
        StartCoroutine(DestroyFireBall());
    }

    // Update is called once per frame
    void Update()
    {
        transformFireBall.position += new Vector3(-movementSpeed * Time.deltaTime, 0, 0);
    }

    IEnumerator DestroyFireBall()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject.Destroy(gameObject);
    }
}
