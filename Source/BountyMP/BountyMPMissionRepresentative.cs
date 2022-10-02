using System;
using TaleWorlds.MountAndBlade;

namespace BountyMP
{
    public class BountyMPMissionRepresentative : MissionRepresentativeBase
    {
        public int Bounty { get; private set; }

        public Action<NetworkCommunicator, int> OnPeerBountyUpdated;

        public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            if (GameNetwork.IsClient)
            {
                GameNetwork.NetworkMessageHandlerRegisterer reg = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
                reg.Register<BountyMPPointsUpdateMessage>(HandleServerEventBountyPointsUpdate);
            }
        }

        private void HandleServerEventBountyPointsUpdate(BountyMPPointsUpdateMessage message)
        {
            var representative = message.NetworkCommunicator.GetComponent<MissionRepresentativeBase>();

            if (representative is BountyMPMissionRepresentative bountyRepresentative)
            {
                bountyRepresentative.UpdateBounty(message.Bounty);
                OnPeerBountyUpdated?.Invoke(message.NetworkCommunicator, message.Bounty);
            }
        }

        public void UpdateBounty(int bounty)
        {
            Bounty = bounty;
        }
    }
}
