using Battlehub.RTHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListUI : MonoBehaviour
{
    public GameObject Prefab;

    public Transform ItemListUICount;

    public Transform LockUI;

    public Button Paper, Steel, Move, Ambient, Environment;


    private void Start()
    {
        Prefab = Resources.Load<GameObject>("UI/PrefabSpawnPoint");
    }
}
