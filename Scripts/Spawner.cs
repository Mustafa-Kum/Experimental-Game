using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    private List<Player> spawnedCharacters;

    private bool hasSpawned;

    public Collider collider;
    public UnityEvent onAllSpawnedCharacterEliminated;

    private void Awake()
    {
        var spawnPointArray = transform.parent.gameObject.GetComponentsInChildren<SpawnPoint>();
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
        spawnedCharacters = new List<Player>();
    }

    private void Update()
    {
        if (!hasSpawned || spawnedCharacters.Count == 0)
            return;

        bool allSpawnedAreDead = true;

        foreach (Player player in spawnedCharacters)
        {
            if (player.currentState != Player.PlayerState.Dead)
            {
                allSpawnedAreDead = false;

                break;
            }
        }

        if (allSpawnedAreDead)
        {
            if (onAllSpawnedCharacterEliminated != null)
                onAllSpawnedCharacterEliminated.Invoke();

            spawnedCharacters.Clear();
        }
    }

    public void SpawnCharacter()
    {
        if (hasSpawned)
            return;

        hasSpawned = true;

        foreach(SpawnPoint point in spawnPointList)
        {
            if (point.EnemyToSpawn != null)
            {
                GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);

                spawnedCharacters.Add(spawnedGameObject.GetComponent<Player>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnCharacter();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, collider.bounds.size);
    }
}
