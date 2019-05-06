using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles.Demo;
using Battlehub.RTCommon;
using System.Linq;

[DefaultExecutionOrder(-10)]
public class ShootGameEditor : SimpleEditor, IRTEState
{
    private List<ShootingArea> m_Arealist = new List<ShootingArea>();

    [SerializeField]
    private GameObject AreaPerfab, AreaPerfabUI;
    [SerializeField]
    private GameObject AreaCount, AreaUICount;


    [SerializeField]
    Button Add, Subtract;

    public bool IsCreated
    {
        get { return true; }
    }

    #region IRTEState implementation
    public event System.Action<object> Created;
    public event System.Action<object> Destroyed;
    private void Use()
    {
        Created(null);
        Destroyed(null);
    }
    #endregion

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected override void Awake()
    {
        base.Awake();
        Add.onClick.AddListener(delegate ()
        {
            Add_Arealist();
        });
        Subtract.onClick.AddListener(delegate ()
        {
            Subtract_Arealist(Editor.Selection.gameObjects);
        });

    }

    void Add_Arealist()
    {
        ShootingArea newArea = new ShootingArea();
        newArea.Instantiate_obj(AreaPerfab, AreaPerfabUI, AreaCount, AreaUICount, delegate () { UIClickEvent(newArea); });
        m_Arealist.Add(newArea);
    }

    void UIClickEvent(ShootingArea value)
    {
        foreach (ShootingArea i in m_Arealist)
        {
            if (value == i)
            {
                List<Object> selection;

                selection = new List<Object>();

                selection.Insert(0, value.Perfab);

                Editor.Undo.Select(selection.ToArray(), value.Perfab);
            }
        }
    }
    void Subtract_Arealist(GameObject[] area)
    {
        if (area.Length == 0)
            return;

        List<ShootingArea> m_Area = getShootingArea(area);

        foreach (ShootingArea i in m_Area)
        {
            i.Destroy_obj();
            m_Arealist.Remove(i);
        }
    }

    List<ShootingArea> getShootingArea(GameObject[] values)
    {
        List<ShootingArea> m_Area = new List<ShootingArea>();
        foreach (GameObject i in values)
        {
            foreach (ShootingArea j in m_Arealist)
            {
                if (j.Perfab == i)
                    m_Area.Add(j);
            }
        }

        return m_Area;
    }


    void refreshUI()
    {

    }
}
