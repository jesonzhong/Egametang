using System.Collections;
using Model;
using UnityEngine;
using UnityEngine.UI;
namespace Hotfix
{
    [ObjectEvent]
    public class UIRoomsComponentEvent : ObjectEvent<UIRoomsComponent>, IAwake,IUpdate
    {
        public void Awake()
        {
            this.Get().Awake();
        }

        public void Update()
        {
            this.Get().Update();
        }
    }
    public class UIRoomsComponent:Component
    {
        public GameObject GridCenter;
        public GameObject RoomItem;
        public GameObject GetRoomList;
        public GameObject CreateRoom;
        public void Awake()
        {
            ReferenceCollector rc = this.GetEntity<UI>().GameObject.GetComponent<ReferenceCollector>();
            GridCenter = rc.Get<GameObject>("GridCenter");
            RoomItem = rc.Get<GameObject>("RoomItem");
            GetRoomList = rc.Get<GameObject>("GetRoomList");
            
            if (GetRoomList != null)
            {
                GetRoomList.GetComponent<Button>().onClick.Add(OnGetRoomList);
            }
            
            CreateRoom = rc.Get<GameObject>("CreateMatchRoom");
            if (CreateRoom != null)
            {
                CreateRoom.GetComponent<Button>().onClick.Add(OnCreateMatchRoom);
            }
        }

        public void Update()
        {
            
        }

        public async void OnCreateMatchRoom()
        {
            Log.Debug("OnCreateMatchRoom");
            
            G2C_CreateMatchRoom g2CCreateRoom = await SessionComponent.Instance.Session.Call<G2C_CreateMatchRoom>(new C2G_CreateMatchRoom());
            string roomid = g2CCreateRoom.RoomId.ToString();
            
            Log.Debug("OnCreateMatchRoom --- RoomID " + roomid);
        }

        public async void OnGetRoomList()
        {
            Log.Debug("OnGetRoomList");
            G2C_GetRoomList g2CGetRoomList = await SessionComponent.Instance.Session.Call<G2C_GetRoomList>(new C2G_GetRoomList());
            for (int i = 0; i < g2CGetRoomList.RoomIds.Length; i++)
            {
                GameObject roomItem = GameObject.Instantiate(RoomItem,GridCenter.transform);

                Button button = roomItem.transform.Find("Button").GetComponent<Button>();
                
                Text text = button.transform.Find("Text").GetComponent<Text>();
                text.text = g2CGetRoomList.RoomIds[i].ToString();
                
                button.onClick.Add(() =>
                {
                    this.OnClickAddRoom(button.gameObject);
                });
                
                roomItem.transform.localScale = roomItem.transform.localScale;
                roomItem.transform.localPosition = roomItem.transform.localPosition;
                roomItem.SetActive(true);
            }
        }

        public void OnClickAddRoom(GameObject butObj)
        {
            Text text = butObj.transform.Find("Text").GetComponent<Text>();
            Debug.Log(text.text);

            JoinRoom(long.Parse(text.text));
        }

        public async void JoinRoom(long roomid)
        {
            G2C_JoinMatchRoom g2CCreateRoom = await SessionComponent.Instance.Session.Call<G2C_JoinMatchRoom>(new C2G_JoinMatchRoom(){  RoomId = roomid });
            Debug.Log(g2CCreateRoom.UnitIds.Length.ToString());
            
            Hotfix.Scene.GetComponent<UIComponent>().Remove(UIType.Rooms);
            Hotfix.Scene.GetComponent<UIComponent>().Create(UIType.BattleMain);
            if (g2CCreateRoom.AgoFrameMessage != null)
            {
                Game.Scene.GetComponent<ClientFrameComponent>().SetAgoFrameMessage(g2CCreateRoom.AgoFrameMessage);
            }
        }
        
        
    }
}