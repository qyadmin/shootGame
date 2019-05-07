using Battlehub.RTHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ShootingItem
{
    public General m_General;
    public action Event;
}

public class ShootingItemList
{
    private Image m_minImage;
    public Image MinImage
    {   
        get
        {
            return m_minImage;
        }
        set
        {
            m_minImage = value;
            PrefabComponet.m_preview = value;
        }
    }
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
            PrefabComponet.m_prefab = value;
        }
    }
    private Vector3 m_scale;
    public Vector3 Scale
    {
        get
        {
            return m_scale;
        }
        set
        {
            m_scale = value;
            PrefabComponet.m_prefabScale = value;
        }
    }
    public string Name;
    public PrefabSpawnPoint PrefabComponet = new PrefabSpawnPoint();
}

public class ShootingArea : MonoBehaviour
{
    public List<ShootingItem> m_ShootingItem;
    public General m_General;
    public General m_ShootPos;
    public GameObject Perfab;
    public GameObject PerfabUI;
    


    public void Instantiate_obj(GameObject perfab,GameObject perfabUI,GameObject perfab_father,GameObject perfabUI_father,System.Action clickevent)
    {
        Perfab = Instantiate(perfab);
        Perfab.transform.parent = perfab_father.transform;
        Perfab.transform.localPosition = new Vector3(0,0,0);
        PerfabUI = Instantiate(perfabUI);
        PerfabUI.transform.parent = perfabUI_father.transform;
        PerfabUI.AddComponent<Button>().onClick.AddListener(delegate() { clickevent(); } );
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
