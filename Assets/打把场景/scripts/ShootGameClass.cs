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
