using UnityEngine;
using LuaInterface;
using System;

public class HelloWorld : MonoBehaviour
{
    void Awake()
    {
        LuaState lua = new LuaState();
        lua.Start();
        string hello =
            @"
                 function test()
                    print('1234');
                end
                print('hello tolua#')
            ";
        lua.DoString(hello, "HelloWorld.cs");
        LuaFunction func = lua.GetFunction("test");

        if(func != null)
        {
            func.Call();
        }

        lua.CheckTop();
        lua.Dispose();
        lua = null;
    }
}
