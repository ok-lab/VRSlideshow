using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DisplayImages : MonoBehaviour
{
    public bool AutoStart = false;

    public string InputName = "Fire1";

    private int count = 0;
    public int max;

    public Texture2D defaltTexture; 

    private void Start()
    {
        if (AutoStart)
        {
            Debug.Log("Start() : ");
            new WaitForSeconds(2.0f);
            DisplayImageOnClick();
        }

        this.max = FileCountPNG(GetDirectoryPath()) + 1;
        Debug.Log("MaxPNG : " + max);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            DisplayImageOnClick();
        }
    }

    int FileCountPNG(string dir)
    {
        string searchPattern = "*.PNG";
        int count = Directory.GetFiles(dir, searchPattern).Length;
        return count;
    }

    public void DisplayImageOnClick()
    {
        /*
         * 1. pngファイルのpathを指定
         * 2. それをTexture2Dに変える
         * 3. 貼り付ける
         */
        count++;
        if (count == max) count = 1;
        string path = GetFilePath(count);
        Texture2D texture2D = PngToTex2D(path);
        AttachedTexture2D(texture2D);
    }

    public void BackImageOnClick()
    {
        /*
         * 1. pngファイルのpathを指定
         * 2. それをTexture2Dに変える
         * 3. 貼り付ける
         */
        count--;
        if (count == 0) count = max-1;
        string path = GetFilePath(count);
        Texture2D texture2D = PngToTex2D(path);
        AttachedTexture2D(texture2D);
    }

    string GetFilePath(int count)
    {
        string path = GetDirectoryPath() + GetFileName(count);

        return path;
    }

    string GetDirectoryPath()
    {
        string DirectoryPath = Application.dataPath + "/Images/";

        return DirectoryPath;
    }

    string GetFileName(int count)
    {
        string FileName = "スライド" + count + ".PNG";

        return FileName;
    }


    Texture2D PngToTex2D(string path)
    {
        try
        {
            BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
            byte[] rb = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            int pos = 16, width = 0, height = 0;
            for (int i = 0; i < 4; i++) width = width * 256 + rb[pos++];
            for (int i = 0; i < 4; i++) height = height * 256 + rb[pos++];
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(rb);
            return texture;
        }
        catch(FileNotFoundException)
        {
            Debug.LogError("FileNotFoundException");
            return defaltTexture;
        }
    }

    void AttachedTexture2D(Texture2D texture2D)
    {
        GetComponent<Renderer>().material.mainTexture = texture2D;
    }
}
