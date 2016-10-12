--region *.lua
--Date
--登陆管理
require("3rd/pblua/NFDefine_pb")
require("3rd/pblua/NFMsgBase_pb")
require "3rd/pblua/NFMsgPreGame_pb"

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
	panelMgr:CreatePanel('builds$login','LoginPanel' ,this.OnCreate);
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

    --登陆--
    local login = NFMsgPreGame_pb.ReqAccountLogin();
    login.account = "test1";
    login.password = "123456";
    login.security_code = "";
    login.signBuff = "";
    login.clientVersion = 1;
    login.loginMode = 0;
    login.clientIP = 0;
    login.clientMAC = 0;
    login.device_info = "";
    login.extra_info = "";
    local msg = login:SerializeToString();
    --Util.Log(msg);
    ----------------------------------------------------------------
    local buffer = ByteBuffer.New();
    buffer:WriteBuffer(msg);
    networkMgr:SendMsg(NFDefine_pb.EGMI_REQ_LOGIN,buffer);
    --]]
end

--关闭事件--
function LoginCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.Login);
end

--endregion
