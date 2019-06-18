// ==================================================================
// 作    者：Pablo.风暴洋-宋杨
// 説明する：在这里输入类的功能
// 作成時間：2019-06-03
// 類を作る：ZipManager.cs
// 版    本：v 1.0
// 会    社：大连仟源科技
// QQと微信：731483140
// ==================================================================

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ZipManager
{
    private static ZipManager instance;

    public static ZipManager _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ZipManager();
            }
            return instance;
        }
    }

    /// <summary>
    /// 创建压缩任务
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destinationZipFilePath"></param>
    public void CreateZip(string sourceFilePath, string destinationZipFilePath)
    {
        if (sourceFilePath[sourceFilePath.Length - 1] != Path.DirectorySeparatorChar)
            sourceFilePath += Path.DirectorySeparatorChar;
        ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
        zipStream.SetLevel(0);  // 压缩级别 0-9
        CreateZipFiles(sourceFilePath, zipStream, sourceFilePath.Length);
        zipStream.Finish();
        zipStream.Close();
    }

    /// <summary>
    /// 递归压缩文件
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="zipStream"></param>
    /// <param name="subIndex"></param>
    private void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, int subIndex)
    {
        Crc32 crc = new Crc32();
        string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
        foreach (string file in filesArray)
        {
            //如果当前是文件夹，递归
            if (Directory.Exists(file))
            {
                CreateZipFiles(file, zipStream, subIndex);
            }
            //如果是文件，开始压缩
            else
            {
                FileStream fileStream = File.OpenRead(file);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                string tempFile = file.Substring(subIndex);
                ZipEntry entry = new ZipEntry(tempFile);
                entry.DateTime = DateTime.Now;
                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipStream.PutNextEntry(entry);
                zipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }

    /// <summary>
    /// 修改后缀
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="dfileName"></param>
    public void ChangeExtension(string fileName, string dfileName)
    {
        File.Move(fileName, dfileName);
    }
}
