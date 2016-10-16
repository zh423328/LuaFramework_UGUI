--region *.lua
--Date
--登陆管理
--Event = require 'events'
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

    --bindevent--
     --local str = string.format("%d",NFDefine_pb.EGMI_ACK_LOGIN);
     --log("bind.."..str);
     --Event.AddListener(str, this.OnLoginResult); 

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

     Event.RemoveListener(string.format("%d",NFDefine_pb.EGMI_ACK_LOGIN));
end

--登陆结果--
function LoginCtrl.OnLoginResult(buffer)
    log("login result");

	local data = buffer:ReadBuffer();
    log('OnLoginResult: read:>'..data);
    local msg = NFMsgPreGame_pb.AckEventResult();
    msg:ParseFromString(data);
	log('TestLoginPblua: protocal:>'..protocal..' msg:>'..msg.id);

    --
    local ret = msg.event_code;

    if ret ==  NFMsgPreGame_pb.EGEC_ACCOUNT_SUCCESS then
        log("login sucess");
    else
        log("login failed");
    end
end
--endregion
