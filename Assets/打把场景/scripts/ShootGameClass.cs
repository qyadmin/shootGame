using Battlehub.RTHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ShootingItem
{
    public General m_General;
    public action Event;
    private Image m_minImage;
   
    private GameObject m_prefab;

    public GameObject Prefab
    {
        get
        {
            return m_prefab;
        }
        set
        {
            m_prefab = value;
        }
    }
}


public class ShootingArea : MonoBehaviour
{
    public List<ShootingItem> m_ShootingItem;
    public General m_General;
    public General m_ShootPos;
    public GameObject Perfab;
    public GameObject PerfabUI;
    public ShootingItemList ItemList;
    public class ShootingItemList
    {
        public List<ShootingItem> m_ShootingItem = new List<ShootingItem>();

        public void Instantiate_obj(GameObject prefab, GameObject perfab_father)
        {
            for (int i = 0; i < perfab_father.transform.childCount; i++)
            {
                Destroy(perfab_father.transform.GetChild(0).gameObject);
            }
            m_ShootingItem.ForEach(item => {
                GameObject newItem = Instantiate(prefab);
                newItem.transform.parent = perfab_father.transform;
                newItem.GetComponent<PrefabSpawnPoint>().m_prefab = item.Prefab;
                newItem.GetComponent<PrefabSpawnPoint>().OnStart();
            });
        }

        public void AddItem(GameObject prefab)
        {
            ShootingItem newItem = new ShootingItem();
            newItem.Prefab = prefab;
            m_ShootingItem.Add(newItem);
        }
    }

    public void Instantiate_obj(GameObject perfab, GameObject perfabUI, GameObject perfab_father, GameObject perfabUI_father, System.Action clickevent)
    {
        Perfab = Instantiate(perfab);
        Perfab.transform.parent = perfab_father.transform;
        Perfab.transform.localPosition = new Vector3(0, 0, 0);
        PerfabUI = Instantiate(perfabUI);
        PerfabUI.transform.parent = perfabUI_father.transform;
        PerfabUI.AddComponent<Button>().onClick.AddListener(delegate () { clickevent(); });
    }

    public void Instantiate_obj(GameObject perfab, GameObject perfabUI, GameObject perfab_father, GameObject perfabUI_father)
    {
        Perfab = Instantiate(perfab);
        Perfab.transform.parent = perfab_father.transform;
        PerfabUI = Instantiate(perfabUI);
        PerfabUI.transform.parent = perfabUI_father.transform;
    }
    public void Destroy_obj()
    {
        Destroy(Perfab);
        Destroy(PerfabUI);
    }
}

public class General
{
    public Vector3 position;
    public Quaternion rotation;
}
