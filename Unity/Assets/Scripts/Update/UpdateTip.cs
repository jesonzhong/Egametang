using UnityEngine;
using System;
using UnityEngine.UI;

public class UpdateTip : MonoBehaviour
{
	private static UpdateTip s_instance;
	public static UpdateTip Instance { get { return s_instance; } }


	private Text _curVersion;
	private Text _srvVersion;
	private Text _patchSizeDesc;

	private Button _buttonUpdate;
	private Button _buttonExit;

    private Text _tipInfo1;
    private Text _tipInfo2;
    private Text _tipInfo3;
    private Text _tipInfo4;

	public bool _isPatchUpdate;


    private Text _newVersionInfo;
	void Awake()
	{
		s_instance = this;

		_curVersion = this.transform.Find("panel_update/txt_myversion").GetComponent<Text>();
		_srvVersion = this.transform.Find("panel_update/txt_newversion").GetComponent<Text>();
		_patchSizeDesc = this.transform.Find("panel_update/txt_patchnum").GetComponent<Text>();

		_buttonUpdate = this.transform.Find("panel_update/btn_ok").GetComponent<Button>();
		_buttonUpdate.onClick.AddListener(OnClickButtonUpdate);

        //_buttonExit = this.transform.Find("panel_update/btn_cancel").GetComponent<Button>();
        //_buttonExit.onClick.AddListener(OnClickButtonExit);

        _tipInfo1 = this.transform.Find("panel_update/txt_info1").GetComponent<Text>();
        _tipInfo2 = this.transform.Find("panel_update/txt_info2").GetComponent<Text>();
        _tipInfo3 = this.transform.Find("panel_update/txt_info3").GetComponent<Text>();
        _tipInfo4 = this.transform.Find("panel_update/txt_info4").GetComponent<Text>();


        _newVersionInfo = this.transform.Find("panel_update/txt_bigversion").GetComponent<Text>();
		Hide();
	}

	void Start()
	{

	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}



	private void OnClickButtonUpdate()
	{
		if (_isPatchUpdate)
		{
			Hide();
			AssetUpdater.Instance.StartDownloadPatch();
		}
		else
		{
			Application.OpenURL(ServerConfig.Instance.InstallUrl);
			Application.Quit();
		}

	}

	private void  OnClickButtonExit()
	{
		Application.Quit();
	}

	public void Show(bool isPatch=false, string patchSizeDesc = "")
	{
		_isPatchUpdate = isPatch;

		//大版本更新
		_curVersion.text = VersionManager.Instance.curVersion;
		_srvVersion.text = VersionManager.Instance.srvVersion;
		if (isPatch)
		{
			_patchSizeDesc.text = patchSizeDesc;
            _tipInfo1.text = "我的版本：";
            _tipInfo2.text = "最新版本：";
            _tipInfo3.text = "补丁大小";
            _tipInfo4.text = "点击确定更新补丁";

            _newVersionInfo.text = "";
		}
		else
		{
            _curVersion.text = "";
            _srvVersion.text = "";
			_patchSizeDesc.text = "";
            _tipInfo1.text = "";
            _tipInfo2.text = "";
            _tipInfo3.text = "";
            _tipInfo4.text = "";

            _newVersionInfo.text = "检测到新版本，是否前往更新";
		}

		this.gameObject.SetActive(true);
	}

	public void Hide()
	{
		this.gameObject.SetActive(false);
	}


	void OnDestroy()
	{

	}


}
