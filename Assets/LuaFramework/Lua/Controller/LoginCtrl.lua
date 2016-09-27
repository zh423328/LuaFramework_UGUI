--region *.lua
--Date
--登陆管理


LoginCtrl = {};
local this = LoginCtrl;

local luabehaviourobj;
local transform;
local gameObject;

--构建函数--
function LoginCtrl.New()
	logWarn("LoginCtrl.New--->>");
	return this;
end

function LoginCtrl.Awake()
	logWarn("LoginCtrl.Awake--->>");
	panelMgr:CreatePanel('Login', this.OnCreate);
end

--启动事件--
function LoginCtrl.OnCreate(obj)
	gameObject = obj;

	luabehaviourobj = gameObject:GetComponent('LuaBehaviour');
	luabehaviourobj:AddClick(LoginPanel.btnLogin, this.OnClick);

	logWarn("Start lua--->>"..gameObject.name);
end

--单击事件--
function LoginCtrl.OnClick(go)
	--destroy(gameObject);
    log("click login");
end

--关闭事件--
function LoginCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.Login);
end

--endregion
