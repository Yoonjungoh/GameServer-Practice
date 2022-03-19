using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;   // 읽는 커서, 처리한 데이터가 있으면 그 위치로 가서 자리잡음
        int _writePos;  // 쓰는 커서, 데이터가 들어오면 그만큼 밀림
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize],0, bufferSize);
            

        }
        public int DataSize { get { return _writePos-_readPos; } } // 실제 _read해서 처리해줘야할 데이터 사이즈
        public int FreeSize { get { return _buffer.Count - _writePos; } } // 버퍼에 남은 공간 말함


        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }
        public void Clean() // r이랑 w다시 처음으로 당겨주는 역할
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 시작 위치로 커서 옮기고 뒤에 처리해야할 데이터들은 복사해옴
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;

            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
