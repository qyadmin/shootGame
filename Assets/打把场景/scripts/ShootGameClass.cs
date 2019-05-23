using Battlehub.RTCommon;
using Battlehub.RTHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ItemType
{
    shootingPos,
    shootingPaperTargets,
    shootingMoveTarget,
    shootingSteelTarget,
    ambientTarget,
    EnvironmentTarget
}
public struct ShootingItem
{
    public ItemType Type;
    public General m_General;
    public action Event;
    private Sprite m_minImage;
    private GameObject m_prefab;
    private GameObject m_prefabUI;
    private int m_Number;
    private string m_Name;
    public bool CanThought;
    public bool ProhibitShooting;
    public bool InvalidItem;

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
    public GameObject PrefabUI
    {
        get
        {
            return m_prefabUI;
        }
        set
        {
            m_prefabUI = value;
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

    public void AddItem(GameObject prefab, Sprite sprite, Transform ItemFather)
    {       
        int num = 1;
        bool iscreat = false;

        while (!iscreat)
        {
            iscreat = true;
            if (m_ShootingItem.Count != 0)
                for (int i = 0; i < m_ShootingItem.Count; i++)
                {
                    if (m_ShootingItem[i].Number == num)
                    {
                        iscreat = false;
                        ++num;
                        break;
                    }
                }
        }
        ShootingItem newItem = new ShootingItem();
        newItem.Prefab = Instantiate(prefab);
        newItem.Prefab.name = prefab.name;
        newItem.Prefab.transform.position = Perfab.transform.position + new Vector3(0,0.05f,0);

        GameObject AreaIteamFather = new GameObject();
        AreaIteamFather.transform.parent = ItemFather;
        AreaIteamFather.transform.localPosition = Vector3.zero;
        AreaIteamFather.name = Perfab.name;
        newItem.Prefab.transform.parent = AreaIteamFather.transform;


        newItem.MinImage = sprite;
        newItem.Number = num;
        newItem.Name = newItem.Prefab.name;
        newItem.Type = ItemType.shootingPos;
        m_ShootingItem.Add(newItem);            
    }


    public void AddItem(GameObject prefab, Sprite sprite ,Transform ItemFather,ItemType type)
    {
        int num = 1;
        bool iscreat = false;

        while (!iscreat)
        {
            iscreat = true;
            if (m_ShootingItem.Count != 0)
                for (int i = 0; i < m_ShootingItem.Count; i++)
                {
                    if (m_ShootingItem[i].Number == num)
                    {
                        iscreat = false;
                        ++num;
                        break;
                    }
                }
        }
        ShootingItem newItem = new ShootingItem();
        newItem.Prefab = Instantiate(prefab);
        newItem.Prefab.name = prefab.name;
        newItem.Prefab.transform.position = Perfab.transform.position + new Vector3(0, 0.3f, 0);

        GameObject AreaIteamFather = new GameObject();
        AreaIteamFather.transform.parent = ItemFather;
        AreaIteamFather.transform.localPosition = Vector3.zero;
        AreaIteamFather.name = Perfab.name;
        newItem.Prefab.transform.parent = AreaIteamFather.transform;

        newItem.MinImage = sprite;
        newItem.Number = num;
        newItem.Name = newItem.Prefab.name;
        newItem.Type = type;
        newItem.CanThought = false;
        newItem.ProhibitShooting = false;
        newItem.InvalidItem = false;
        m_ShootingItem.Add(newItem);
    }

    public void Instantiate_Item(GameObject prefab, GameObject perfab_father, IRTE editor)
    {
        for (int i = 0; i < perfab_father.transform.childCount; i++)
        {
            Destroy(perfab_father.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < m_ShootingItem.Count; i++)
        {
            ShootingItem newItem = m_ShootingItem[i];
            GameObject prefabUI = Instantiate(prefab);
            prefabUI.name = "Area_Item_UI" + m_ShootingItem[i].Number;
            prefabUI.transform.Find("Text").gameObject.GetComponent<Text>().text = m_ShootingItem[i].Name;
            prefabUI.transform.Find("Image").gameObject.GetComponent<Image>().sprite = m_ShootingItem[i].MinImage;
            

            prefabUI.transform.parent = perfab_father.transform;
            prefabUI.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                List<UnityEngine.Object> selection;

                selection = new List<UnityEngine.Object>();

                selection.Insert(0, newItem.Prefab);

                editor.Undo.Select(selection.ToArray(), newItem.Prefab);

            });
            newItem.PrefabUI = prefabUI;
            if (newItem.Prefab.layer == LayerMask.NameToLayer("Item"))
            {
                newItem.PrefabUI.transform.Find("Delete").gameObject.GetComponent<Button>().interactable = true;
                newItem.PrefabUI.transform.Find("Delete").gameObject.GetComponent<Button>().onClick.AddListener(delegate () { DeleteItem(newItem); });
            }              
            m_ShootingItem[i] = newItem;
        }
       
    }

    void DeleteItem(ShootingItem item)
    {
        Destroy(item.PrefabUI.gameObject);
        Destroy(item.Prefab.gameObject);
        m_ShootingItem.Remove(item);
    }



    public General m_General;
    public ShootingItem m_ShootPos = new ShootingItem();

    private int m_areaTime = 0;
    private int m_areaShootNum = 0;


    public int AreaTime
    {
        get
        {
            return m_areaTime;
        }
        set
        {
            m_areaTime = value;
            m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().shootingTime = AreaTime;
        }
    }

    public int AreaShootNum
    {
        get
        {
            return m_areaShootNum;
        }
        set
        {
            m_areaShootNum = value;
            m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().effectiveShooting = AreaShootNum;
        }
    }
    public void getAreaMessage(out int areaTime,out int areaShootNum)
    {
        areaTime = AreaTime;
        areaShootNum = AreaShootNum;
    }

    public void setAreaMessage(int areaTine,int areaShootNum)
    {
        AreaTime = areaTine;
        AreaShootNum = areaShootNum;
    }

    public int m_Number;
    public GameObject Perfab;
    public GameObject PerfabUI;
    public ShootingItemList ItemList;
    public class ShootingItemList
    {
        public List<ShootingItem> m_ShootingItem = new List<ShootingItem>();

        public void Instantiate_obj(GameObject prefab, GameObject perfab_father,ItemType type)
        {
            for (int i = 0; i < perfab_father.transform.childCount; i++)
            {
                Destroy(perfab_father.transform.GetChild(i).gameObject);
            }
            m_ShootingItem.ForEach(item =>
            {
                if (item.Type == type)
                {
                    GameObject PrefabUI = Instantiate(prefab);
                    PrefabUI.name = item.Name;
                    PrefabUI.transform.parent = perfab_father.transform;
                    PrefabUI.GetComponent<PrefabSpawnPoint>().m_prefab = item.Prefab;
                    PrefabUI.GetComponent<PrefabSpawnPoint>().m_prefabNum = item.Number;
                    PrefabUI.GetComponent<PrefabSpawnPoint>().m_preview.sprite = item.MinImage;
                    PrefabUI.GetComponent<PrefabSpawnPoint>().OnStart();
                }               
            });
        }

        public void AddItem(GameObject prefab, Sprite sprite,ItemType type)
        {
            int num = 1;
            bool iscreat = false;

            while (!iscreat)
            {
                iscreat = true;
                if (m_ShootingItem.Count != 0)
                    for (int i = 0; i < m_ShootingItem.Count; i++)
                    {
                        if (m_ShootingItem[i].Number == num)
                        {
                            iscreat = false;
                            ++num;
                            break;
                        }
                    }
            }
            ShootingItem newItem = new ShootingItem();
            newItem.Prefab = prefab;
            newItem.MinImage = sprite;
            newItem.Number = num;
            newItem.Name = newItem.Prefab.name;
            newItem.CanThought = false;
            newItem.ProhibitShooting = false;
            newItem.InvalidItem = false;
            newItem.Type = type;
            m_ShootingItem.Add(newItem);
        }
        public void AddItem(GameObject prefab, Sprite sprite,bool CanThought)
        {
            int num = 1;
            bool iscreat = false;

            while (!iscreat)
            {
                iscreat = true;
                if (m_ShootingItem.Count != 0)
                    for (int i = 0; i < m_ShootingItem.Count; i++)
                    {
                        if (m_ShootingItem[i].Number == num)
                        {
                            iscreat = false;
                            ++num;
                            break;
                        }
                    }
            }
            ShootingItem newItem = new ShootingItem();
            newItem.Prefab = prefab;
            newItem.MinImage = sprite;
            newItem.Number = num;
            newItem.Name = newItem.Prefab.name;
            newItem.CanThought = CanThought;
            newItem.Type = ItemType.EnvironmentTarget;
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
        PerfabUI.transform.Find("Text").GetComponent<Text>().text = m_Number.ToString();
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
    public void Destroy_obj(GameObject Itemfather)
    {    
        Destroy(Perfab);
        Destroy(PerfabUI);
        for (int i = 0; i < Itemfather.transform.childCount; i++)
        {
            if (Itemfather.transform.GetChild(i).name == Perfab.name)
            {
                Destroy(Itemfather.transform.GetChild(i).gameObject);
                break;
            }                
        }
    }
}

public struct General
{
    public Vector3 position;
    public Quaternion rotation;
}
