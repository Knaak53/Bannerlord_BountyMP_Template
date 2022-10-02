using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BountyMP
{
    public class BountyMPVM : ViewModel
    {
        private MissionMultiplayerBountyMPClient _client;
        private readonly MissionMultiplayerGameModeBaseClient _gameMode;
        private bool _isMyRepresentativeAssigned;
        private MissionScoreboardComponent _scoreboardComponent;

        private const float UpdatePlayersDuration = 5.0f;
        private float _updatePlayersTimeElapsed = 0.0f;

        public BountyMPVM(MissionMultiplayerBountyMPClient client)
        {
            _client = client;
            _client.OnMyRepresentativeAssigned += OnMyRepresentativeAssigned;
            _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();

            _scoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();

            Players = new MBBindingList<BountyMPPlayerVM>();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            _client.OnMyRepresentativeAssigned -= OnMyRepresentativeAssigned;

            if (_isMyRepresentativeAssigned)
            {
                _client.MyRepresentative.OnPeerBountyUpdated -= OnPeerBountyUpdated;
            }
        }

        public void Tick(float dt)
        {
            if (_gameMode.CheckTimer(out int remainingTime, out int remainingWarningTime, false))
            {
                RemainingRoundTime = TimeSpan.FromSeconds(remainingTime).ToString("mm':'ss");
            }

            _updatePlayersTimeElapsed += dt;
            if (_updatePlayersTimeElapsed >= UpdatePlayersDuration)
            {
                InitializePlayers();
                _updatePlayersTimeElapsed = 0.0f;
            }
        }

        private void OnMyRepresentativeAssigned()
        {
            _client.MyRepresentative.OnPeerBountyUpdated += OnPeerBountyUpdated;

            _isMyRepresentativeAssigned = true;

            InitializePlayers();
        }

        private void OnPeerBountyUpdated(NetworkCommunicator peer, int newBounty)
        {
            var updatedPlayer = Players.FirstOrDefault(p => p.Peer.GetNetworkPeer() == peer);
            if (updatedPlayer != null)
            {
                updatedPlayer.Bounty = newBounty;
            }
        }

        private void InitializePlayers()
        {
            Players.Clear();

            var playerMissionPeer = GameNetwork.MyPeer?.GetComponent<MissionPeer>();

            BattleSideEnum firstSideToAdd = BattleSideEnum.Attacker;
            BattleSideEnum secondSideToAdd = BattleSideEnum.Defender;
            if (playerMissionPeer != null)
            {
                Team playersTeam = playerMissionPeer.Team;
                if (playersTeam != null)
                {
                    if (playersTeam.Side == BattleSideEnum.Defender)
                    {
                        firstSideToAdd = BattleSideEnum.Defender;
                        secondSideToAdd = BattleSideEnum.Attacker;
                    }
                }
            }

            var side = _scoreboardComponent.Sides.FirstOrDefault(s => s != null && s.Side == firstSideToAdd);
            if (side != null)
            {
                foreach (var player in side.Players)
                {
                    var bountyPlayer = new BountyMPPlayerVM(player, (int)side.Side);
                    bountyPlayer.Bounty = player.GetNetworkPeer().GetComponent<BountyMPMissionRepresentative>().Bounty;
                    Players.Add(bountyPlayer);
                }
            }

            side = _scoreboardComponent.Sides.FirstOrDefault(s => s != null && s.Side == secondSideToAdd);

            if (side != null)
            {
                foreach (var player in side.Players)
                {
                    var bountyPlayer = new BountyMPPlayerVM(player, (int)side.Side);
                    bountyPlayer.Bounty = player.GetNetworkPeer().GetComponent<BountyMPMissionRepresentative>().Bounty;
                    Players.Add(bountyPlayer);
                }
            }
        }

        #region Properties
        private bool _isEnabled;
        private string _remainingRoundTime;
        private MBBindingList<BountyMPPlayerVM> _players;

        [DataSourceProperty]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public string RemainingRoundTime
        {
            get { return _remainingRoundTime; }
            set
            {
                if (value != _remainingRoundTime)
                {
                    _remainingRoundTime = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<BountyMPPlayerVM> Players
        {
            get { return _players; }
            set
            {
                if (value != _players)
                {
                    _players = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }
        #endregion
    }
}
