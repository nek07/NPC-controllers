using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPSsManager : MonoBehaviour
{
    [Header("Player param")] 
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnRadius = 10f;


    [Header("Optimitization param")]
    [SerializeField] private float maxNpcDistance = 70f;
    [SerializeField] private bool deleteNonVisibleNpc = true;
    [SerializeField] private bool allowObjectPolling = true;
    [SerializeField] private float poolSize = 30f;
    
    
    [Header("NPC param")] 
    [SerializeField] private int maxNPC = 10;
    [SerializeField] private List<GameObject> npcPrefabs;
    private List<GameObject> npcList;
    
    [Header("InteractionPlaces")] 
    [SerializeField] private bool allowInteraction;

    private void Start()
    {
        npcList = new List<GameObject>();
        for (int i = 0; i < maxNPC; i++)
        {
            SpawnRandomNPC();
        }
    }

    private void Update()
    {
        RemoveFarNPCs();
        if (npcList.Count < maxNPC)
        {
            SpawnRandomNPC();
        }
        ObjectPolling();
    }

    void SpawnRandomNPC(Transform tr)
    {
        npcList.Add(Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)], tr.position, Quaternion.identity));
    }
    void SpawnRandomNPC()
    {
        // Получаем случайную позицию вокруг игрока в пределах радиуса
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += player.transform.position;
        randomDirection.y = player.transform.position.y; // Убедись, что высота остается одинаковой

        NavMeshHit hit;

        // Проверяем, есть ли точка на NavMesh поблизости
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            // Создаем NPC в этой точке
            GameObject newNPC = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)], hit.position, Quaternion.identity);

            if (allowObjectPolling) newNPC.SetActive(false);
            npcList.Add(newNPC);
        }
    }
    
    void RemoveFarNPCs()
    {
        for (int i = npcList.Count - 1; i >= 0; i--)
        {
            GameObject npc = npcList[i];
            float distance = Vector3.Distance(player.transform.position, npc.transform.position);

            if (distance > maxNpcDistance)
            {
                Destroy(npc); // Удаляем NPC
                npcList.RemoveAt(i); // Удаляем из списка
            }
        }
    }

    void ObjectPolling()
    {
        //abylai gau
        for (int i = npcList.Count - 1; i >= 0; i--)
        {
            GameObject npc = npcList[i];
            float distance = Vector3.Distance(player.transform.position, npc.transform.position);

            if (distance > poolSize)
            {
                npc.gameObject.SetActive(false);
            }
            else
            {
                npc.gameObject.SetActive(true);
            }
        }
    }
}
