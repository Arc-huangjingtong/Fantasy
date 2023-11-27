using UnityEngine;
using UnityEngine.UI;
using Fantasy;

/// <summary>
/// 登录面板
/// </summary>
public class LoginPanel : BasePanel
{
    public InputField account;
    public InputField password;
    public Text prompt;
    public Button btn_submit;
    public Button btn_register;
    public Button btn_selectZone;
    public Text zoneInfo;

    [HideInInspector]public bool isLogining;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void EnterPanel()
    {
        base.EnterPanel();
        zoneInfo.text = GameManager.Ins.defaultZone.ZoneName;
    }

    void Start()
    {
        btn_selectZone.onClick.SetListener(() => {
            mUIFacade.GetUIPanel(StringManager.ServerListPanel).EnterPanel(); 
        });

        // 异步事件
        btn_submit.onClick.SetListener(async () => {
            // >> test
            UIFacade.Instance.EnterScene(new RoleScene());
            return;

            // 前端验证
            if(account.text==""||password.text==""){
                prompt.text = StringManager.AccNull;
                return;
            }

            // >>process account 网络层判断登录
            await Login(account.text,password.text);
        });

        btn_register.onClick.SetListener(() => {
            // >>process account 从登录进入注册界面
            mUIFacade.GetUIPanel(StringManager.RegisterPanel).EnterPanel();     
        });
    }

    public async FTask Login(string name,string password)
    {
        // >>process account 网络层判断登录
        var response = (R2C_LoginResponse) await GameManager.sender.Login(new C2R_LoginRequest(){
            AuthName = name,Pw = password, Version = ConstValue.Version
        });

        GameManager.response.LoginResponse(response.ErrorCode);
        Log.Info(response.ErrorCode.ToJson());

        if(response.ErrorCode!=ErrorCode.Success)
            return;

        // 更新网关地址
        GameManager.Ins.gateAdress = response.GateAddress+":"+response.GatePort;
        
        // 登录帐号后连接网关
        await LoginGate(account.text,response.Key);
    }

    public async FTask LoginGate(string account,long key)
    {
        // 登录网关
            // 登录网关后创建角色，或者加载角色列表，选择角色进入游戏地图
        var loginGate = (G2C_LoginGateResponse) await GameManager.sender.Call(new C2G_LoginGateRequest(){
            AuthName = account, Key = key,
        });
        Log.Info(loginGate.ErrorCode.ToJson());
    }

    public override void ResetPanel(){
        // 重置输入区内容
        account.text = "";
        password.text = "";
        prompt.text = "";
        isLogining = false;
    }

}