using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    [Header("Assets")]
    public GameObject[] carPrefabs;
    public Transform[] spawnPoints;

    [Header("Settings")]
    public float moveSpeed = 10f;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;
    public float carLifeTime = 20f;

    private List<GameObject> activeCars = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        for(int i = activeCars.Count -1; i >= 0; i--)
        {
            GameObject car = activeCars[i];
            if(car == null)
            {
                activeCars.RemoveAt(i);
                continue;
            }
            car.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime , maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            SpawnCar();
        }
    }
    void SpawnCar()
    {
        if(carPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            return;
        }
        int carIndex = Random.Range(0 , carPrefabs.Length);
        int pointIndex = Random.Range(0 , spawnPoints.Length);
        Transform point = spawnPoints[pointIndex];
        GameObject selectedCar = carPrefabs[carIndex];
        GameObject newCar = Instantiate(selectedCar , point.position , point.rotation);
        activeCars.Add(newCar);
        Destroy(newCar , carLifeTime);
    }
}
