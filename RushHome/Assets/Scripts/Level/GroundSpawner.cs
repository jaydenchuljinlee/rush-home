using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField] GameObject groundTilePrefab;
    [SerializeField] int tileCount = 8;
    [SerializeField] float tileLength = 20f;

    readonly List<GameObject> activeTiles = new();
    float spawnZ;
    Transform playerTransform;

    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().transform;
        spawnZ = -tileLength; // 플레이어 뒤쪽부터 시작

        for (int i = 0; i < tileCount; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Playing이 아니어도 타일 리사이클 (캐릭터가 직접 전진하므로)
        while (activeTiles.Count > 1 &&
               playerTransform.position.z > activeTiles[1].transform.position.z + tileLength)
        {
            RecycleTile();
        }
    }

    void SpawnTile()
    {
        GameObject tile = Instantiate(groundTilePrefab, new Vector3(0f, 0f, spawnZ), Quaternion.identity, transform);
        activeTiles.Add(tile);
        spawnZ += tileLength;
    }

    void RecycleTile()
    {
        GameObject oldTile = activeTiles[0];
        activeTiles.RemoveAt(0);
        oldTile.transform.position = new Vector3(0f, 0f, spawnZ);
        activeTiles.Add(oldTile);
        spawnZ += tileLength;
    }
}
