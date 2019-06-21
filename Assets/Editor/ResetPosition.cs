using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class MyEditor : EditorWindow 
{

	[MenuItem ("Tools/ReStartTool")]
	static void AddWindow ()
	{       
		//创建窗口
		Rect  wr = new Rect (0,0,300,1000);
		MyEditor window = (MyEditor)EditorWindow.GetWindowWithRect (typeof (MyEditor),wr,true,"ReStartTool");	
		window.Show();

	}

	//输入文字的内容
	private string text;

	private string X, Y, Z;
	private string Width, Height;
	private Vector3 ChangeToVe3;
	string PathWidth;
	string PathHeight;
	string PathSziea;
	//选择贴图的对象
	//private Texture texture;
	private GameObject ChoseFatherObj;
	private GameObject ChangeFatherObj;
	private List<GameObject> SaveObj=new List<GameObject>();

	public void Awake () 
	{
		//在资源中读取一张贴图
		//texture = Resources.Load("1") as Texture;
	}

	//绘制窗口时调用
	void OnGUI () 
	{
		//if(GUILayout.Button("打开通知",GUILayout.Width(200)))
		//{
			//打开一个通知栏
			//this.ShowNotification(new GUIContent("This is a Notification"));
		//}
			
		//if(GUILayout.Button("关闭通知",GUILayout.Width(200)))
		//{
			//关闭通知栏
			//this.RemoveNotification();
		//}

		//文本框显示鼠标在窗口的位置
		//EditorGUILayout.LabelField ("鼠标在窗口的位置", Event.current.mousePosition.ToString ());

		//选择贴图
		//texture =  EditorGUILayout.ObjectField("添加贴图",texture,typeof(Texture),true) as Texture;


		if(GUILayout.Button("重置坐标",GUILayout.Width(100),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				child.transform.position = new Vector3 (0, 0, 0);
			}
		}

		if(GUILayout.Button("重置旋转",GUILayout.Width(100),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				child.transform.rotation = Quaternion.Euler (0, 0, 0);
			}
		}

		if(GUILayout.Button("重置缩放",GUILayout.Width(100),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				child.transform.localScale = new Vector3 (1, 1, 1);
			}
		}


		if(GUILayout.Button("重置坐标包含子物体",GUILayout.Width(150),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				foreach (Transform childf in child.transform) 
				{
					childf.transform.position = new Vector3 (0, 0, 0);
				}
			}
		}

		if(GUILayout.Button("重置旋转包含子物体",GUILayout.Width(150),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				foreach (Transform childf in child.transform) 
				{
					childf.rotation = Quaternion.Euler (0, 0, 0);
				}
			}
		}

		if(GUILayout.Button("重置缩放包含子物体",GUILayout.Width(150),GUILayout.Height(30)))
		{
			foreach (GameObject child in SaveObj) 
			{
				foreach (Transform childf in child.transform) 
				{
					childf.transform.localScale = new Vector3 (1, 1, 1);
				}
			}
		}


		//输入框控件
		text = EditorGUILayout.TextField("输入文字:",text);
		if(GUILayout.Button("改名字",GUILayout.Width(150),GUILayout.Height(30)))
		{
			int i = 0;
			int j = 0;
			foreach (GameObject child in SaveObj) 
			{
				if (SaveObj.Count == 1)
					child.name = text;
				else 
				{
					if(i.ToString().Length==1)
					child.name = text +"0"+ i.ToString ();
					else
					child.name = text + i.ToString ();
				}
				
				j = 0;
				foreach (Transform childf in child.transform) 
				{
					childf.gameObject.name = child.name + "_child" + j.ToString ();
					j++;
				}
				i++;
			}
		}

		X=EditorGUILayout.TextField("x:",X);
		Y=EditorGUILayout.TextField("y:",Y);
		Z=EditorGUILayout.TextField("z:",Z);

		if(GUILayout.Button("选择物体所在的子物体变换坐标",GUILayout.Width(250),GUILayout.Height(30)))
		{
			ChangeToVe3 = new Vector3 (int.Parse (X), int.Parse (Y), int.Parse (Z));
			for (int i = 0; i <=SaveObj.Count-1; i++) 
			{
				SaveObj [i].transform.GetChild (0).position = ChangeToVe3;
			}
		}

		if(GUILayout.Button("选择物体所在的子物体变换局部坐标",GUILayout.Width(250),GUILayout.Height(30)))
		{
			ChangeToVe3 = new Vector3 (int.Parse (X), int.Parse (Y), int.Parse (Z));
			for (int i = 0; i <=SaveObj.Count-1; i++) 
			{
				SaveObj [i].transform.GetChild (0).localPosition = ChangeToVe3;
			}
		}

		Width=EditorGUILayout.TextField("Width:",Width);
		Height=EditorGUILayout.TextField("Height:",Height);
		Vector2 SavePos = new Vector2 ();
		if(GUILayout.Button("选择物体所在的子物体变换的Width和Height",GUILayout.Width(250),GUILayout.Height(30)))
		{
			SavePos = new Vector2 (int.Parse (Width), int.Parse (Height));
			for (int i = 0; i <=SaveObj.Count-1; i++) 
			{
				SaveObj [i].transform.GetChild (0).GetComponent<RectTransform>().sizeDelta = SavePos;
			}
		}

        //if(GUILayout.Button("选择物体所在的子物体的路径PathGridComponent",GUILayout.Width(250),GUILayout.Height(30)))
        //{
        //	for (int i = 0; i <=SaveObj.Count-1; i++) 
        //	{
        //		SaveObj [i].transform.GetChild (0).GetComponent<PathGridComponent>().m_debugShow =!SaveObj [i].transform.GetChild (0).GetComponent<PathGridComponent>().m_debugShow ;
        //	}
        //}

        //if(GUILayout.Button("选择物体所在的子物体的路径ObstacleGridComponent",GUILayout.Width(250),GUILayout.Height(30)))
        //{
        //	for (int i = 0; i <=SaveObj.Count-1; i++) 
        //	{
        //		SaveObj [i].transform.GetChild (0).GetComponent<ObstacleGridComponent>().m_show =!SaveObj [i].transform.GetChild (0).GetComponent<ObstacleGridComponent>().m_show ;
        //		SaveObj [i].transform.GetChild (0).GetComponent<ObstacleGridComponent>().m_rasterizeEveryFrame =!SaveObj [i].transform.GetChild (0).GetComponent<ObstacleGridComponent>().m_rasterizeEveryFrame ;
        //	}
        //}


        //PathWidth=EditorGUILayout.TextField("PathWidth:",PathWidth);
        //PathHeight=EditorGUILayout.TextField("PathHeight:",PathHeight);
        //PathSziea=EditorGUILayout.TextField("PathSziea:",PathSziea);
        //if(GUILayout.Button("选择物体所在的子物体的路径参数",GUILayout.Width(250),GUILayout.Height(30)))
        //{
        //	for (int i = 0; i <=SaveObj.Count-1; i++) 
        //	{
        //		SaveObj [i].transform.GetChild (0).GetComponent<PathGridComponent>().m_numberOfRows=int.Parse(PathWidth) ;
        //		SaveObj [i].transform.GetChild (0).GetComponent<PathGridComponent>().m_numberOfColumns=int.Parse(PathHeight) ;
        //		SaveObj [i].transform.GetChild (0).GetComponent<PathGridComponent> ().m_cellSize =int.Parse(PathSziea);
        //	}
        //}

        //GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);


        ChoseFatherObj = EditorGUILayout.ObjectField("选择原父物体", ChoseFatherObj, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("---------now", GUILayout.Width(250), GUILayout.Height(30)))
        {
            foreach (Transform child in ChoseFatherObj.transform)
            {
                child.GetChild(0).eulerAngles += new Vector3(0, Random.Range(5, 170), 0);
                child.GetChild(1).eulerAngles += new Vector3(0, Random.Range(5, 170), 0);
            }
        }
        //		ChangeFatherObj =  EditorGUILayout.ObjectField("选择新父物体",ChangeFatherObj,typeof(GameObject),true) as GameObject;
        //		if (GUILayout.Button ("遍历场景里ChoseFatherObj子物体到ChangeFatherObj", GUILayout.Width (250), GUILayout.Height (30))) 
        //		{
        ////			object[] gameObjects;
        ////			gameObjects = GameObject.FindSceneObjectsOfType (typeof(Transform));
        //			foreach (Transform go in ChoseFatherObj.transform)
        //			{
        //				if (go.name.Length <= 8)
        //					continue;
        //				if (go.name.Substring(0,8)=="Powerbox") 
        //				{
        //					go.parent = ChangeFatherObj.transform;
        //				}
        //			}

        //		}


        //		if (GUILayout.Button ("添加脚本", GUILayout.Width (250), GUILayout.Height (30))) 
        //		{
        //			//			object[] gameObjects;
        //			//			gameObjects = GameObject.FindSceneObjectsOfType (typeof(Transform));
        //			foreach (Transform go in ChangeFatherObj.transform)
        //			{

        //				if (go.GetComponent<FootprintComponent>()==null) 
        //				{
        //					go.gameObject.AddComponent<FootprintComponent> ();
        //				}
        //			}

        //		}

        if (GUILayout.Button("关闭窗口",GUILayout.Width(150),GUILayout.Height(20)))
		{
			//关闭窗口
			this.Close();
		}
			
	}


	//更新
	void Update()
	{

	}

	void OnFocus()
	{
		//Debug.Log("当窗口获得焦点时调用一次");
	}

	void OnLostFocus()
	{
		//Debug.Log("当窗口丢失焦点时调用一次");
	}

	void OnHierarchyChange()
	{
		//Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
	}

	void OnProjectChange()
	{
		//Debug.Log("当Project视图中的资源发生改变时调用一次");
	}

	void OnInspectorUpdate()
	{
		//Debug.Log("窗口面板的更新");
		//这里开启窗口的重绘，不然窗口信息不会刷新
		this.Repaint();
	}

	void OnSelectionChange()
	{
		SaveObj.Clear ();
		//当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		foreach(Transform t in Selection.transforms)
		{
			SaveObj.Add (t.gameObject);
			//有可能是多选，这里开启一个循环打印选中游戏对象的名称
			//Debug.Log("OnSelectionChange" + t.name);
		}
	}

	void OnDestroy()
	{
		//Debug.Log("当窗口关闭时调用");
	}
}