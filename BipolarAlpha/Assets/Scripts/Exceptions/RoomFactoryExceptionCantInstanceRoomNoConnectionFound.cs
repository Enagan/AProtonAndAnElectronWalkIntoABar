using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RoomFactoryExceptionCantInstanceRoomNoConnectionFound : BipolarException
{
  public RoomFactoryExceptionCantInstanceRoomNoConnectionFound(string message) : base(message) { }
}

