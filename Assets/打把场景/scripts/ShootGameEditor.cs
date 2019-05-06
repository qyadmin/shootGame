using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles.Demo;
using Battlehub.RTCommon;
using System.Linq;
using Battlehub.RTHandles;

[DefaultExecutionOrder(-10)]
public class ShootGameEditor : SimpleEditor
{
    private List<ShootingArea> m_Arealist = new List<ShootingArea>();

    [SerializeField]
    RuntimeSceneComponent Component;

    [SerializeField]
    private GameObject AreaPerfab;
    [SerializeField]
    private GameObject AreaCount;


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
        EditorUI._Instance.areaListUI.Add.onClick.AddListener(delegate (){Add_Arealist();});
        EditorUI._Instance.areaListUI.Subtract.onClick.AddListener(delegate (){Subtract_Arealist(Editor.Selection.gameObjects);});
        EditorUI._Instance.runtimeToolUI.Move_Button.onClick.AddListener(delegate() { SetRuntimeTool(RuntimeTool.Move); });
        EditorUI._Instance.runtimeToolUI.Rotate_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Rotate); });
        EditorUI._Instance.runtimeToolUI.Scale_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Scale); });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected override void Awake()
    {
        base.Awake();
    }

    void Add_Arealist()
    {
        ShootingArea newArea = new ShootingArea();
        newArea.Instantiate_obj(AreaPerfab, EditorUI._Instance.areaListUI.AreaPerfabUI, AreaCount, EditorUI._Instance.areaListUI.AreaUICount, delegate () { UIClickEvent(newArea); });
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
                

                //Component.handle.LockObject.PositionY = true;
                //Editor.Selection.activeTransform.position;
                //Debug.Log(Editor.Tools.PivotRotation);
            }
        }
    }



    void SetRuntimeTool(RuntimeTool runtimeTool)
    {
        switch (runtimeTool)
        {
            case RuntimeTool.Move:
                Editor.Tools.Current = RuntimeTool.Move;               
                break;
            case RuntimeTool.Rotate:
                Editor.Tools.Current = RuntimeTool.Rotate;
                break;
            case RuntimeTool.Scale:
                Editor.Tools.Current = RuntimeTool.Scale;
                break;
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
