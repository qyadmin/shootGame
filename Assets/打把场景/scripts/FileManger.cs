
/*************************
 * Title: 工程名
 * Function:方法作用
 *      - 
 * Used By: 
 * Author: 001
 * Date:    2015.10
 * Version: 1.0
 * Record:  
 *      
 *************************/
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;



public class FileManger  {
    /// <summary>
    /// 存储项目信息的类
    /// </summary>
    public class Project
    {
        public string Name;   // 项目名称
        public string ProjectPath;  //  项目路径
        public string ProjectNamePath;  //  项目全文件夹路径
        public Project(string pName, string pPath)
        {
            Name = pName;
            ProjectPath = pPath;
            ProjectNamePath = ProjectPath + "\\" + Name;
        }
    }
    //public Stack<Project> Projects; //  存储所有工程的配置信息的 Stack
    public List<Project> Projects;  // 存储所有工程的配置信息的字典
    public string ConfigName = "Config.ini";    ///  编辑器的配置文件ini的名字
    public string ConfigFilePath;   //  應用程序存储数据的目錄

    public const string ConfigSection = "Projects";
    public const string ProjectName = ".ProjectName";
    public const string ProjectPath = ".ProjectPath";
    public const string ProjectCount = "ProjectCount";

    public List<string> DeleteFileName;     ///  保存删除的图片名称
    public List<string> AddFileName;   ///  保存在修改模式下新添加的图片文件路径
    public string IniFileName = "config.ini";   ///  用户的配置文件ini的名字
    
    public string UserPath;
    public string UserProjectName;
    public Dictionary<string,DirectoryInfo> ProjectNames; // 保存數據的工程目錄下所有的項目 文件夾名稱 和 日期
    public string CompilingPath;  // 编译的路径
    public static Project CurrentProject;   //  用户当前操作的项目
    public static Dictionary<string, string> PictureTailDic = new Dictionary<string, string>()
    {
        {"jpg",".jpg"},{"png",".png"},{"jpeg",".jpeg"},
        {"gif",".gif"},{"icon",".icon"}
    };
    public const string AudioTail = ".ogg";
    public const string VideoTail = ".ogg";
    public const string SampleName = "Sample";
    public const string PictureTail = ".jpg";
   
    /// <summary>
    /// 获取带扩展名的文件名字
    /// </summary>
    /// <param name="pFileName"></param>
    /// <returns></returns>
    public static string GetSourceFileName(string pFileName) 
    {
        string res = pFileName;
        int n = LastTargetIndex(pFileName, '\\'), h = LastTargetIndex(pFileName, '/');
        if(n != -1)
        {
            if (n > h)
            {
                res = pFileName.Substring(n + 1);
            }
            else
            {
                res = pFileName.Substring(h + 1);
            }
        }
        else if(h != -1)
        {
            res = pFileName.Substring(h + 1);
        }
        //Debug.Log("GetSourceFileName:" + pFileName + "; " + res + ";  n=" + n.ToString() + ";  h=" + h.ToString());
        return res;
    }
    /// <summary>
    /// 查找目标字符在字符串中最后一次出现的位置
    /// </summary>
    /// <param name="pStr"></param>
    /// <param name="pCh"></param>
    /// <returns></returns>
    public static int LastTargetIndex(string pStr,char pCh) 
    {
        int res = -1;
        char[] chs = pStr.ToCharArray();
        for (int i = 0; i < chs.Length; i++)
        {
            if (pCh == chs[i])
            {
                res = i;
            }
        }
        return res;
    }


   
    /// <summary>
    /// 编译发布
    /// </summary>
    /// <param name="pFileName"></param>
    public void Compiling(string pFileName) 
    {
        string xmlpath = Application.dataPath  + "/ShootingItem" + ".xml";
        string Launchxmlpath = Application.dataPath + "/Simple" + "/shooting_Data/";
        string Launchpath = Application.dataPath  + "/Simple/";

        CopyDirectory(new DirectoryInfo(xmlpath), new DirectoryInfo(Launchxmlpath), false);
        CopyDirectory(new DirectoryInfo(Launchpath),new DirectoryInfo(pFileName),true);
        File.Delete(xmlpath);
        File.Delete(Launchxmlpath + "/ShootingItem.xml");

        //DirectoryInfo di = new DirectoryInfo(xmlpath);
        //DirectoryInfo dis = new DirectoryInfo(Launchxmlpath+"/ShootingItem.xml");
        //di.Delete(true);        
        //dis.Delete(true);
    }


    public static void Delete()
    {
        string xmlpath = Application.dataPath + "/ShootingItem" + ".xml";
        //string Launchxmlpath = Application.dataPath + "/Simple" + "/shooting_Data/";

        File.Delete(xmlpath);
        //File.Delete(Launchxmlpath + "/ShootingItem.xml");
    }

    /// <summary>
    /// 获取文件的扩展名
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFileExt(string fileName) 
    {
        string res = string.Empty;
        int n = fileName.LastIndexOf('.');
        if(n != -1)
        {
            res = fileName.Substring(n,fileName.Length - n);
        }
        return res;
    }
    /// <summary>
    /// 获取文件名字
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetNotExtFileName(string fileName)
    {
        string res = string.Empty;
        int n = fileName.LastIndexOf('.');
        if (n != -1)
        {
            res = fileName.Substring(0, n);
        }
        return res;
    }
    /// <summary>
    /// 获取WWW格式路径
    /// </summary>
    /// <param name="fileFilePath"></param>
    /// <returns></returns>
    public static string GetWWWFilePath(string fileFilePath) 
    {
        string res = "";
        int n = res.IndexOf("file://");
        if(n >= 0)
        {
            fileFilePath = fileFilePath.Substring(n + 7, fileFilePath.Length - 7);
        }
        res = fileFilePath.Replace(@"\", @"\\");
        res = res.Replace("/", @"\\");
        return "file://" + res;
    }

    public static string GetFilePath(string fileFilePath)
    {
        string res = fileFilePath.Replace(@"\", @"\\");
        res = res.Replace("/", @"\\");
        return res;
    }

    /// <summary>
    /// 获取Windows下格式路径
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetWindowsPath(string filePath) 
    {
        string res = filePath;
        int n = res.IndexOf("file://");
        if (n == -1)
        {
            res = res.Replace("/", @"\");
        }
        else 
        {
            res = res.Substring(n + 7,res.Length - 7);
        }
        return res;
    }


    public Project GetProject(string pProjectNamePath) 
    {
        Project pj = null;
        for (int i = 0; i < Projects.Count; i++)
        {
            if(Projects[i].ProjectNamePath == pProjectNamePath)
            {
                pj = Projects[i];
                break;
            }
        }
        return pj;
    }  

    /// <summary>
    /// 创建保存项目的文件夹
    /// </summary>
    /// <param name="pProjectName">文件夹的名字</param>
    public bool CreateSubDirectory(Project pProject) 
    {
        bool res = true;
        DirectoryInfo dir = new DirectoryInfo(pProject.ProjectNamePath);
        //Debug.Log(pProject.ProjectNamePath);
        if (dir.Exists)
        {
            //  如果存在配置文件 则创建目录失败
            if (IsProject(pProject))
            {
                res = false;
            }
        }
        else 
        {
            dir.Create();
        }
        if(res)
        {
            CurrentProject = pProject;
            if(!IsContentProject(pProject))
            {
                Projects.Add(CurrentProject);
            } 
        }
        return res;
    }

    public static bool TestChineseDirectory(string pStr) 
    {
        bool res = false;
        char[] strCh = pStr.ToCharArray();
        for (int i = 0; i < strCh.Length; i++)
        {
            if (Regex.IsMatch(strCh[i].ToString(), @"^[\u4e00-\u9fa5]+$"))
            {
                res = true;
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 如果文件夹是项目文件夹
    /// </summary>
    /// <param name="pProject"></param>
    /// <returns></returns>
    public bool IsProject(Project pProject) 
    {
        bool res = false;
        //  是否存在配置文件
        if (File.Exists(pProject.ProjectNamePath + "/" + IniFileName))
        {
            res = true;
        }
        return res;
    }
    /// <summary>
    /// 如果文件夹是项目文件夹
    /// </summary>
    /// <param name="pProject"></param>
    /// <returns></returns>
    public bool IsProject(string pProjectPath)
    {
        bool res = false;
        //  是否存在配置文件
        if (File.Exists(pProjectPath + "/" + IniFileName))
        {
            res = true;
        }
        return res;
    }
    /// <summary>
    /// 设置当前的项目 如果项目在列表中不存在 将项目加入列表
    /// </summary>
    public void SetCurrProject(string pProjectPath) 
    {
        string name = GetFolderName(pProjectPath),path = GetProjectPath(pProjectPath);
        if(name != "" && path != "")
        {
            CurrentProject = new Project(name,path);
            //Debug.Log(CurrentProject.Name + ":" + CurrentProject.ProjectPath);
            //  如果项目在列表中不存在
            if (!IsContentProject(CurrentProject))
            {
                Projects.Add(CurrentProject);
            }
        }
    }
    /// <summary>
    /// 设置当前的项目 如果项目在列表中不存在 将项目加入列表
    /// </summary>
    public void SetCurrProject(Project pProject)
    {
        CurrentProject = pProject;
        //Debug.Log(CurrentProject.Name + ":" + CurrentProject.ProjectPath);
        //  如果项目在列表中不存在
        if (!IsContentProject(CurrentProject))
        {
            Projects.Insert(0,CurrentProject);
        }
        else //  如果项目在列表中 已经 存在 那么将其移动到第一个
        {
            Project pj;
            for (int i = 0; i < Projects.Count; i++)
            {
                if (pProject.ProjectNamePath == Projects[i].ProjectNamePath)
                {
                    pj = Projects[i];
                    Projects.RemoveAt(i);
                    Projects.Insert(0,pj);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 从全文件路径 获取项目文件的父文件夹 的路径
    /// </summary>
    /// <param name="pProjectFilePath"></param>
    /// <returns></returns>
    public string GetProjectPath(string pProjectFilePath) 
    {
        string res = "";
        int n = pProjectFilePath.LastIndexOf('/'),m = pProjectFilePath.LastIndexOf('\\'),h ;
        if(m != -1)
        {
            h = m;
        }
        else if (n != -1)
        {
            h = n;
        }
        else 
        {
            Debug.Log("路径名称有误：" + pProjectFilePath);
            h = pProjectFilePath.Length;
        }
        res = pProjectFilePath.Substring(0,h);
        return res;
    }
    /// <summary>
    /// 判断项目是否则读取的配置文件列表中
    /// </summary>
    /// <param name="pj"></param>
    /// <returns></returns>
    public bool IsContentProject(Project pj) 
    {
        bool res = false;
        for (int i = 0; i < Projects.Count; i++)
        {
            if (Projects[i].ProjectNamePath == pj.ProjectNamePath)
            {
                res = true;
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 获取最底层文件夹的名字
    /// </summary>
    /// <param name="pFilePath"></param>
    /// <returns></returns>
    public static string GetFolderName(string pFilePath) 
    {
        string res = "";
        int n = pFilePath.LastIndexOf('/'),m = pFilePath.LastIndexOf('\\'),h ;
        if(m != -1)
        {
            h = m;
        }
        else if (n != -1)
        {
            h = n;
        }
        else 
        {
            Debug.Log("路径名称有误：" + pFilePath);
            h = pFilePath.Length;
        }
        res = pFilePath.Substring(h + 1);
        return res;
    }

    /// <summary>
    /// 获取项目文件夹下所有 项目名称（文件夹名称） 和 项目及配置文件全路径 的字典
    /// </summary>
    /// <param name="parentDir"></param>
    /// <returns></returns>
    public Dictionary<string, DirectoryInfo> GetSubDictionaryInfo(string pFilePath) 
    {
        Dictionary<string, DirectoryInfo> subDirs = new Dictionary<string, DirectoryInfo>();
        DirectoryInfo dir = new DirectoryInfo(pFilePath);
        //Debug.Log(dir.ToString());
        DirectoryInfo[] dirEs = dir.GetDirectories();
        DirectoryInfo dirInfo;
        string folder = "";
        for (int i = 0; i < dirEs.Length; i++)
        {
            dirInfo = new DirectoryInfo(dirEs[i].ToString() + "/" + IniFileName);
            folder = GetFolderName(dirEs[i].ToString());
            if(dirInfo.Exists)
            {
                subDirs.Add(folder, dirEs[i]);
            }
        }
        return subDirs;
    }


    public bool GetOutOfAddImgFileName(string fileName)
    {
        bool res = false;
        for (int i = 0; i < AddFileName.Count; i++)
        {
            if (fileName == AddFileName[i])
            {
                res = true;
                break;
            }
        }
        return res;
    }
    /// <summary>
    /// 返回全景图对象中的热点图片的可用的本地文件名字
    /// </summary>
    /// <param name="pFileName">图片的全文件路径</param>
    /// <param name="pLocalFile">图片的本地文件名字</param>
    /// <param name="pfileType">文件类型</param>
    /// <param name="k">可用命名方式的父起点</param>
    /// <param name="m">可用命名方式的子起点</param>
    /// <returns></returns>
    
    


    /// <summary>
    /// 拷贝目录内容
    /// </summary>
    /// <param name="source">源目录</param>
    /// <param name="destination">目的目录</param>
    /// <param name="copySubDirs">是否拷贝子目录</param>
    public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination, bool copySubDirs)
    {
        if (!destination.Exists)
        {
            destination.Create(); //目标目录若不存在就创建
        }
        FileInfo[] files = source.GetFiles();
        foreach (FileInfo file in files)
        {
            file.CopyTo(Path.Combine(destination.FullName, file.Name), true); // 复制目录中所有文件
        }
        if (copySubDirs)
        {
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                string destinationDir = Path.Combine(destination.FullName, dir.Name);
                CopyDirectory(dir, new DirectoryInfo(destinationDir), copySubDirs); //复制子目录
            }
        }
    }
}
