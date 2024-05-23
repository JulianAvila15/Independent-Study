using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spawner : MonoBehaviour
{
    public GameObject coin;
    public GameObject leftBound, rightBound,spawnPosition;
    private Transform leftBoundTransform, rightBoundTransform,spawnPositionTransform;
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        leftBoundTransform = leftBound.GetComponent<Transform>();
        rightBoundTransform = rightBound.GetComponent<Transform>();
        spawnPositionTransform = spawnPosition.GetComponent<Transform>();
        StartCoroutine(SpawnCoin());
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        if (started == true)
            Start();

    }
    IEnumerator SpawnCoin()
    {
        while (CollectingGameManager.coinsProduced<11)
        {
            var spawnedCoin = Instantiate(coin, new Vector3(Random.Range(leftBound.transform.position.x+.5f, rightBound.transform.position.x-.5f), spawnPositionTransform.transform.position.y, spawnPositionTransform.transform.position.z), Quaternion.identity);
            float scaleFactor = 0.4f; // Adjust this value as needed
            spawnedCoin.transform.localScale = Vector3.one * scaleFactor;
            CollectingGameManager.coinsProduced++;
            yield return new WaitForSeconds(3.0f);
        }
    }

   
}
