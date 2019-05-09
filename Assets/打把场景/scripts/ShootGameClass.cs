using Battlehub.RTCommon;
using Battlehub.RTHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public struct ShootingItem
{
    public General m_General;
    public action Event;
    private Sprite m_minImage;
    private GameObject m_prefab;
    private int m_Number;
    private string m_Name;
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

    public Sprite MinImage
    {
        get
        {
            return m_minImage;
        }
        set
        {
            m_minImage = value;
        }
    }

    public int Number
    {
        get
        {
            return m_Number;
        }
        set
        {
            m_Number = value;
        }
    }

    public string Name
    {
        get
        {
            return m_Name;
        }
        set
        {
            m_Name = value;
        }
    }

}


public class ShootingArea : MonoBehaviour
{
    public List<ShootingItem> m_ShootingItem = new List<ShootingItem>();

    public void Instantiate_Item(GameObject prefab, GameObject perfab_father, IRTE editor)
    {
        for (int i = 0; i < perfab_father.transform.childCount; i++)
        {
            Destroy(perfab_father.transform.GetChild(i).gameObject);
        }
        m_ShootingItem.ForEach(item =>
        {
            GameObject newItem = Instantiate(prefab);
            newItem.name = "Area_Item" + item.Number;
            newItem.transform.Find("Text").gameObject.GetComponent<Text>().text = item.Name;
            newItem.transform.Find("Image").gameObject.GetComponent<Image>().sprite = item.MinImage;
            newItem.transform.parent = perfab_father.transform;
            newItem.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                List<UnityEngine.Object> selection;

                selection = new List<UnityEngine.Object>();

                selection.Insert(0, item.Prefab);

                editor.Undo.Select(selection.ToArray(), item.Prefab);

            });
        });
    }

    public General m_General;
    public General m_ShootPos;
    public int m_Number;
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
                Destroy(perfab_father.transform.GetChild(i).gameObject);
            }
            m_ShootingItem.ForEach(item =>
            {                              
                GameObject newItem = Instantiate(prefab);
                newItem.name = item.Name;
                newItem.transform.parent = perfab_father.transform;
                newItem.GetComponent<PrefabSpawnPoint>().m_prefab = item.Prefab;
                newItem.GetComponent<PrefabSpawnPoint>().m_prefabNum = item.Number;
                newItem.GetComponent<PrefabSpawnPoint>().m_preview.sprite = item.MinImage;
                newItem.GetComponent<PrefabSpawnPoint>().OnStart();
            });
        }

        public void AddItem(GameObject prefab, Sprite sprite)
        {
            int num = 1;
            if (m_ShootingItem.Count != 0)
                m_ShootingItem.ForEach(item =>
                {
                    if (item.Number == num)
                        ++num;
                });
            ShootingItem newItem = new ShootingItem();
            newItem.Prefab = prefab;
            newItem.MinImage = sprite;
            newItem.Number = num;
            newItem.Name = newItem.Prefab.name;
            m_ShootingItem.Add(newItem);
        }
    }

    public void Instantiate_obj(GameObject perfab, GameObject perfabUI, GameObject perfab_father, GameObject perfabUI_father, System.Action clickevent, int Number)
    {
        Perfab = Instantiate(perfab);
        m_Number = Number;
        Perfab.transform.parent = perfab_father.transform;
        Perfab.transform.name = "Area" + Number;
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

public struct General
{
    public Vector3 position;
    public Quaternion rotation;
}
