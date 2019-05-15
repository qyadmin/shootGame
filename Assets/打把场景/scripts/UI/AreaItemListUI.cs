using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaItemListUI : MonoBehaviour
{
    [SerializeField]
    public GameObject AreaItemListUICount;
    [HideInInspector]
    public GameObject AreaItemListPrefabUI;

    public Transform LockUI;

    public Sprite ordinary,active;
    private void Start()
    {
        AreaItemListPrefabUI = Resources.Load<GameObject>("UI/AreaItemListPrefab");
    }
}
