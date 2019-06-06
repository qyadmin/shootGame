
/*************************
 * Title: 工程名
 * Function:方法作用
 *      - 打开文件对话框、弹出式对话框
 * Used By: 
 * Author: 001
 * Date:    2015.10
 * Version: 1.0
 * Record:  
 *      
 *************************/
using UnityEngine;
using System.Collections;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Windows.Forms;
#endif
using System.Runtime.InteropServices;
using System;

public class OpenDialog {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    /// <summary>
    /// 弹出对话框 从用户获得图片的文件路径
    /// </summary>
    /// <returns>The picture file name.</returns>
    public static string GetPictureFileName()
	{
		string res = "";
		OpenFileDialog OpenFile = new OpenFileDialog ();
		//PSD, TIFF, JPG, TGA, PNG, GIF, BMP, IFF, PICT
		OpenFile.Filter = "所有文件(*.*)|*.*|" + "所有图像|*.psd;*.tiff;*.jpg;*.tga;*.png;*.gif;*.bmp;*.iff;*.pict|" + 
			"PSD(*.psd)|*.psd|TIFF(*.tiff)|*.tiff|JPG(*.jpg)|*.jpg|TGA(*.tga)|*.tga|PNG(*.png)|*.png"+
				"|GIF(*.gif)|*.gif|BMP(*.bmp)|*.bmp|IFF(*.iff)|*.iff|PICT(*.pict)|*.pict";
		OpenFile.InitialDirectory = "file://" + UnityEngine.Application.dataPath;
		if(OpenFile.ShowDialog() == DialogResult.OK){
			res = "file://" + OpenFile.FileName;
		}
		return res;
	}
	/// <summary>
	/// 弹出对话框 从用户获得音频文件路径
	/// </summary>
	/// <returns>The mi file name.</returns>
    public static string GetAudioFileName()
	{
		string res = "";
		OpenFileDialog OpenFile = new OpenFileDialog ();
		//PSD, TIFF, JPG, TGA, PNG, GIF, BMP, IFF, PICT
		OpenFile.Filter = 
            //"所有文件(*.*)|*.*|" + "所有音频|*.aif;*.wav;*.mp3;*.ogg;*.xm;*.mod;*.it;*.s3m|" + 
			//"aif(*.aif)|*.aif|wav(*.wav)|*.wav|mp3(*.mp3)|*.mp3|" + 
            "ogg(*.ogg)|*.ogg"
            // +"|xm(*.xm)|*.xm|mod(*.mod)|*.mod|it(*.it)|*.it|s3m(*.s3m)|*.s3m"
            ;
		OpenFile.InitialDirectory = "file://" + UnityEngine.Application.dataPath;
		if(OpenFile.ShowDialog() == DialogResult.OK){
			res = "file://" + OpenFile.FileName;
		}
		return res;
	}

	/// <summary>
	/// 弹出对话框 从用户获得视屏文件路径
	/// </summary>
	/// <returns>The mi file name.</returns>
    public static string GetVideoFileName()
	{
        string res = "";
		OpenFileDialog OpenFile = new OpenFileDialog ();
		//PSD, TIFF, JPG, TGA, PNG, GIF, BMP, IFF, PICT
        OpenFile.Filter = "ogg(*.ogg)|*.ogg";
            //"所有文件(*.*)|*.*|" + "所有视频|*.mp4;*.mov;*.mpg;*.mpeg;*.avi;*.asf|" + 
            //"mp4(*.mp4)|*.mp4|avi(*.avi)|*.avi|mov(*.mov)|*.mov|mpg(*.mpg)|*.mpg|mpeg(*.mpeg)|*.mpeg"+
            //    "|asf(*.asf)|*.asf";
		OpenFile.InitialDirectory = "file://" + UnityEngine.Application.dataPath;
		if(OpenFile.ShowDialog() == DialogResult.OK){
			res = "file://" + OpenFile.FileName;
		}
		return res;
	}
    /// <summary>
    /// 打开文件夹对话框
    /// </summary>
    /// <returns></returns>
    public static string GetFolder() 
    {
        string res = string.Empty;
        
        FolderBrowserDialog fb = new FolderBrowserDialog();

        DialogResult dr = fb.ShowDialog();
        if(dr == DialogResult.OK)
        {
            res = fb.SelectedPath;
        }
        return res;
    }
    /// <summary>
    /// 弹出提示对话框
    /// </summary>
    /// <param name="pStr"></param>
    /// <param name="pTatel"></param>
    public static void ShowDialog(string pStr, string pTatel) 
    {
        MessageBox.Show(pStr,pTatel,MessageBoxButtons.OK,MessageBoxIcon.Information);
    }
    /// <summary>
    /// 保存文件对话框
    /// </summary>
    /// <returns></returns>
    public static string SaveDialog(string pFilter) 
    {
        string res = "";
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = pFilter;
        DialogResult dr = sfd.ShowDialog();
        if(dr == DialogResult.OK)
        {
            res = sfd.FileName;
        }
        return res;
    }
#endif
}
