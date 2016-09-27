--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

local transform;
local gameObject;

LoginPanel = {};
local this = LoginPanel;

--启动事件--
function LoginPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
	logWarn("Awake lua--->>"..gameObject.name);
end

--初始化面板--
function LoginPanel.InitPanel()
	this.btnLogin = transform:FindChild("bg/Button").gameObject;
end

--单击事件--
function LoginPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end



--endregion
