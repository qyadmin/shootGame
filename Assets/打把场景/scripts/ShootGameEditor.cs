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

        //Editor.Selection.SelectionChanged += test;
        Editor.Selection.Selectioned += ToolsUIChange;
        
        Editor.Tools.LockAxes = new LockObject
        {
            PositionY = true,
            ScaleY = true,
            RotationX = true,
            RotationZ = true      
        };
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
        newArea.Instantiate_obj(Resources.Load<GameObject>("ShootingArea"), EditorUI._Instance.areaListUI.AreaPerfabUI, AreaCount, EditorUI._Instance.areaListUI.AreaUICount, delegate () { UIClickEvent(newArea); });
        newArea.ItemList = new ShootingArea.ShootingItemList();
        newArea.ItemList.AddItem(Resources.Load<GameObject>("纸靶子"));
        

        m_Arealist.Add(newArea);
    }

    void test(Object[] unselected)
    {
        if (unselected != null)
            for (int i = 0; i < unselected.Length; ++i)
            {
                GameObject unselectedObj = unselected[i] as GameObject;
                Debug.Log(unselectedObj.name);
            }
    }
    void ToolsUIChange(Object[] selected)
    {
        bool Postion = true;
        bool Rotation = true;
        bool Scale = true;

        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            Destroy(EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(0).gameObject);
        }

        if (selected != null)
        {
            if (selected.Length == 1)
                m_Arealist.ForEach(item => {
                    if (item.Perfab == selected[0] as GameObject)
                    {
                        item.ItemList.Instantiate_obj(EditorUI._Instance.itemListUI.Prefab, EditorUI._Instance.itemListUI.ItemListUICount.gameObject);
                    }
                });
              

            for (int i = 0; i < selected.Length; ++i)
            {
                GameObject selectedObj = selected[i] as GameObject;
                ExposeToEditor ObjEditor = selectedObj.GetComponent<ExposeToEditor>();
                if (ObjEditor)
                {
                    foreach (DisabledToolsType item in ObjEditor.DisabledToolsType)
                    {
                        if (item == DisabledToolsType.Position && Postion)
                            Postion = false;
                        if (item == DisabledToolsType.Roation && Rotation)
                            Rotation = false;
                        if (item == DisabledToolsType.Scale && Scale)
                            Scale = false;
                    }
                }
            }
        }
            
        EditorUI._Instance.runtimeToolUI.Move_Button.interactable = Postion;
        EditorUI._Instance.runtimeToolUI.Rotate_Button.interactable = Rotation;
        EditorUI._Instance.runtimeToolUI.Scale_Button.interactable = Scale;

        

    }

    void UIClickEvent(ShootingArea value)
    {
        //Editor.Tools.Reset();
        foreach (ShootingArea i in m_Arealist)
        {
            if (value == i)
            {
                List<Object> selection;

                selection = new List<Object>();

                selection.Insert(0, value.Perfab);

                Editor.Undo.Select(selection.ToArray(), value.Perfab);

                
                //Component.handle.LockObject.PositionY = true;
                
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
