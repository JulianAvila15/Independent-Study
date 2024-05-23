using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Transform transformCoin;
    float movementSpeed = 3;
    public ParticleSystem badCollision, goodCollision;
    bool hasCollided = false; // Flag to track if collision has occurred

    // Start is called before the first frame update
    void Start()
    {
        transformCoin = gameObject.GetComponent<Transform>();
        if (CollectingGameManager.coinsProduced >= 11)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transformCoin.position -= new Vector3(0, movementSpeed * Time.deltaTime, 0);

        if (transformCoin.position.y <= .5f)
        {
            if (!hasCollided) // Check if collision hasn't occurred already
            {
                Instantiate(badCollision, transformCoin.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCollided) // Check if collision hasn't occurred already
        {
            Instantiate(goodCollision, transformCoin.position, Quaternion.identity);
            CollectingGameManager.coinsCollected++;
            hasCollided = true; // Set the flag to true to prevent further collisions
            TimeManager.ResetAFKTimer();
            Destroy(gameObject);

        }
    }
}
