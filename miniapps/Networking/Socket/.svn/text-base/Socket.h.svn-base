#ifndef __SOCKET_H__
#define __SOCKET_H__

#include <WinSock2.h>

#include <string>


typedef std::string String;


class Socket {
public:

  virtual ~Socket();
  Socket(const Socket&);
  Socket& operator=(Socket&);

  String ReceiveLine();
  String ReceiveBytes();

  void   Close();

  // The parameter of SendLine is not a const reference
  // because SendLine modifes the String passed.
  void   SendLine (String);

  // The parameter of SendBytes is a const reference
  // because SendBytes does not modify the String passed 
  // (in contrast to SendLine).
  void   SendBytes(const String&);

protected:
  friend class SocketServer;
  friend class SocketSelect;

  Socket(SOCKET s);
  Socket();


  SOCKET s_;

  int* refCounter_;

private:
  static void Start();
  static void End();
  static int  nofSockets_;
};

class SocketClient : public Socket {
public:
  SocketClient(const String& host, int port);
};


class SocketServer : public Socket {
public:
  SocketServer(int port, int connections);

  Socket* Accept();
};

// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winsock/wsapiref_2tiq.asp
class SocketSelect {
  public:
    SocketSelect(Socket const * const s1, Socket const * const s2);

    bool Readable(Socket const * const s);

  private:
    fd_set fds_;
}; 


#endif
