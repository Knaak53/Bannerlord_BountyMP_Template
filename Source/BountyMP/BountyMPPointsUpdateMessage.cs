using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace BountyMP
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class BountyMPPointsUpdateMessage : GameNetworkMessage
    {
        public int Bounty { get; private set; }
        public NetworkCommunicator NetworkCommunicator { get; private set; }

        public BountyMPPointsUpdateMessage() { }

        public BountyMPPointsUpdateMessage(BountyMPMissionRepresentative representative, int bounty)
        {
            Bounty = bounty;
            NetworkCommunicator = representative.GetNetworkPeer();
        }

        protected override void OnWrite()
        {
            WriteIntToPacket(Bounty, CompressionMatchmaker.ScoreCompressionInfo);
            WriteNetworkPeerReferenceToPacket(NetworkCommunicator);
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            Bounty = ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref bufferReadValid);
            NetworkCommunicator = ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.GameMode;
        }

        protected override string OnGetLogFormat()
        {
            return "BountyMP Point Update";
        }
    }
}
