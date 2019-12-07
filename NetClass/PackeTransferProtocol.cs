using CommonClass;
using ICSharpCode.SharpZipLib.GZip;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace CommonClassLibrary
{
    public class MsgPackSession : AppSession<MsgPackSession, BinaryRequestInfo>
    {
    }

    public class MsgPackServer : AppServer<MsgPackSession, BinaryRequestInfo>
    {
        public MsgPackServer() : base(new MsgPackReceiveFilterFactory())
        {

        }
    }


    public class MsgPackReceiveFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new MsgPackReceiveFilter();
        }
    }

    /// +-------+---+-------------------------------+
    /// |request| l |                               |
    /// | name  | e |    request body               |
    /// |  (4)  | n |                               |
    /// |       |(4)|                               |
    /// +-------+---+-------------------------------+
    public class MsgPackReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public MsgPackReceiveFilter() : base(8)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var headerData = new byte[4];
            Array.Copy(header, offset + 4, headerData, 0, 4);
            return BitConverter.ToInt32(headerData, 0);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, 4), bodyBuffer.CloneRange(offset, length));
        }
    }


    public static class PacketTranslator
    {
        public static string GetTypeString(TransferType type) {
            switch (type)
            {
                case TransferType.TypeLogin:
                    return "LOGN";
                case TransferType.TypeMessage:
                    return "MESS";
                case TransferType.TypeSwitch:
                    return "SWTH";
                case TransferType.TypeUserProfile:
                    return "PROF";
                case TransferType.TypeGameControll:
                    return "GAME";
                default:
                    return "UNKN";
            }
        }

        public static TransferType GetTransferType(string type_string) {
            switch (type_string) {
                case "LOGN":
                    return TransferType.TypeLogin;
                case "MESS":
                    return TransferType.TypeMessage;
                case "SWTH":
                    return TransferType.TypeSwitch;
                case "PROF":
                    return TransferType.TypeUserProfile;
                case "GAME":
                    return TransferType.TypeGameControll;
                default:
                    return TransferType.TypeUnknown;
            }
        }
        
        public static byte[] Data2byte(MyData data, string Key)
        {
            /*
            //var commandData = new byte[4];//协议命令只占4位
            var commandData = Encoding.UTF8.GetBytes(Key);//协议命令只占4位,如果占的位数长过协议，那么协议解析肯定会出错的

            byte[] dataBody = Encoding.UTF8.GetBytes(JsonUntity.Object2Json(data));
            var dataLen = BitConverter.GetBytes(dataBody.Length);//int类型占4位，根据协议这里也只能4位，否则会出错


            var sendData = new byte[8 + dataBody.Length];//命令加内容长度为8

            // +-------+---+-------------------------------+
            // |request| l |                               |
            // | name  | e |    request body               |
            // |  (4)  | n |                               |
            // |       |(4)|                               |
            // +-------+---+-------------------------------+

            Array.ConstrainedCopy(commandData, 0, sendData, 0, 4);
            Array.ConstrainedCopy(dataLen, 0, sendData, 4, 4);
            Array.ConstrainedCopy(dataBody, 0, sendData, 8, dataBody.Length);

            return sendData;
            */

            //var commandData = new byte[4];//协议命令只占4位
            var commandData = Encoding.UTF8.GetBytes(Key);//协议命令只占4位,如果占的位数长过协议，那么协议解析肯定会出错的

            byte[] dataBody = Data2byte(data);

            var dataLen = BitConverter.GetBytes(dataBody.Length);//int类型占4位，根据协议这里也只能4位，否则会出错
            var sendData = new byte[8 + dataBody.Length];//命令加内容长度为8

            // +-------+---+-------------------------------+
            // |request| l |                               |
            // | name  | e |    request body               |
            // |  (4)  | n |                               |
            // |       |(4)|                               |
            // +-------+---+-------------------------------+

            Array.ConstrainedCopy(commandData, 0, sendData, 0, 4);
            Array.ConstrainedCopy(dataLen, 0, sendData, 4, 4);
            Array.ConstrainedCopy(dataBody, 0, sendData, 8, dataBody.Length);

            return sendData;
        }

        public static byte[] Data2byte(MyData data)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(JsonUntity.Object2Json(data));
            using (MemoryStream ms = new MemoryStream())
            {
                GZipOutputStream compressedzipStream = new GZipOutputStream(ms);

                compressedzipStream.Write(rawData, 0, rawData.Length);
                compressedzipStream.Close();
                byte[] dataBody = ms.ToArray();
                ms.Close();
                return dataBody;
            }
        }

        public static MyData Unpack(byte[] bytes)
        {
            using (MemoryStream raw = new MemoryStream(bytes))
            {
                GZipInputStream zipFile = new GZipInputStream(raw);
                using (MemoryStream re = new MemoryStream(1000))
                {
                    int count;
                    byte[] data = new byte[1000];
                    while ((count = zipFile.Read(data, 0, data.Length)) != 0)
                    {
                        re.Write(data, 0, count);
                    }
                    byte[] overarr = re.ToArray();
                    re.Close();
                    zipFile.Close();
                    raw.Close();

                    //将byte数组转为string
                    string result = Encoding.UTF8.GetString(overarr);
                    return JsonUntity.Json2Object<MyData>(result);
                }
            }

            /*
            string str = Encoding.UTF8.GetString(bytes);
            return JsonUntity.Json2Object<MyData>(str);
            */
        }
    }
}
