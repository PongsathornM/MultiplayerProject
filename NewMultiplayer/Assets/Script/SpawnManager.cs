using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private SpawnPoint[] _spawnpoints;

    private void Awake()
    {
        Instance = this;

        _spawnpoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnpoint()
    {
        return _spawnpoints[Random.Range(0, _spawnpoints.Length)].transform;
    }
}
