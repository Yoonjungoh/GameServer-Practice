using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{

    class Packet
    {
        public ushort size;
        public ushort packetId;
    }
    class PlayerInfoReq : Packet
    {
        public long playerId;
    }
    class PlayerInfoOk : Packet // 서버에서 클라로 답변을 주는 친구
    {
        public int hp;
        public int attack;
    }
    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }
    class ServerSession : Session
    {
        //static unsafe void ToBytes(byte[] array, int offset, ulong value) // TryWriteBytes 버전 안맞을때 맞춰주려고 쓴 코든데 맞는지 모르겠당
        //{
        //    fixed (byte* ptr = &array[offset])
        //        *(ulong*)ptr = value;
        //}
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 }; // 패킷아이디 일단 임의 부여
            
            // 보낸다(서버랑 반대)
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);

                ushort count = 0;
                bool success = true;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
                count += 8;

                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);


                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
                if(success)
                    Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
