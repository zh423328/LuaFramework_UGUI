﻿using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LuaFramework;

public class Packager
{
    private static string cAssetsResourcesPath = "Assets/" + AppConst.AppName + "/Resources";

    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
    static List<string> dirpaths = new List<string>();

    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();

    ///-----------------------------------------------------------
    static string[] exts = { ".txt", ".xml", ".lua", ".assetbundle", ".json" };
    static bool CanCopy(string ext)     //能不能复制
    {
        foreach(string e in exts)
        {
            if(ext.Equals(e)) return true;
        }

        return false;
    }

    /// <summary>
    /// 载入素材
    /// </summary>
    static UnityEngine.Object LoadAsset(string file)
    {
        if(file.EndsWith(".lua")) file += ".txt";

        return AssetDatabase.LoadMainAssetAtPath("Assets/LuaFramework/Resources/Builds/" + file);
    }

    [MenuItem("LuaFramework/Build iPhone Resource", false, 100)]
    public static void BuildiPhoneResource()
    {
        BuildTarget target;
#if UNITY_5
        target = BuildTarget.iOS;
#else
        target = BuildTarget.iPhone;
#endif
        BuildAssetResource(target);
    }

    [MenuItem("LuaFramework/Build Android Resource", false, 101)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(BuildTarget.Android);
    }

    [MenuItem("LuaFramework/Build Windows Resource", false, 102)]
    public static void BuildWindowsResource()
    {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }

    public static void HandleSceneRes()
    {
        string sceneRes = "Assets/" + AppConst.AppName + "/Scenes";
        string[] files = Directory.GetFiles(sceneRes, "*.unity");

        for(int i = 0; i < files.Length; i++)
        {
            string currentFile = files[i].Replace("\\", "/");
            int startIndex = currentFile.IndexOf(sceneRes);
            string assetFile = currentFile.Substring(startIndex, currentFile.Length - startIndex);
            string sceneName = Path.GetFileNameWithoutExtension(assetFile);
            string srcABFileName = "scenes$" + sceneName + AppConst.ExtName;
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = srcABFileName;
            build.assetNames = new string[] { files[i] };
            maps.Add(build);
        }
    }
    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource(BuildTarget target)
    {
        if(Directory.Exists(Util.DataPath))
        {
            Directory.Delete(Util.DataPath, true);
        }

        string streamPath = Application.streamingAssetsPath;

        if(Directory.Exists(streamPath))
        {
            Directory.Delete(streamPath, true);
        }

        Directory.CreateDirectory(streamPath);
        AssetDatabase.Refresh();
        maps.Clear();

        if(AppConst.LuaBundleMode)
        {
            HandleLuaBundle();
        }
        else
        {
            HandleLuaFile();
        }

        HandleSceneRes();
        HandleResourceBundle();
        string resPath = "Assets/" + AppConst.AssetDir;
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
                                          BuildAssetBundleOptions.UncompressedAssetBundle;
        BuildPipeline.BuildAssetBundles(resPath, maps.ToArray(), options, target);
        BuildFileIndex();
        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;

        if(Directory.Exists(streamDir)) Directory.Delete(streamDir, true);

        AssetDatabase.Refresh();
    }

    static void AddBuildMap(string bundleName, string pattern, string path)
    {
        string[] files = Directory.GetFiles(path, pattern);

        if(files.Length == 0) return;

        for(int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace('\\', '/');
        }

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;
        build.assetNames = files;
        maps.Add(build);
    }

    /// <summary>
    /// 处理Lua代码包
    /// </summary>
    public static void HandleLuaBundle()
    {
        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;

        if(!Directory.Exists(streamDir)) Directory.CreateDirectory(streamDir);

        string[] srcDirs = { CustomSettings.luaDir, CustomSettings.FrameworkPath + "/ToLua/Lua" };

        for(int i = 0; i < srcDirs.Length; i++)
        {
            if(AppConst.LuaByteMode)
            {
                string sourceDir = srcDirs[i];
                string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                int len = sourceDir.Length;

                if(sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
                {
                    --len;
                }

                for(int j = 0; j < files.Length; j++)
                {
                    string str = files[j].Remove(0, len);
                    string dest = streamDir + str + ".bytes";
                    string dir = Path.GetDirectoryName(dest);
                    Directory.CreateDirectory(dir);
                    EncodeLuaFile(files[j], dest);
                }
            }
            else
            {
                ToLuaMenu.CopyLuaBytesFiles(srcDirs[i], streamDir);
            }
        }

        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.AllDirectories);

        for(int i = 0; i < dirs.Length; i++)
        {
            string name = dirs[i].Replace(streamDir, string.Empty);
            name = name.Replace('\\', '_').Replace('/', '_');
            name = "lua/lua_" + name.ToLower() + AppConst.ExtName;
            string path = "Assets" + dirs[i].Replace(Application.dataPath, "");
            AddBuildMap(name, "*.bytes", path);
        }

        AddBuildMap("lua/lua" + AppConst.ExtName, "*.bytes", "Assets/" + AppConst.LuaTempDir);
        //-------------------------------处理非Lua文件----------------------------------
        string luaPath = AppDataPath + "/StreamingAssets/lua/";

        for(int i = 0; i < srcDirs.Length; i++)
        {
            paths.Clear();
            files.Clear();
            string luaDataPath = srcDirs[i].ToLower();
            Recursive(luaDataPath);

            foreach(string f in files)
            {
                if(f.EndsWith(".meta") || f.EndsWith(".lua")) continue;

                string newfile = f.Replace(luaDataPath, "");
                string path = Path.GetDirectoryName(luaPath + newfile);

                if(!Directory.Exists(path)) Directory.CreateDirectory(path);

                string destfile = path + "/" + Path.GetFileName(f);
                File.Copy(f, destfile, true);
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取目录下的所有文件夹
    /// </summary>
    static void GetResAllDir(string path)
    {
        //添加目录
        dirpaths.Add(path);
        string[] dirs = Directory.GetDirectories(path);

        for(int i = 0; i < dirs.Length; ++i)
        {
            if(dirs[i].StartsWith("."))
                continue;

            GetResAllDir(dirs[i]);
        }
    }
    /// <summary>
    /// 处理框架实例包,出了场景包
    /// </summary>
    static void HandleResourceBundle()
    {
        string resPath = AppDataPath + "/" + AppConst.AssetDir + "/";

        if(!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);

        dirpaths.Clear();
        GetResAllDir(cAssetsResourcesPath);

        for(int i = 0; i < dirpaths.Count; ++i)
        {
            string srcABFileName = string.Empty;

            if(dirpaths[i] == cAssetsResourcesPath)
            {
                srcABFileName = "Resources";
            }
            else
            {
                srcABFileName = dirpaths[i].Substring(cAssetsResourcesPath.Length);

                if(srcABFileName.StartsWith(@"\\"))
                {
                    srcABFileName = srcABFileName.Substring(2);
                }
                else if(srcABFileName.StartsWith("/") || srcABFileName.StartsWith(@"\"))
                {
                    srcABFileName = srcABFileName.Substring(1);
                }

                srcABFileName = srcABFileName.Replace('\\', '/');
                srcABFileName = srcABFileName.Replace("/", "$");
            }

            srcABFileName += AppConst.ExtName;
            string[] files = Directory.GetFiles(dirpaths[i]);
            List<string> newfiles = new List<string>();

            for(int j = 0; j < files.Length; j++)
            {
                if(files[j].EndsWith(".meta") || files[j].EndsWith(".svn") || files[j].EndsWith(".unity"))
                    continue;

                files[j] = files[j].Replace('\\', '/');
                newfiles.Add(files[j]);
            }

            if(newfiles.Count == 0)
                continue;

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = srcABFileName;
            build.assetNames = newfiles.ToArray();
            maps.Add(build);
        }

        //AddBuildMap("prompt" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Resources/Builds/Prompt");
        //AddBuildMap("message" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Resources/Builds/Message");
        //AddBuildMap("prompt_asset" + AppConst.ExtName, "*.png", "Assets/LuaFramework/Resources/Textures/Prompt");
        //AddBuildMap("shared_asset" + AppConst.ExtName, "*.png", "Assets/LuaFramework/Resources/Textures/Shared");
        ////怪物模型的
        //AddBuildMap("ai_ge_001_ty" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Resources/Model/ModelPrefabs");
        ////登陆
        //AddBuildMap("login_asset" + AppConst.ExtName, "*.png", "Assets/LuaFramework/Resources/Builds/Login");
        //AddBuildMap("Login" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Resources/Builds/Login");
    }


    /// <summary>
    /// 处理Lua文件
    /// </summary>
    static void HandleLuaFile()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        string luaPath = resPath + "/lua/";

        //----------复制Lua文件----------------
        if(!Directory.Exists(luaPath))
        {
            Directory.CreateDirectory(luaPath);
        }

        string[] luaPaths = { AppDataPath + "/LuaFramework/lua/",
                              AppDataPath + "/LuaFramework/Tolua/Lua/"
                            };

        for(int i = 0; i < luaPaths.Length; i++)
        {
            paths.Clear();
            files.Clear();
            string luaDataPath = luaPaths[i].ToLower();
            Recursive(luaDataPath);
            int n = 0;

            foreach(string f in files)
            {
                if(f.EndsWith(".meta")) continue;

                string newfile = f.Replace(luaDataPath, "");
                string newpath = luaPath + newfile;
                string path = Path.GetDirectoryName(newpath);

                if(!Directory.Exists(path)) Directory.CreateDirectory(path);

                if(File.Exists(newpath))
                {
                    File.Delete(newpath);
                }

                if(AppConst.LuaByteMode)
                {
                    EncodeLuaFile(f, newpath);
                }
                else
                {
                    File.Copy(f, newpath, true);
                }

                UpdateProgress(n++, files.Count, newpath);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void BuildFileIndex()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";

        if(File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear();
        files.Clear();
        Recursive(resPath);
        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);

        for(int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);

            if(file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = Util.md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }

        sw.Close();
        fs.Close();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get
        {
            return Application.dataPath.ToLower();
        }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach(string filename in names)
        {
            string ext = Path.GetExtension(filename);

            if(ext.Equals(".meta")) continue;

            files.Add(filename.Replace('\\', '/'));
        }

        foreach(string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    public static void EncodeLuaFile(string srcFile, string outFile)
    {
        if(!srcFile.ToLower().EndsWith(".lua"))
        {
            File.Copy(srcFile, outFile, true);
            return;
        }

        bool isWin = true;
        string luaexe = string.Empty;
        string args = string.Empty;
        string exedir = string.Empty;
        string currDir = Directory.GetCurrentDirectory();

        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            isWin = true;
            luaexe = "luajit.exe";
            args = "-b " + srcFile + " " + outFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
        }
        else if(Application.platform == RuntimePlatform.OSXEditor)
        {
            isWin = false;
            luaexe = "./luac";
            args = "-o " + outFile + " " + srcFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luavm/";
        }

        Directory.SetCurrentDirectory(exedir);
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = luaexe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.ErrorDialog = true;
        info.UseShellExecute = isWin;
        Util.Log(info.FileName + " " + info.Arguments);
        Process pro = Process.Start(info);
        pro.WaitForExit();
        Directory.SetCurrentDirectory(currDir);
    }

    [MenuItem("LuaFramework/Build Protobuf-lua-gen File")]
    public static void BuildProtobufFile()
    {
        if(!AppConst.ExampleMode)
        {
            UnityEngine.Debug.LogError("若使用编码Protobuf-lua-gen功能，需要自己配置外部环境！！");
            return;
        }

        string dir = AppConst.FrameworkRoot + "/Lua/3rd/pblua";
        paths.Clear();
        files.Clear();
        Recursive(dir);
        string protoc = "F:/GitHub/proto-lua/protoc-gen-lua/protoc-gen-lua/example/proto/protoc.exe";
        string protoc_gen_dir = "\"F:/GitHub/proto-lua/protoc-gen-lua/protoc-gen-lua/plugin/protoc-gen-lua.bat\"";

        foreach(string f in files)
        {
            string name = Path.GetFileName(f);
            string ext = Path.GetExtension(f);

            if(!ext.Equals(".proto")) continue;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = protoc;
            info.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;
            info.ErrorDialog = true;
            Util.Log(info.FileName + " " + info.Arguments);
            Process pro = Process.Start(info);
            pro.WaitForExit();
        }

        AssetDatabase.Refresh();
    }
}