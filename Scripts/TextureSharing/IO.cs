using UnityEngine;
using System;
using System.IO;
using System.Text;

public class IO : MonoBehaviour
{
    /**
     *  Windows向けにビルドするとexeが生成されたフォルダの中に「プロジェクト名_Data」フォルダが生成される。
     * 「プロジェクト名_Data」フォルダのパスは、C#プログラム内で Application.dataPath と記述することで取得できる。
     * （※これはビルドしたexeからゲームを実行した時の話。Editor上で実行した際は代わりにAssetsフォルダのパスが取得される。）
     */

    private string path;
    private string writeTxt = "hello";
    private string fileName = "output.txt";

    void Start()
    {
        path = Application.dataPath + "/" + fileName;
        Debug.Log(path);
        ReadFile();

        WriteFile(writeTxt);
    }

    void WriteFile(string txt)
    {
        FileInfo fi = new FileInfo(path);
        using (StreamWriter sw = fi.AppendText())
        {
            sw.WriteLine(txt);
        }
    }

    void ReadFile()
    {
        FileInfo fi = new FileInfo(path);
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                string readTxt = sr.ReadToEnd();
                Debug.Log(readTxt);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

}