#include "Socket.h"

#include <iostream>

using namespace std;


int main() {

    SocketClient s("127.0.0.1", 8002);

	s.SendBytes("202Hello");


    //while (1) {
    //  string l = s.ReceiveLine();
    //  if (l.empty()) break;
    //  cout << l;
    //  cout.flush();
    //}


  //int bla = 0;
  //cin >> bla;

  return 0;
}





