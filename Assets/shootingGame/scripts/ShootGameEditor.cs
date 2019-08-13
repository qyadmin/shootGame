using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles.Demo;
using Battlehub.RTCommon;
using System.Linq;
using Battlehub.RTHandles;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-10)]
public class ShootGameEditor : SimpleEditor
{
    public static ShootGameEditor _Instance;

    private List<ShootingArea> m_Arealist = new List<ShootingArea>();

    public FileManger filemanger = new FileManger();

    public delegate void ModulEvent();

    public event ModulEvent m_editorEvent;

    public event ModulEvent m_gameEvent;
    public List<ShootingArea> Arealist
    {
        get
        {
            return m_Arealist;
        }
    }

    Modul m_modul;
    public Modul modul
    {
        get { return m_modul; }
        set
        {
            m_modul = value;
            if (modul == Modul.Editor)
                m_editorEvent();
            if (modul == Modul.Game)
                m_gameEvent();
        }
    }

    [SerializeField]
    private GameObject AreaCount = null;
    [SerializeField]
    private GameObject AreaItemCount = null;
    bool m_Lock = false;

    public bool Lock
    {
        get { return m_Lock; }
        set
        {
            m_Lock = value;
            if (Lock)
                EditorUI._Instance.runtimeToolUI.Lock_Button.GetComponent<Image>().sprite = EditorUI._Instance.runtimeToolUI.UnLock;
            else
                EditorUI._Instance.runtimeToolUI.Lock_Button.GetComponent<Image>().sprite = EditorUI._Instance.runtimeToolUI.Lock;
        }
    }




    public bool IsCreated
    {
        get { return true; }
    }

    private GameObject EditorArea;

    public GameObject m_editorArea
    {
        get
        {
            return EditorArea;
        }
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



    void UIButtonAddEvent()
    {
        EditorUI._Instance.areaListUI.Add.onClick.AddListener(delegate () { Add_Arealist(); });
        EditorUI._Instance.areaListUI.Subtract.onClick.AddListener(delegate () { Subtract_Arealist(Editor.Selection.gameObjects); });
        EditorUI._Instance.runtimeToolUI.Move_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Move); });
        EditorUI._Instance.runtimeToolUI.Rotate_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Rotate); });
        EditorUI._Instance.runtimeToolUI.Scale_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Scale); });
        EditorUI._Instance.runtimeToolUI.Lock_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Lock); });
        EditorUI._Instance.runtimeToolUI.View_Button.onClick.AddListener(delegate () { OnPlayClick(); });
        EditorUI._Instance.gameFinishUI.View.onClick.AddListener(delegate () { OnPlayClick(); });
        EditorUI._Instance.runtimeToolUI.Save_Button.onClick.AddListener(delegate () { SaveFile(); });
        EditorUI._Instance.areaEditorUI.m_time.onEndEdit.AddListener(delegate (string value) { GetEditorArea().AreaTime = int.Parse(value); });
        EditorUI._Instance.areaEditorUI.m_shootNum.onEndEdit.AddListener(delegate (string value) { GetEditorArea().AreaShootNum = int.Parse(value); });
        EditorUI._Instance.itemListUI.Paper.onClick.AddListener(delegate () { ChoiceType(ItemType.shootingPaperTargets); });
        EditorUI._Instance.itemListUI.Steel.onClick.AddListener(delegate () { ChoiceType(ItemType.shootingSteelTarget); });
        EditorUI._Instance.itemListUI.Move.onClick.AddListener(delegate () { ChoiceType(ItemType.shootingMoveTarget); });
        EditorUI._Instance.itemListUI.Environment.onClick.AddListener(delegate () { ChoiceType(ItemType.EnvironmentTarget); });
        EditorUI._Instance.itemListUI.Ambient.onClick.AddListener(delegate () { ChoiceType(ItemType.ambientTarget); });
        EditorUI._Instance.publishTypeUI.PC_Button.onClick.AddListener(delegate() { setPCsave(); });
        EditorUI._Instance.publishTypeUI.Android_Button.onClick.AddListener(delegate () { setAndroidsave(); });
        EditorUI._Instance.itemEditorUI.CanThought.onValueChanged.AddListener(delegate (bool value)
        {
            ShootingItem item = getActiveItem(Editor.Selection.activeGameObject);
            item.CanThought = value;
            for (int i = 0; i < GetEditorArea().m_ShootingItem.Count; i++)
            {
                if (item.Prefab == GetEditorArea().m_ShootingItem[i].Prefab)
                {
                    GetEditorArea().m_ShootingItem[i] = item;
                    break;
                }
            }
        });
        EditorUI._Instance.itemEditorUI.ProhibitShooting.onValueChanged.AddListener(delegate (bool value)
        {
            ShootingItem item = getActiveItem(Editor.Selection.activeGameObject);
            item.ProhibitShooting = value;
            for (int i = 0; i < GetEditorArea().m_ShootingItem.Count; i++)
            {
                if (item.Prefab == GetEditorArea().m_ShootingItem[i].Prefab)
                {
                    GetEditorArea().m_ShootingItem[i] = item;
                    break;
                }
            }
        });
        EditorUI._Instance.itemEditorUI.InvalidItem.onValueChanged.AddListener(delegate (bool value)
        {
            ShootingItem item = getActiveItem(Editor.Selection.activeGameObject);
            item.InvalidItem = value;
            for (int i = 0; i < GetEditorArea().m_ShootingItem.Count; i++)
            {
                if (item.Prefab == GetEditorArea().m_ShootingItem[i].Prefab)
                {
                    GetEditorArea().m_ShootingItem[i] = item;
                    break;
                }
            }
        });
    }

    protected override void Awake()
    {
        base.Awake();
        _Instance = this;
       
#if UNITY_ANDROID && !UNITY_EDITOR
        Destroy(editorCamera.GetComponent<AmplifyOcclusionEffect>());
#endif
    }

    void AreaContalMoudle()
    {
        Editor.Mask = (1 << LayerMask.NameToLayer("Area"));
    }
    void ItemContalMoudle()
    {
        Editor.Mask = Editor.Mask = (1 << LayerMask.NameToLayer("ShootPos") | 1 << LayerMask.NameToLayer("Item"));
    }

    protected override void Start()
    {
        base.Start();

        UIButtonAddEvent();
        //Editor.Selection.SelectionChanged += test;
        Editor.Selection.Selectioned += refreshUI;
        m_editorEvent += EditorScene;
        m_gameEvent += GameScene;
        AreaContalMoudle();
        Set_LockItemList(Lock, null);
        Editor.Tools.LockAxes = new LockObject
        {
           // PositionY = true,
            ScaleY = true,
            //RotationX = true,
            //RotationZ = true,
            RotationFree = true,
            RotationScreen = true

        };
        Xml_ShootingItem._Instance.OnStart();
        LocalizationManager.GetInstance.SetValue();
        Debug.Log(Xml_ShootingItem._Instance.existXml);
        if (Xml_ShootingItem._Instance.existXml)
        {
            modul = Modul.Game;
            LoadGameXml();
        }
        else
            modul = Modul.Editor;

        //EditorUI._Instance.sceneNameUI.OnStart();
        //StartCoroutine(load(Xml_ShootingItem._Instance.path));
        Debug.Log(Application.persistentDataPath+"  "+getLastpath(Application.persistentDataPath));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Editor.Selection.Selectioned -= refreshUI;
        m_editorEvent -= EditorScene;
        m_gameEvent -= GameScene;
    }

    public bool getExistXml
    {
        get
        {
            return Xml_ShootingItem._Instance.existXml;
        }
    }

    //增加Area
    void Add_Arealist()
    {
        ShootingArea newArea = new ShootingArea();
        int num = 1;
        bool iscreat = false;

        while (!iscreat)
        {
            iscreat = true;
            if (m_Arealist.Count != 0)
                for (int i = 0; i < m_Arealist.Count; i++)
                {
                    if (m_Arealist[i].m_Number == num)
                    {
                        iscreat = false;
                        ++num;
                        break;
                    }
                }

        }

        newArea.Instantiate_obj(Resources.Load<GameObject>("ShootingArea")
                                , EditorUI._Instance.areaListUI.AreaPerfabUI
                                , getSceneSprit()
                                , AreaCount
                                , EditorUI._Instance.areaListUI.AreaUICount
                                , delegate () { UIClickEvent(newArea); }
                                , num);
        newArea.ItemList = new ShootingArea.ShootingItemList();
        //newArea.deleteItem += DeleteItemEvent;
        newArea.AddItem(Resources.Load<GameObject>("区域射击点"), getAreaItemListSprit("区域射击点"), AreaItemCount.transform,10001);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(红)大"), getShootingAreaItemListSprit("钢靶(红)大"), ItemType.shootingSteelTarget, true,10002);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(白)大"), getShootingAreaItemListSprit("钢靶(白)大"), ItemType.shootingSteelTarget, true,10003);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(蓝)大"), getShootingAreaItemListSprit("钢靶(蓝)大"), ItemType.shootingSteelTarget, true,10004);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(红)小"), getShootingAreaItemListSprit("钢靶(红)小"), ItemType.shootingSteelTarget, true,10005);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(白)小"), getShootingAreaItemListSprit("钢靶(白)小"), ItemType.shootingSteelTarget, true,10006);
        newArea.ItemList.AddItem(getShootingAreaItemList("钢靶(蓝)小"), getShootingAreaItemListSprit("钢靶(蓝)小"), ItemType.shootingSteelTarget, true,10007);


        newArea.ItemList.AddItem(getShootingAreaItemList("铁丝网"), getShootingAreaItemListSprit("铁丝网"), ItemType.ambientTarget,10008);
        newArea.ItemList.AddItem(getShootingAreaItemList("铁丝网(门)"), getShootingAreaItemListSprit("铁丝网(门)"), ItemType.ambientTarget, true,10009);
        newArea.ItemList.AddItem(getShootingAreaItemList("木围栏"), getShootingAreaItemListSprit("木围栏"), ItemType.ambientTarget,10010);
        newArea.ItemList.AddItem(getShootingAreaItemList("木围栏(门)"), getShootingAreaItemListSprit("木围栏(门)"), ItemType.ambientTarget, true,10011);
        newArea.ItemList.AddItem(getShootingAreaItemList("木围栏(窗)"), getShootingAreaItemListSprit("木围栏(窗)"), ItemType.ambientTarget,10012);

        newArea.ItemList.AddItem(getShootingAreaItemList("IDPA纸靶(白)"), getShootingAreaItemListSprit("IDPA纸靶(白)"), ItemType.shootingPaperTargets,10013);
        newArea.ItemList.AddItem(getShootingAreaItemList("IDPA纸靶(黑)"), getShootingAreaItemListSprit("IDPA纸靶(黑)"), ItemType.shootingPaperTargets,10014);
        newArea.ItemList.AddItem(getShootingAreaItemList("IDPA纸靶(黄)"), getShootingAreaItemListSprit("IDPA纸靶(黄)"), ItemType.shootingPaperTargets,10015);

        newArea.ItemList.AddItem(getShootingAreaItemList("IPSC纸靶(黑)"), getShootingAreaItemListSprit("IPSC纸靶(黑)"), ItemType.shootingPaperTargets,10016);
        newArea.ItemList.AddItem(getShootingAreaItemList("IPSC纸靶(白)"), getShootingAreaItemListSprit("IPSC纸靶(白)"), ItemType.shootingPaperTargets,10017);
        newArea.ItemList.AddItem(getShootingAreaItemList("IPSC纸靶(黄)"), getShootingAreaItemListSprit("IPSC纸靶(黄)"), ItemType.shootingPaperTargets,10018);

        newArea.ItemList.AddItem(getShootingAreaItemList("方形钢靶(白)"), getShootingAreaItemListSprit("方形钢靶(白)"), ItemType.shootingSteelTarget,10019);
        newArea.ItemList.AddItem(getShootingAreaItemList("方形钢靶(蓝)"), getShootingAreaItemListSprit("方形钢靶(蓝)"), ItemType.shootingSteelTarget,10020);
        newArea.ItemList.AddItem(getShootingAreaItemList("方形钢靶(红)"), getShootingAreaItemListSprit("方形钢靶(红)"), ItemType.shootingSteelTarget,10021);

        newArea.ItemList.AddItem(getShootingAreaItemList("圆形钢靶(白)"), getShootingAreaItemListSprit("圆形钢靶(白)"), ItemType.shootingSteelTarget,10022);
        newArea.ItemList.AddItem(getShootingAreaItemList("圆形钢靶(蓝)"), getShootingAreaItemListSprit("圆形钢靶(蓝)"), ItemType.shootingSteelTarget,10023);
        newArea.ItemList.AddItem(getShootingAreaItemList("圆形钢靶(红)"), getShootingAreaItemListSprit("圆形钢靶(红)"), ItemType.shootingSteelTarget,10024);

        newArea.ItemList.AddItem(getShootingAreaItemList("滑道移动靶IDPA(单)"), getShootingAreaItemListSprit("滑道移动靶(单)"), ItemType.shootingMoveTarget,10025);
        newArea.ItemList.AddItem(getShootingAreaItemList("滑道移动靶IDPA(双)"), getShootingAreaItemListSprit("滑道移动靶(双)"), ItemType.shootingMoveTarget,10026);
        newArea.ItemList.AddItem(getShootingAreaItemList("滑道移动靶IPSC(单)"), getShootingAreaItemListSprit("滑道移动靶(单)"), ItemType.shootingMoveTarget,10027);
        newArea.ItemList.AddItem(getShootingAreaItemList("滑道移动靶IPSC(双)"), getShootingAreaItemListSprit("滑道移动靶(双)"), ItemType.shootingMoveTarget,10028);
        newArea.ItemList.AddItem(getShootingAreaItemList("旋转靶(单)"), getShootingAreaItemListSprit("旋转靶(单)"), ItemType.shootingSteelTarget,10029);
        newArea.ItemList.AddItem(getShootingAreaItemList("旋转靶(双)"), getShootingAreaItemListSprit("旋转靶(双)"), ItemType.shootingSteelTarget,10030);
        newArea.ItemList.AddItem(getShootingAreaItemList("左右移动靶(白)"), getShootingAreaItemListSprit("左右移动靶(白)"), ItemType.shootingMoveTarget,10031);
        newArea.ItemList.AddItem(getShootingAreaItemList("左右移动靶(黄)"), getShootingAreaItemListSprit("左右移动靶(黄)"), ItemType.shootingMoveTarget,10032);
        newArea.ItemList.AddItem(getShootingAreaItemList("左右移动靶(黑)"), getShootingAreaItemListSprit("左右移动靶(黑)"), ItemType.shootingMoveTarget,10033);
        newArea.ItemList.AddItem(getShootingAreaItemList("上下移动靶(白)"), getShootingAreaItemListSprit("上下移动靶(白)"), ItemType.shootingMoveTarget,10034);
        newArea.ItemList.AddItem(getShootingAreaItemList("上下移动靶(黄)"), getShootingAreaItemListSprit("上下移动靶(黄)"), ItemType.shootingMoveTarget,10035);
        newArea.ItemList.AddItem(getShootingAreaItemList("上下移动靶(黑)"), getShootingAreaItemListSprit("上下移动靶(黑)"), ItemType.shootingMoveTarget,10036);

        newArea.ItemList.AddItem(getShootingAreaItemList("西瓜"), getShootingAreaItemListSprit("西瓜"), ItemType.EnvironmentTarget,10037);
        newArea.ItemList.AddItem(getShootingAreaItemList("气球"), getShootingAreaItemListSprit("气球"), ItemType.EnvironmentTarget,10038);
        newArea.ItemList.AddItem(getShootingAreaItemList("可乐瓶"), getShootingAreaItemListSprit("可乐瓶"), ItemType.EnvironmentTarget,10039);
        newArea.ItemList.AddItem(getShootingAreaItemList("油桶"), getShootingAreaItemListSprit("油桶"), ItemType.EnvironmentTarget,10040);
        newArea.ItemList.AddItem(getShootingAreaItemList("轮胎"), getShootingAreaItemListSprit("轮胎"), ItemType.EnvironmentTarget,10041);
        newArea.ItemList.AddItem(getShootingAreaItemList("燃烧瓶"), getShootingAreaItemListSprit("燃烧瓶"), ItemType.EnvironmentTarget,10042);
        newArea.deleteItem += DeleteItemEvent;
        m_Arealist.Add(newArea);

        List<Object> selection;
        selection = new List<Object>();
        selection.Insert(0, newArea.Perfab);
        Editor.Undo.Select(selection.ToArray(), newArea.Perfab);

        Associated();
    }

    GameObject getShootingAreaItemList(string name)
    {
        return Resources.Load<GameObject>(name);
    }
    Sprite getShootingAreaItemListSprit(string name)
    {
        return Resources.Load<Sprite>("UI/Sprite/large/" + name);
    }
    Sprite getAreaItemListSprit(string name)
    {
        return Resources.Load<Sprite>("UI/Sprite/small/" + name);
    }

    Sprite getSceneSprit()
    {
        if (Static.Instance.sceneType == SceneType.OutSide)
            return Resources.Load<Sprite>("UI/Sprite/" + "OutSide");
        if (Static.Instance.sceneType == SceneType.InSide)
            return Resources.Load<Sprite>("UI/Sprite/" + "InSide");

        return null;
    }

    //增加Area的Item
    public void Add_AreaItemList(GameObject prefabInstance, int number)
    {
        GetEditorArea().ItemList.m_ShootingItem.ForEach(item =>
        {
            if (item.Number == number)
            {
                ShootingItem newItem = new ShootingItem();
                newItem = item;
                newItem.MinImage = getAreaItemListSprit(newItem.MinImage.name);
                newItem.Prefab = prefabInstance;
                newItem.Prefab.transform.parent = AreaItemCount.transform.Find(GetEditorArea().Perfab.name);
                newItem.m_General.position = newItem.Prefab.transform.localPosition;
                newItem.m_General.rotation = newItem.Prefab.transform.eulerAngles;
                newItem.m_General.scale = newItem.Prefab.transform.localScale;

                GetEditorArea().m_ShootingItem.Add(newItem);
                Debug.Log(item.Name);
            }
        });
    }
    //获取锁定Area的GameObject类型
    GameObject[] GetEditorAreaObj()
    {
        GameObject[] selection = new GameObject[1];
        selection[0] = EditorArea;
        return selection;
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


    void DeleteItemEvent(int itemnum)
    {
        if (GetEditorArea().m_ShootingItem.Count > itemnum && GetEditorArea().m_ShootingItem[itemnum].Prefab == Editor.Selection.activeGameObject)
        {
            Editor.Undo.Select(null, null);

        }
        else
        {
            refreshUI(Editor.Selection.objects);

        }
            
    }
    //监听选中物体的的UI变化
    void refreshUI(Object[] selected)
    {
        bool Postion = true;
        bool Rotation = true;
        bool Scale = true;
        bool Locks = true;
        if (!Lock)
            CleanItemList();

        EditorUI._Instance.itemEditorUI.gameObject.SetActive(false);

        if ((selected != null && selected.Length != 0) || EditorArea != null)
        {
            GameObject[] m_selected = new GameObject[1];

            if (EditorArea != null)
                m_selected[0] = EditorArea;
            else
                m_selected[0] = selected[0] as GameObject;
            if (m_selected[0].layer == LayerMask.NameToLayer("Area"))
            {

                m_Arealist.ForEach(item =>
                {
                    if (item.Perfab == m_selected[0])
                    {
                        item.ItemList.Instantiate_obj(EditorUI._Instance.itemListUI.Prefab, EditorUI._Instance.itemListUI.ItemListUICount.gameObject, itemListType);
                        item.Instantiate_Item(EditorUI._Instance.areaItemListUI.AreaItemListPrefabUI, EditorUI._Instance.areaItemListUI.AreaItemListUICount, Editor);

                        int areaTime;
                        int areaShootNum;
                        item.getAreaMessage(out areaTime, out areaShootNum);
                        EditorUI._Instance.areaEditorUI.gameObject.SetActive(true);
                        if (Lock)
                            EditorUI._Instance.areaEditorUI.enable();
                        else
                            EditorUI._Instance.areaEditorUI.disable();
                        EditorUI._Instance.areaEditorUI.setvalue(areaTime, areaShootNum);
                    }
                });
            }
            else
                Locks = false;



            if (selected != null)
            {
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

                    if (selectedObj.layer == LayerMask.NameToLayer("Item") || selectedObj.layer == LayerMask.NameToLayer("ShootPos"))
                    {
                        EditorUI._Instance.itemEditorUI.gameObject.SetActive(true);
                        ShootingItem item = getActiveItem(selectedObj);
                        EditorUI._Instance.itemEditorUI.ItemName.text = item.Name;

                        if (item.CanLink)
                        {
                            EditorUI._Instance.itemEditorUI.LinkList.onValueChanged.RemoveAllListeners();
                            EditorUI._Instance.itemEditorUI.LinkList.gameObject.SetActive(true);
                            EditorUI._Instance.itemEditorUI.LinkList.options.Clear();
                            EditorUI._Instance.itemEditorUI.LinkItem.gameObject.SetActive(true);
                            Dropdown.OptionData nullData = new Dropdown.OptionData();
                            nullData.text = LocalizationManager.GetInstance.GetValue("10000");

                            EditorUI._Instance.itemEditorUI.LinkList.options.Add(nullData);

                            int num = 0;
                            List<int> disableGroup = new List<int>();
                            GetLinkList(item).ForEach(obj =>
                            {
                                Dropdown.OptionData temoData;
                                temoData = new Dropdown.OptionData();
                                temoData.text = obj.Name;                                

                                EditorUI._Instance.itemEditorUI.LinkList.options.Add(temoData);
                                if (item.LinkageItem == obj.Prefab.transform)
                                {
                                    EditorUI._Instance.itemEditorUI.LinkList.value = num + 1;
                                    EditorUI._Instance.itemEditorUI.LinkItem.sprite = obj.MinImage;


                                    EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();


                                    EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.AddListener(delegate ()
                                    {
                                        GameObject.Find("SceneComponent").GetComponent<RuntimeSceneComponent>().Focus(obj.Prefab.transform);
                                    });
                                }
                                else
                                {
                                    Debug.Log(obj.IsLink);
                                    if(obj.IsLink)
                                    disableGroup.Add(num+1);
                                }

                                num++;
                            });
                            EditorUI._Instance.itemEditorUI.LinkList.gameObject.GetComponent<DRDropDown>().disables = disableGroup;
                            //foreach (int child in disableGroup)
                            //{
                            //    Debug.Log(child);
                            //    EditorUI._Instance.itemEditorUI.LinkList.template.GetComponent<ScrollRect>().content.transform.GetChild(child).GetComponent<Toggle>().interactable = false;
                            //}

                            if (!item.LinkageItem)
                            {
                                EditorUI._Instance.itemEditorUI.LinkList.value = 0;
                                EditorUI._Instance.itemEditorUI.LinkItem.sprite = getAreaItemListSprit("无");
                                EditorUI._Instance.itemEditorUI.LinkList.captionText.text = LocalizationManager.GetInstance.GetValue("10000");
                                EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();

                            }



                            EditorUI._Instance.itemEditorUI.LinkList.onValueChanged.AddListener(delegate (int id)
                            {
                                LinkListEvent(item, id);
                            });
                        }
                        else
                        {
                            EditorUI._Instance.itemEditorUI.LinkList.gameObject.SetActive(false);
                            EditorUI._Instance.itemEditorUI.LinkItem.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                Postion = false;
                Rotation = false;
                Scale = false;

            }

        }
        else
        {
            Locks = false;
            EditorUI._Instance.areaEditorUI.gameObject.SetActive(false);
        }
        EditorUI._Instance.runtimeToolUI.Move_Button.interactable = Postion;
        EditorUI._Instance.runtimeToolUI.Rotate_Button.interactable = Rotation;
        EditorUI._Instance.runtimeToolUI.Scale_Button.interactable = Scale;
        EditorUI._Instance.runtimeToolUI.Lock_Button.interactable = Locks;

        if (selected == null || selected.Length == 0)
        {
            EditorUI._Instance.runtimeToolUI.Move_Button.interactable = false;
            EditorUI._Instance.runtimeToolUI.Rotate_Button.interactable = false;
            EditorUI._Instance.runtimeToolUI.Scale_Button.interactable = false;
        }

        Set_LockItemList(Lock, selected);
    }

    //清除itemList和AreaItemList
    void CleanItemList()
    {
        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            Destroy(EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.childCount; i++)
        {
            Destroy(EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).gameObject);
        }
    }
    //设置锁定状态下itemList是否可拖动
    void Set_LockItemList(bool value, Object[] selected)
    {
        EditorUI._Instance.itemListUI.LockUI.gameObject.SetActive(!value);

        EditorUI._Instance.areaListUI.LockUI.gameObject.SetActive(value);

        EditorUI._Instance.areaItemListUI.LockUI.gameObject.SetActive(!value);

        m_Arealist.ForEach(item =>
        {
            item.Perfab.GetComponent<MeshRenderer>().enabled = false;
        });

        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(i).GetComponent<PrefabSpawnPoint>().CanDrag = value;

        }
        for (int i = 0; i < EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = !value;
            EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.GetChild(i).transform.Find("Sprite").GetComponent<Image>().sprite =
                EditorUI._Instance.areaListUI.ordinary;
        }
        for (int i = 0; i < EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = value;
            EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).transform.Find("Sprite").GetComponent<Image>().sprite =
                EditorUI._Instance.areaItemListUI.ordinary;
        }
        if (m_Arealist.Count < 6)
            EditorUI._Instance.areaListUI.Add.interactable = true;
        else
            EditorUI._Instance.areaListUI.Add.interactable = false;
        EditorUI._Instance.areaListUI.Subtract.interactable = false;

        if (EditorArea)
        {
            GetEditorArea().PerfabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaListUI.active;
            GetEditorArea().Perfab.GetComponent<MeshRenderer>().enabled = true;
        }


        if (selected != null && selected.Length > 0)
        {
            if ((selected[0] as GameObject).layer == LayerMask.NameToLayer("Area"))
            {
                GetActiveArea(selected[0] as GameObject).PerfabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaListUI.active;
                GetActiveArea(selected[0] as GameObject).Perfab.GetComponent<MeshRenderer>().enabled = true;
                EditorUI._Instance.areaListUI.Subtract.interactable = true;
            }
            if ((selected[0] as GameObject).layer == LayerMask.NameToLayer("Item") || (selected[0] as GameObject).layer == LayerMask.NameToLayer("ShootPos"))
            {       
                if(getActiveItem(selected[0] as GameObject).PrefabUI)
                getActiveItem(selected[0] as GameObject).PrefabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaItemListUI.active;
            }
        }
    }
    //ShootingArea列表的点击事件
    void UIClickEvent(ShootingArea value)
    {
        List<Object> selection;

        selection = new List<Object>();

        selection.Insert(0, value.Perfab);

        Editor.Undo.Select(selection.ToArray(), value.Perfab);
    }

    ItemType itemListType = ItemType.shootingPaperTargets;

    ItemType m_itemListType
    {
        get { return itemListType; }
        set
        {
            itemListType = value;
            refreshUI(Editor.Selection.objects);
        }
    }
    void ChoiceType(ItemType type)
    {
        m_itemListType = type;
    }


    //设置Tools种类
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
            case RuntimeTool.Lock:
                if (!Lock)
                    LockAcitiveArea();
                else
                    UnLockAcitiveArea();
                break;
        }
    }

    //list换位排序
    public void ShootAreaSorting()
    {
        List<ShootingArea> newlist = new List<ShootingArea>();

        foreach (Transform i in EditorUI._Instance.areaListUI.AreaUICount.transform)
        {
            m_Arealist.ForEach(item =>
            {
                if (i.gameObject == item.PerfabUI)
                    newlist.Add(item);
            });
        }

        m_Arealist = newlist;

        m_Arealist.ForEach(item =>
        {
            Debug.Log(item.m_Number);
        });
    }

    publishType m_publishType;

    IEnumerator Save()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        string filePath = OpenDialog.SaveDialog(LocalizationManager.GetInstance.GetValue("20002"));
        if (filePath != "")
        {
            if (FileManger.TestChineseDirectory(filePath))
            {
                OpenDialog.ShowDialog(LocalizationManager.GetInstance.GetValue("20003"), LocalizationManager.GetInstance.GetValue("20004"));
            }
            else
            {
                filemanger.Compiling(filePath,m_publishType);
                Debug.Log(filePath);
                
                string pathurl = null;
                if (m_publishType == publishType.PC)
                    pathurl = filePath + "/shooting_Data" + "/ShootingItem" + ".xml";
                if (m_publishType == publishType.Android)
                    pathurl = filePath + "/Android/data/com.qy.shootgame/files" + "/ShootingItem" + ".xml";
                Xml_ShootingItem._Instance.DeleteXmlByPath(pathurl);
                Xml_ShootingItem._Instance.CreateXml(pathurl);
                int id = 0;
                foreach (ShootingArea sa in m_Arealist)
                {
                    yield return new WaitForSeconds(0.5f);
                    ++id;
                    Xml_ShootingItem._Instance.AddXmlData(sa, id, pathurl);
                }
                Xml_ShootingItem._Instance.addSceneid(Static.Instance.sceneType.ToString(), pathurl);
                Xml_ShootingItem._Instance.addSceneName(EditorUI._Instance.publishTypeUI.EditorSenceName.text,pathurl);

                //if (m_publishType == publishType.Android)
                //{
                //    ZipManager._Instance.CreateZip(Application.dataPath +"/Android", Application.dataPath + "/shootgame.zip");
                //    ZipManager._Instance.ChangeExtension(Application.dataPath + "/shootgame.zip", Path.ChangeExtension(Application.dataPath + "/shootgame.zip", ".apk"));
                //}
                OpenDialog.ShowDialog(LocalizationManager.GetInstance.GetValue("20001")+"!", LocalizationManager.GetInstance.GetValue("20001"));
                EditorUI._Instance.publishTypeUI.Clear();
            }
        }
#else
        yield return null;
#endif
    }


    string getLastpath(string path)
    {
        string[] arr = path.Split('/'); // 以','字符对字符串进行分割，返回字符串数组
        List<string> list = new List<string>(arr);
        list.RemoveAt(list.Count-1);

        arr = list.ToArray();
        return string.Join("/", arr);
    }

    void setpublish()
    {
        EditorUI._Instance.publishTypeUI.gameObject.SetActive(true);
    }

    void setPCsave()
    {
        if (EditorUI._Instance.publishTypeUI.EditorSenceName.text == string.Empty)
        {
            EditorUI._Instance.worningUI.Type = worningType.msg;
            EditorUI._Instance.worningUI.tital.text = LocalizationManager.GetInstance.GetValue("20005");
            EditorUI._Instance.worningUI.msg.text = LocalizationManager.GetInstance.GetValue("20006");
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
            return;
        }
        m_publishType = publishType.PC;
        StartCoroutine(Save());
        EditorUI._Instance.publishTypeUI.gameObject.SetActive(false);
    }

    void setAndroidsave()
    {
        if (EditorUI._Instance.publishTypeUI.EditorSenceName.text == string.Empty)
        {
            EditorUI._Instance.worningUI.Type = worningType.msg;
            EditorUI._Instance.worningUI.tital.text = LocalizationManager.GetInstance.GetValue("20005");
            EditorUI._Instance.worningUI.msg.text = LocalizationManager.GetInstance.GetValue("20006");
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
            return;
        }
        m_publishType = publishType.Android;
        StartCoroutine(Save());
        EditorUI._Instance.publishTypeUI.gameObject.SetActive(false);
    }

    void SaveFile()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (m_Arealist.Count == 0)
        {
            EditorUI._Instance.worningUI.Type = worningType.msg;
            EditorUI._Instance.worningUI.tital.text = LocalizationManager.GetInstance.GetValue("20007");
            EditorUI._Instance.worningUI.msg.text = LocalizationManager.GetInstance.GetValue("20008");
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
            return;
        }
        setpublish();

#endif
    }

    void LoadGameXml()
    {
        int id = 0;

        foreach (XElement sa in Xml_ShootingItem._Instance.getAllXmlData())
        {
            ++id;

            ArrayList arrayList = Xml_ShootingItem._Instance.GetXmlData(id.ToString());

            string s = arrayList[0].ToString();

            using (StringReader reader = new StringReader(s))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GetDate));
                GetDate ItemList = (GetDate)serializer.Deserialize(reader);

                ShootingArea area = new ShootingArea();
                area.setData(ItemList);
                m_Arealist.Add(area);
            }
        }
        if (Xml_ShootingItem._Instance.GetSceneId() == SceneType.InSide.ToString())
            Static.Instance.sceneType = SceneType.InSide;
        if (Xml_ShootingItem._Instance.GetSceneId() == SceneType.OutSide.ToString())
            Static.Instance.sceneType = SceneType.OutSide;

        

        OnGameStart();
    }

    //锁定Area
    void LockAcitiveArea()
    {
        GameObject[] selected = Editor.Selection.gameObjects;
        Lock = true;
        if (selected != null)
        {
            for (int i = 0; i < selected.Length; ++i)
            {
                GameObject selectedObj = selected[i];
                EditorArea = selectedObj;
                //Debug.Log(selectedObj.name);
                //ExposeToEditor exposeToEditor = EditorArea.GetComponent<ExposeToEditor>();
                //SelectionGizmo selectionGizmo = EditorArea.AddComponent<SelectionGizmo>();

                //if (exposeToEditor.Selected != null)
                //{
                //    exposeToEditor.Selected.Invoke(exposeToEditor);
                //}
            }
        }
        //Editor.Selection.activeObject = null;

        Editor.Undo.Select(null, null);

        Editor.Tools.Current = RuntimeTool.Move;

        ItemContalMoudle();
    }

    //解锁Area
    void UnLockAcitiveArea()
    {
        Lock = false;
        List<Object> selection;

        selection = new List<Object>();

        selection.Insert(0, EditorArea);

        Editor.Undo.Select(selection.ToArray(), EditorArea);

        AreaContalMoudle();

        //Set_LockItemList(false);

        Editor.Tools.Current = RuntimeTool.Move;
        EditorArea = null;
    }

    //ShootAreaList选定删除
    void Subtract_Arealist(GameObject[] area)
    {
        if (area.Length == 0)
            return;

        List<int> m_Area = getShootingArea(area);

        foreach (int i in m_Area)
        {
            m_Arealist[i].Destroy_obj(AreaItemCount);
            m_Arealist[i].deleteItem -= DeleteItemEvent;
            m_Arealist.RemoveAt(i);
        }
    }
    //获取参数中物体在ShootAreaList中的标记
    List<int> getShootingArea(GameObject[] values)
    {
        List<int> m_Area = new List<int>();
        for (int i = 0; i < m_Arealist.Count; i++)
        {
            foreach (GameObject j in values)
            {
                if (m_Arealist[i].Perfab.name == j.name)
                {
                    m_Area.Add(i);
                }
            }
        }
        return m_Area;
    }

    int getShootingArea(GameObject values)
    {
        int m_Area = new int();
        for (int i = 0; i < m_Arealist.Count; i++)
        {

            if (m_Arealist[i].Perfab.name == values.name)
            {
                m_Area = i;
                break;
            }

        }
        return m_Area;
    }
    //获取锁定Area的ShootingArea属性结构
    public ShootingArea GetEditorArea()
    {
        return m_Arealist[getShootingArea(GetEditorAreaObj())[0]];
    }

    public ShootingArea GetActiveArea(GameObject value)
    {
        return m_Arealist[getShootingArea(value)];
    }


    //获取联动列表
    private List<ShootingItem> GetLinkList(ShootingItem shootingitem)
    {
        List<ShootingItem> linklist = new List<ShootingItem>();

        if (shootingitem.CanLink)
        {
            GetEditorArea().m_ShootingItem.ForEach(item =>
            {

                
                if (item.Type == ItemType.shootingMoveTarget)
                {
                    ShootingItem newitem = item;
                    linklist.Add(newitem);
                }
                //if (item.Type == ItemType.shootingMoveTarget && (shootingitem.LinkageItem && shootingitem.LinkageItem.gameObject == item.Prefab))
                //{
                //    ShootingItem newitem = item;
                //    linklist.Add(newitem);
                //}

            });
        }

        return linklist;
    }


    //联动Event
    private void LinkListEvent(ShootingItem item, int id)
    {
        for (int i = 0; i < GetEditorArea().m_ShootingItem.Count; i++)
        {
            if (item.LinkageItem)
                if (GetEditorArea().m_ShootingItem[i].Prefab == item.LinkageItem.gameObject)
                {
                    ShootingItem linkers = GetEditorArea().m_ShootingItem[i];
                    linkers.IsLink = false;
                    GetEditorArea().m_ShootingItem[i] = linkers;
                }
            if (id != 0)
                if (GetEditorArea().m_ShootingItem[i].Prefab == GetLinkList(item)[id - 1].Prefab)
                {
                    ShootingItem linkers = GetEditorArea().m_ShootingItem[i];
                    linkers.IsLink = true;
                    GetEditorArea().m_ShootingItem[i] = linkers;
                    EditorUI._Instance.itemEditorUI.LinkItem.sprite = GetEditorArea().m_ShootingItem[i].MinImage;
                    EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();



                    EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        GameObject.Find("SceneComponent").GetComponent<RuntimeSceneComponent>().Focus(GetLinkList(item)[id - 1].Prefab.transform);
                    });
                }
        }
        ShootingItem shootingitem = item;
        if (id == 0)
        {
            shootingitem.LinkageItem = null;
            shootingitem.LinageItemNum = 0;
            EditorUI._Instance.itemEditorUI.LinkItem.sprite = getAreaItemListSprit("无"); ;
            EditorUI._Instance.itemEditorUI.LinkItem.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        else
        {
            shootingitem.LinkageItem = GetLinkList(item)[id - 1].Prefab.transform;
            shootingitem.LinageItemNum = id;
            GameObject.Find("SceneComponent").GetComponent<RuntimeSceneComponent>().Focus(GetLinkList(item)[id - 1].Prefab.transform);
        }

        for (int i = 0; i < GetEditorArea().m_ShootingItem.Count; i++)
        {
            if (GetEditorArea().m_ShootingItem[i].Prefab == item.Prefab)
                GetEditorArea().m_ShootingItem[i] = shootingitem;
        }
        refreshUI(Editor.Selection.objects);

    }

    //获取选中item
    public ShootingItem getActiveItem(GameObject value)
    {
        ShootingItem newItem = new ShootingItem();
        if(value)
            GetEditorArea().m_ShootingItem.ForEach(item =>
            {
                if (item.Prefab == value)
                    newItem = item;
            });

        return newItem;
    }

    [SerializeField]
    bool isPlaying = false;
    private void OnPlayClick()
    {
        if (m_Arealist.Count == 0)
        {
            EditorUI._Instance.worningUI.Type = worningType.msg;
            EditorUI._Instance.worningUI.tital.text = "请编辑场景";
            EditorUI._Instance.worningUI.msg.text = "请添加并编辑至少一个射击区域";
            EditorUI._Instance.worningUI.gameObject.SetActive(true);
            return;
        }
        EditorUI._Instance.gameFinishUI.gameObject.SetActive(false);
        isPlaying = !isPlaying;
        ShootGame._Instance.isEditor = !isPlaying;
        if (isPlaying)
        {
            //Editor.Undo.Purge();
            CoPlay();
        }
        else
            OnStopClick();
        ShootGame._Instance.delateTime = 1;
        ShootGame._Instance.Start();
    }

    private void CoPlay()
    {
        
        modul = Modul.Game;
        if (Lock)
            UnLockAcitiveArea();
        Editor.Undo.Select(null, null);
        Editor.Selection.Enabled = false;
        EditorUI._Instance.playingModle();
        Associated();
    }

    private void OnGameStart()
    {
        GameObject.Find("EventSystem").GetComponent<OutLog>().enabled = false;
        modul = Modul.Game;
        isPlaying = !isPlaying;
        Cursor.visible = false;
        ShootGame._Instance.isEditor = !isPlaying;

        if (Lock)
            UnLockAcitiveArea();
        Editor.Undo.Select(null, null);
        Editor.Selection.Enabled = false;
        EditorUI._Instance.GameModle();
        //EditorUI._Instance.sceneNameUI.OnStart();
        Associated();
        ShootGame._Instance.delateTime = 7;
        ShootGame._Instance.Start();
    }
    private void OnStopClick()
    {
        modul = Modul.Editor;
        Editor.Selection.Enabled = true;
        Editor.Undo.Select(null, null);
        EditorUI._Instance.editorModle();
    }

    //射击点与靶子关联
    void Associated()
    {
        m_Arealist.ForEach(item =>
        {
            //InteractiveSwitch Point = new InteractiveSwitch();
            List<TargetHealth> targets = new List<TargetHealth>();
            List<OtherHealth> others = new List<OtherHealth>();
            item.m_ShootingItem.ForEach(obj =>
            {
                if (obj.Prefab.GetComponent<InteractiveSwitch>())
                {
                    //Point = obj.Prefab.GetComponent<InteractiveSwitch>();
                    item.m_ShootPos = obj;
                }
                if (obj.Prefab.GetComponent<TargetHealth>())
                {
                    targets.Add(obj.Prefab.GetComponent<TargetHealth>());
                }
                if (obj.Prefab.GetComponent<OtherHealth>())
                {
                    others.Add(obj.Prefab.GetComponent<OtherHealth>());
                }

            });
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().targets = targets;
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().others = others;
        });
        if (m_Arealist.Count == 1)
        {
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = true;
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = true;
        }
        if (m_Arealist.Count > 1)
        {
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = true;
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = false;
            m_Arealist.Last<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = false;
            m_Arealist.Last<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = true;
        }

        for (int i = 0; i < m_Arealist.Count; i++)
        {
            if (i + 1 < m_Arealist.Count)
                m_Arealist[i].m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().nextStage =
                    m_Arealist[i + 1].m_ShootPos.Prefab.GetComponent<InteractiveSwitch>();
        }
        m_Arealist.Last<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().nextStage = null;
    }


    [SerializeField]
    GameObject inside, outside, editorCamera, Ground;

    void EditorScene()
    {
        if (Static.Instance.sceneType == SceneType.InSide)
        {
            inside.SetActive(true);
            outside.SetActive(false);
            editorCamera.transform.position = GameObject.Find("insidecameraPos").transform.position;
            inside.transform.Find("Top").gameObject.SetActive(false);
        }
        if (Static.Instance.sceneType == SceneType.OutSide)
        {
            inside.SetActive(false);
            outside.SetActive(true);
            editorCamera.transform.position = GameObject.Find("outsidecameraPos").transform.position;
        }
        m_Arealist.ForEach(item => {
            item.m_ShootPos.Prefab.GetComponent<MeshRenderer>().enabled = true;
        });

        Ground.gameObject.SetActive(false);
        GameObject.Find("SceneWindow").GetComponent<Image>().raycastTarget = true;
    }
    void GameScene()
    {
        if (Static.Instance.sceneType == SceneType.InSide)
        {
            inside.SetActive(true);
            outside.SetActive(false);
            //editorCamera.transform.position = GameObject.Find("insidecameraPos").transform.position;
            inside.transform.Find("Top").gameObject.SetActive(true);
        }
        if (Static.Instance.sceneType == SceneType.OutSide)
        {
            inside.SetActive(false);
            outside.SetActive(true);
            //editorCamera.transform.position = GameObject.Find("outsidecameraPos").transform.position;
        }

        m_Arealist.ForEach(item => {
            item.m_ShootPos.Prefab.GetComponent<MeshRenderer>().enabled = false;
        });
        Ground.gameObject.SetActive(true);
        GameObject.Find("SceneWindow").GetComponent<Image>().raycastTarget = false;
    }


}
