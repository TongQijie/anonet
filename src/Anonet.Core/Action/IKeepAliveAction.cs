﻿namespace Anonet.Core
{
    interface IKeepAliveAction
    {
        void Heartbeat(bool isRequest);
    }
}