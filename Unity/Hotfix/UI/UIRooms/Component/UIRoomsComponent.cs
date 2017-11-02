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
            
            CreateRoom = rc.Get<GameObject>("CreateRoom");
            if (CreateRoom != null)
            {
                CreateRoom.GetComponent<Button>().onClick.Add(OnCreateRoom);
            }
        }

        public void Update()
        {
            
        }

        public async void OnCreateRoom()
        {
            Log.Debug("OnCreateRoom");
            
            G2C_CreateRoom g2CCreateRoom = await SessionComponent.Instance.Session.Call<G2C_CreateRoom>(new C2G_CreateRoom());
            string roomid = g2CCreateRoom.RoomId.ToString();
            Hotfix.Scene.GetComponent<UIComponent>().Remove(UIType.Rooms);
            Hotfix.Scene.GetComponent<UIComponent>().Create(UIType.BattleMain);
            Log.Debug("OnCreateRoom --- RoomID " + roomid);
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
                
                roomItem.transform.localScale = roomItem.transform.localScale;
                roomItem.transform.localPosition = roomItem.transform.localPosition;
                roomItem.SetActive(true);
            }
            /*TimerComponent timerComponent = Hotfix.Scene.GetComponent<TimerComponent>();
            while (true)
            {
                await timerComponent.WaitAsync(1000);
                GameObject roomItem = GameObject.Instantiate(RoomItem,GridCenter.transform);
                roomItem.transform.localScale = roomItem.transform.localScale;
                roomItem.transform.localPosition = roomItem.transform.localPosition;
                roomItem.SetActive(true);
            }*/
        }

        public async void OnAddRoom()
        {
            
        }
    }
}