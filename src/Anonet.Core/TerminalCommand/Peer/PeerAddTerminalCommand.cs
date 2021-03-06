﻿using System;

namespace Anonet.Core
{
    [TerminalCommand(new string[] { "peeradd" })]
    class PeerAddTerminalCommand : TerminalCommandBase
    {
        public PeerAddTerminalCommand(TerminalCommandLine terminalCommandLine)
            : base(terminalCommandLine)
        {
        }

        public override void Execute(ITerminalCommandChannel terminalCommandChannel, Action<string> prompt)
        {
            if (terminalCommandChannel is NetworkPeerManager)
            {
                Handled = true;

                var networkPeerManager = terminalCommandChannel as NetworkPeerManager;

                var networkPeerType = TerminalCommandLine["type"];
                if (networkPeerType == null
                    || !(networkPeerType.Equals("normal", StringComparison.OrdinalIgnoreCase)
                    || networkPeerType.Equals("tracker", StringComparison.OrdinalIgnoreCase)))
                {
                    Result = TerminalCommandResult.InvalidArguments();
                    return;
                }

                var ipEndPointString = TerminalCommandLine["ep"];
                if (ipEndPointString == null)
                {
                    Result = TerminalCommandResult.InvalidArguments();
                    return;
                }

                var ipEndPoint = IPEndPointHelper.ConvertFromString(ipEndPointString);
                if (ipEndPoint == null)
                {
                    Result = TerminalCommandResult.InvalidArguments();
                    return;
                }

                var networkPoint = new NetworkPointBase(ipEndPoint);
                foreach (var peer in networkPeerManager.Peers.GetAll())
                {
                    if (peer.NetworkConnection.NetworkPoints.Exists(networkPoint))
                    {
                        return;
                    }
                }

                if (networkPeerType.Equals("normal", StringComparison.OrdinalIgnoreCase))
                {
                    networkPeerManager.Peers.Add(new NormalNetworkPeerBase(null, new INetworkPoint[] { networkPoint }));
                }
                else if (networkPeerType.Equals("tracker", StringComparison.OrdinalIgnoreCase))
                {
                    networkPeerManager.Peers.Add(new TrackerNetworkPeerBase(null, new INetworkPoint[] { networkPoint }));
                }

                Result = TerminalCommandResult.Done();
            }
        }
    }
}
