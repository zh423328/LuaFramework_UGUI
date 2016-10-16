--主入口函数。从这里开始lua逻辑
function Main()					
	 --LuaFramework.Util.Log("HelloWorld");
	 --local go = UnityEngine.GameObject("go");
	 --go.transform.position = Vector3.one;	
end

--场景切换通知
function OnLevelWasLoaded(level)
	Time.timeSinceLevelLoad = 0
end