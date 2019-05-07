using Battlehub.RTHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemListUI : MonoBehaviour
{
    public GameObject Prefab;

    public Transform ItemListUICount;

    private void Start()
    {
        Prefab = Resources.Load<GameObject>("UI/PrefabSpawnPoint");
    }
}
