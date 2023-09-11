using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject attackDirection;
    [SerializeField] private float spawnTime = 1f;
    private bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnTime);
        Instantiate(arrow, attackDirection.transform.position,Quaternion.identity);
        canSpawn = true;
    }
}
