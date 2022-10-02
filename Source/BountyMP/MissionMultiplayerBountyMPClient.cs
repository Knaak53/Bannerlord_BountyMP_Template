using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using NetworkMessages.FromServer;

namespace BountyMP
{
    public class MissionMultiplayerBountyMPClient : MissionMultiplayerGameModeBaseClient
    {
        public override bool IsGameModeUsingGold => true;
        public override bool IsGameModeTactical => false;
        public override bool IsGameModeUsingRoundCountdown => false;
        public override bool IsGameModeUsingAllowCultureChange => false;
        public override bool IsGameModeUsingAllowTroopChange => false;
        public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;

        public BountyMPMissionRepresentative MyRepresentative { get; private set; }
        public Action OnMyRepresentativeAssigned;

        private void OnMyClientSynchronized()
        {
            MyRepresentative = GameNetwork.MyPeer.GetComponent<BountyMPMissionRepresentative>();
            MyRepresentative?.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
            OnMyRepresentativeAssigned?.Invoke();
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

            MissionNetworkComponent.OnMyClientSynchronized += OnMyClientSynchronized;
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            MissionNetworkComponent.OnMyClientSynchronized -= OnMyClientSynchronized;
            MyRepresentative?.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
        }

        public override void AfterStart()
        {
            Mission.SetMissionMode(MissionMode.Battle, true);
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (GameNetwork.IsClient)
            {
                registerer.Register<NetworkMessages.FromServer.SyncGoldsForSkirmish>(HandleServerEventUpdateGold);
            }
        }

        private void HandleServerEventUpdateGold(NetworkMessages.FromServer.SyncGoldsForSkirmish message)
        {
            var representative = message.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
            OnGoldAmountChangedForRepresentative(representative, message.GoldAmount);
        }

        public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
        {
            if (representative != null && MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
            {
                representative.UpdateGold(goldAmount);
            }
        }

        public override int GetGoldAmount()
        {
            return MyRepresentative?.Gold ?? 0;
        }
    }
}
