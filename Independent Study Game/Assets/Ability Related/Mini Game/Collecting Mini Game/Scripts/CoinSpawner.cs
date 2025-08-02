using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CoinSpawner : MonoBehaviour
{
    public GameObject coin;
    public GameObject leftBound, rightBound,spawnArea;
    private Transform leftBoundTransform, rightBoundTransform,spawnPositionTransform;


  public  bool startedSpawning;
    public bool canStartSpawningInTutorial = false;//check if a certain step in the tutorial has been reached, namely catch coin
                                                   // Start is called before the first frame update

   public AbilityTutorialProgressiveDisclosureHandler abilityTutorialProgressiveDisclosureHandler;

    Vector3 coinSpawnPosition;
    private Vector3 pdFirstCoinSpawnPosition= new Vector3(-22.6000004f, 5.99856353f, -21.8999996f);
    private Vector3 pdSecondCoinSpawnPosition = new Vector3(-25.6000004f, 7, -21.8999996f);
    private Vector3 generalSpawnPosition;
    GameObject spawnedCoin;
    float scaleFactor = 0.4f; // Adjust this value as needed
   public CollectingGameManager collectingGameManager;

    public Player chefPlayerObj;
    private Vector3 spawnPosition;

    [SerializeField] private int maxCoinsNormalMode = 11;
[SerializeField] private int maxCoinsTutorialMode = 12;

[SerializeField] private float spawnEdgeBuffer = 0.5f;
    [SerializeField] private int firstCoinIndexThreshold = 1;
    [SerializeField] private float coinSpawnTimeInterval = 3.0f;

    void Start()
    {
        leftBoundTransform = leftBound.GetComponent<Transform>();
        rightBoundTransform = rightBound.GetComponent<Transform>();
        spawnPositionTransform = spawnArea.GetComponent<Transform>();


        generalSpawnPosition = new Vector3(0, spawnPositionTransform.transform.position.y, spawnPositionTransform.transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartSpawnCoroutine()
    {
     
        collectingGameManager.coinSpawnCoroutine = StartCoroutine(SpawnCoin());
        startedSpawning = true;
    }
    
    void StopSpawnCoin()
    {
        startedSpawning = false;

        if (collectingGameManager.coinSpawnCoroutine != null)
        {
            StopCoroutine(collectingGameManager.coinSpawnCoroutine);
            collectingGameManager.coinSpawnCoroutine = null;
        }
    }

  public  IEnumerator SpawnCoin()
    {

        if ((!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered||canStartSpawningInTutorial))
        { 

                while (CanSpawnCoin())
                {

                if (abilityTutorialProgressiveDisclosureHandler.miniGamePDHandler.InCollectingMiniGameTutorial())
                {
                    spawnPosition = generalSpawnPosition;
                    spawnPosition.x = GetRandomCoinXSpawnPosition();
                }
                else
                {
                    spawnPosition = (collectingGameManager.coinsProduced + 1 <= firstCoinIndexThreshold) ? pdFirstCoinSpawnPosition : pdSecondCoinSpawnPosition;
                }

                coinSpawnPosition = spawnPosition;

                if (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || spawnedCoin == null)
                {
                    spawnedCoin = Instantiate(coin, coinSpawnPosition, Quaternion.identity);
                    spawnedCoin.transform.localScale = Vector3.one * scaleFactor;
                    collectingGameManager.coinsProduced++;

                }
              

                yield return new WaitForSeconds(coinSpawnTimeInterval);
                }
            
        }
        else if(!canStartSpawningInTutorial)//if progressive disclosure tutorial is happening dont start immediately
        {
            yield return null;
        }
    }

    private void OnDisable()
    {
        startedSpawning = false;
    }


    bool CanSpawnCoin()
    {
        return (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && collectingGameManager.coinsProduced <= maxCoinsNormalMode) || (canStartSpawningInTutorial && collectingGameManager.coinsProduced < maxCoinsTutorialMode);
    }

    float GetRandomCoinXSpawnPosition()
    {
        return Random.Range(leftBound.transform.position.x + spawnEdgeBuffer, rightBound.transform.position.x - spawnEdgeBuffer);
    }
}
