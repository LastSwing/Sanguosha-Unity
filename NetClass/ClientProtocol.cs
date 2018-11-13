using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace CommonClassLibrary
{
    public class MsgBinaryRequestInfo : IPackageInfo<string>
    {
        public MsgBinaryRequestInfo(string key, byte[] body)
        {
            Key = key;
            Body = body;
        }

        public string Key { get; protected set; }

        public byte[] Body { get; protected set; }
    }

    public class MsgFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<MsgBinaryRequestInfo>
    {

        public MsgFixedHeaderReceiveFilter() : base(8)
        {
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            byte[] headData = new byte[8];
            bufferStream.Read(headData, 0, 8);

            byte[] length1 = new byte[4];
            Array.Copy(headData, 4, length1, 0, 4);
            
            return BitConverter.ToInt32(length1, 0);
        }

        public override MsgBinaryRequestInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] headData = new byte[8];
            bufferStream.Read(headData, 0, 8);

            var length1 = new byte[4];
            Array.Copy(headData, 4, length1, 0, 4);

            int bodyLength = BitConverter.ToInt32(length1, 0);

            var bodyData = new byte[bodyLength];
            bufferStream.Read(bodyData, 0, bodyLength);
;

            MsgBinaryRequestInfo requestInfo = new MsgBinaryRequestInfo(Encoding.UTF8.GetString(headData, 0, 4), bodyData);
            return requestInfo;
        }
    }
}