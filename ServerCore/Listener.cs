using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수(초과하면 fail뜸)
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);   // 최초로 한번은 실행해줌
            
        }
        void RegisterAccept(SocketAsyncEventArgs args)   // 등록한다
        {
            args.AcceptSocket = null;
            
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)  // 완료됐다는 말(운좋았음, 실행하는 동시에 클라이언트가 접속했다는 말임)
                OnAcceptCompleted(null, args);
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)    // 완료 됐을때
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);   // 다음 유저를 위해 등록하는 거임
        }
        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
