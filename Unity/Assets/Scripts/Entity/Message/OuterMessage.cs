using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace Model
{
    [BsonKnownTypes(typeof(AActorMessage))]
    public abstract partial class AMessage
    {
    }

    [BsonKnownTypes(typeof(AActorRequest))]
    [ProtoInclude((int)Opcode.C2R_Login, typeof(C2R_Login))]
    [ProtoInclude((int)Opcode.C2G_LoginGate, typeof(C2G_LoginGate))]
    [ProtoInclude((int)Opcode.C2G_EnterMap, typeof(C2G_EnterMap))]
    [ProtoInclude((int)Opcode.C2G_GetRoomList, typeof(C2G_GetRoomList))]
    [ProtoInclude((int)Opcode.C2G_JoinMatchRoom, typeof(C2G_JoinMatchRoom))]
    [ProtoInclude((int)Opcode.C2G_CreateMatchRoom, typeof(C2G_CreateMatchRoom))]
    public abstract partial class ARequest : AMessage
    {
    }

    [BsonKnownTypes(typeof(AActorResponse))]
    [ProtoInclude((int)Opcode.R2C_Login, typeof(R2C_Login))]
    [ProtoInclude((int)Opcode.G2C_LoginGate, typeof(G2C_LoginGate))]
    [ProtoInclude((int)Opcode.G2C_EnterMap, typeof(G2C_EnterMap))]
    [ProtoInclude((int)Opcode.G2C_GetRoomList, typeof(G2C_GetRoomList))]
    [ProtoInclude((int)Opcode.G2C_JoinMatchRoom, typeof(G2C_JoinMatchRoom))]
    [ProtoInclude((int)Opcode.G2C_CreateMatchRoom, typeof(G2C_CreateMatchRoom))]
    public abstract partial class AResponse : AMessage
    {
    }

    [BsonKnownTypes(typeof(Actor_Test))]
    [BsonKnownTypes(typeof(AFrameMessage))]
    [BsonKnownTypes(typeof(Actor_CreateUnits))]
    [BsonKnownTypes(typeof(Actor_RankList))]
    [BsonKnownTypes(typeof(FrameMessage))]
    [ProtoInclude((int)Opcode.FrameMessage, typeof(FrameMessage))]
    [ProtoInclude((int)Opcode.AFrameMessage, typeof(AFrameMessage))]
    [ProtoInclude((int)Opcode.Actor_CreateUnits, typeof(Actor_CreateUnits))]
    [ProtoInclude((int)Opcode.Actor_RankList, typeof(Actor_RankList))]
    public abstract partial class AActorMessage : AMessage
    {
    }

    [BsonKnownTypes(typeof(Actor_TestRequest))]
    [BsonKnownTypes(typeof(Actor_TransferRequest))]
    public abstract partial class AActorRequest : ARequest
    {
    }

    [BsonKnownTypes(typeof(Actor_TestResponse))]
    [BsonKnownTypes(typeof(Actor_TransferResponse))]
    public abstract partial class AActorResponse : AResponse
    {
    }

    /// <summary>
    /// 帧消息，继承这个类的消息会经过服务端转发
    /// </summary>
    [ProtoInclude((int)Opcode.Frame_ClickMap, typeof(Frame_ClickMap))]
    [BsonKnownTypes(typeof(Frame_ClickMap))]
    public abstract partial class AFrameMessage : AActorMessage
    {
    }

    [ProtoInclude((int)Opcode.Frame_ClickAction, typeof(Frame_ClickAction))]
    [BsonKnownTypes(typeof(Frame_ClickMap))]
    public class Frame_ClickAction : AFrameMessage
    {
        [ProtoMember(1)]
        public int ActionID;
    }

    [ProtoContract]
    [Message(Opcode.C2R_Login)]
    public class C2R_Login : ARequest
    {
        [ProtoMember(1)]
        public string Account;

        [ProtoMember(2)]
        public string Password;
    }

    [ProtoContract]
    [Message(Opcode.R2C_Login)]
    public class R2C_Login : AResponse
    {
        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public long Key { get; set; }
    }

    [ProtoContract]
    [Message(Opcode.C2G_LoginGate)]
    public class C2G_LoginGate : ARequest
    {
        [ProtoMember(1)]
        public long Key;
    }

    [ProtoContract]
    [Message(Opcode.G2C_LoginGate)]
    public class G2C_LoginGate : AResponse
    {
        [ProtoMember(1)]
        public long PlayerId;
    }


    [Message(Opcode.Actor_Test)]
    public class Actor_Test : AActorMessage
    {
        public string Info;
    }

    [Message(Opcode.Actor_TestRequest)]
    public class Actor_TestRequest : AActorRequest
    {
        public string request;
    }

    [Message(Opcode.Actor_TestResponse)]
    public class Actor_TestResponse : AActorResponse
    {
        public string response;
    }


    [Message(Opcode.Actor_TransferRequest)]
    public class Actor_TransferRequest : AActorRequest
    {
        public int MapIndex;
    }

    [Message(Opcode.Actor_TransferResponse)]
    public class Actor_TransferResponse : AActorResponse
    {
    }

    [ProtoContract]
    [Message(Opcode.C2G_EnterMap)]
    public class C2G_EnterMap : ARequest
    {
    }

    [ProtoContract]
    [Message(Opcode.G2C_EnterMap)]
    public class G2C_EnterMap : AResponse
    {
        [ProtoMember(1)]
        public long UnitId;
        [ProtoMember(2)]
        public int Count;
    }

    [ProtoContract]
    [Message(Opcode.C2G_CreateMatchRoom)]
    public class C2G_CreateMatchRoom : ARequest
    {
    }

    [ProtoContract]
    [Message(Opcode.G2C_CreateMatchRoom)]
    public class G2C_CreateMatchRoom : AResponse
    {
        [ProtoMember(1)]
        public long RoomId;
    }

    [ProtoContract]
    [Message(Opcode.C2G_GetRoomList)]
    public class C2G_GetRoomList : ARequest
    {
    }

    [ProtoContract]
    [Message(Opcode.G2C_GetRoomList)]
    public class G2C_GetRoomList : AResponse
    {
        [ProtoMember(1)]
        public long[] RoomIds;
    }
    [ProtoContract]
    [Message(Opcode.C2G_JoinMatchRoom)]
    public class C2G_JoinMatchRoom : ARequest
    {
        [ProtoMember(1)]
        public long RoomId;
    }

    [ProtoContract]
    [Message(Opcode.G2C_JoinMatchRoom)]
    public class G2C_JoinMatchRoom : AResponse
    {
        [ProtoMember(1)]
        public long[] UnitIds;
        public FrameMessage AgoFrameMessage;
    }

    [ProtoContract]
    public class UnitInfo
    {
        [ProtoMember(1)]
        public long UnitId;
        [ProtoMember(2)]
        public int X;
        [ProtoMember(3)]
        public int Z;
    }

    [ProtoContract]
    [Message(Opcode.Actor_CreateUnits)]
    public class Actor_CreateUnits : AActorMessage
    {
        [ProtoMember(1)]
        public List<UnitInfo> Units = new List<UnitInfo>();
    }

    [ProtoContract]
    public class RankInfo
    {
        [ProtoMember(1)]
        public long Id;
        [ProtoMember(2)]
        public string name;
        [ProtoMember(3)]
        public int score;
    }

    [ProtoContract]
    [Message(Opcode.Actor_RankList)]
    public class Actor_RankList : AActorMessage
    {
        [ProtoMember(1)]
        public List<RankInfo> Units = new List<RankInfo>();
    }

    public struct FrameMessageInfo
    {
        public long Id;
        public AMessage Message;
    }

    [ProtoContract]
    [Message(Opcode.FrameMessage)]
    public class FrameMessage : AActorMessage
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<AFrameMessage> Messages = new List<AFrameMessage>();
    }

    [ProtoContract]
    [Message(Opcode.Frame_ClickMap)]
    public class Frame_ClickMap : AFrameMessage
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Z;
    }

    [Message(Opcode.C2M_Reload)]
    public class C2M_Reload : ARequest
    {
        public AppType AppType;
    }

    [Message(Opcode.M2C_Reload)]
    public class M2C_Reload : AResponse
    {
    }

    [Message(Opcode.C2R_Ping)]
    public class C2R_Ping : ARequest
    {
    }

    [Message(Opcode.R2C_Ping)]
    public class R2C_Ping : AResponse
    {
    }
}