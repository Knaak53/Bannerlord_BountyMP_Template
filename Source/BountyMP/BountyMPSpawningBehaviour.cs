using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace BountyMP
{
    public class BountyMPSpawningBehavior : SpawningBehaviorBase
    {
        public BountyMPSpawningBehavior()
        {
            IsSpawningEnabled = true;
        }

        public override void Initialize(SpawnComponent spawnComponent)
        {
            base.Initialize(spawnComponent);

            base.OnAllAgentsFromPeerSpawnedFromVisuals += OnAllAgentsFromPeerSpawnedFromVisuals;
        }

        public override void Clear()
        {
            base.Clear();

            base.OnAllAgentsFromPeerSpawnedFromVisuals -= OnAllAgentsFromPeerSpawnedFromVisuals;
        }

        public override void OnTick(float dt)
        {
            if (IsSpawningEnabled && _spawnCheckTimer.Check(Mission.CurrentTime))
            {
                SpawnAgents();
            }

            base.OnTick(dt);
        }

        protected override void SpawnAgents()
        {
            var cultureTeamOne = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
            var cultureTeamTwo = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());

            foreach (var networkPeer in GameNetwork.NetworkPeers)
            {
                if (!networkPeer.IsSynchronized)
                {
                    continue;
                }

                var missionPeer = networkPeer.GetComponent<MissionPeer>();

                if (missionPeer != null
                    && (missionPeer.ControlledAgent == null) // Agent has been spawned ..
                    && !missionPeer.HasSpawnedAgentVisuals // .. or its visuals have been spawned
                    && missionPeer.Team != null
                    && missionPeer.Team != Mission.SpectatorTeam
                    && missionPeer.SpawnTimer.Check(Mission.CurrentTime))
                {
                    BasicCultureObject teamCulture = missionPeer.Team.Side == BattleSideEnum.Attacker ? cultureTeamOne : cultureTeamTwo;

                    var mpClass = MultiplayerClassDivisions.GetMPHeroClassForPeer(missionPeer);
                    if (mpClass == null || mpClass.TroopCasualCost > GameMode.GetCurrentGoldForPeer(missionPeer))
                    {
                        if (missionPeer.SelectedTroopIndex != 0)
                        {
                            missionPeer.SelectedTroopIndex = 0; //Reset SelectedTroopIndex

                            GameNetwork.BeginBroadcastModuleEvent();
                            GameNetwork.WriteMessage(new NetworkMessages.FromServer.UpdateSelectedTroopIndex(networkPeer, 0));
                            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkPeer);
                        }
                    }
                    else
                    {
                        var character = mpClass.HeroCharacter;

                        Equipment equipment = character.Equipment.Clone();
                        var alternativeEquipments = MPPerkObject.GetOnSpawnPerkHandler(missionPeer)?.GetAlternativeEquipments(true);
                        if (alternativeEquipments != null)
                        {
                            foreach (var alternativeEquipment in alternativeEquipments)
                            {
                                equipment[alternativeEquipment.Item1] = alternativeEquipment.Item2;
                            }
                        }

                        var agentBuildData = new AgentBuildData(character);
                        agentBuildData.MissionPeer(missionPeer);
                        agentBuildData.Equipment(equipment);
                        agentBuildData.Team(missionPeer.Team);

                        agentBuildData.IsFemale(missionPeer.Peer.IsFemale);
                        agentBuildData.BodyProperties(GetBodyProperties(missionPeer,
                            missionPeer.Team == Mission.AttackerTeam ? cultureTeamOne : cultureTeamTwo));
                        agentBuildData.VisualsIndex(0);
                        agentBuildData.ClothingColor1(
                            missionPeer.Team == Mission.AttackerTeam ? teamCulture.Color : teamCulture.ClothAlternativeColor);
                        agentBuildData.ClothingColor2(missionPeer.Team == Mission.AttackerTeam
                            ? teamCulture.Color2
                            : teamCulture.ClothAlternativeColor2);
                        agentBuildData.TroopOrigin(new BasicBattleAgentOrigin(character));

                        if (GameMode.ShouldSpawnVisualsForServer(networkPeer))
                        {
                            AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(missionPeer, agentBuildData, missionPeer.SelectedTroopIndex);
                        }

                        GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData);
                    }
                }
            }
        }

        public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
        {
            return true;
        }

        // TODO_KORNEEL GetRespawnPeriod will never be used, even in Duel, TDM, FFA, because there is always at least an attacker team
        public override int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
        {
            if (GameMode.WarmupComponent != null && GameMode.WarmupComponent.IsInWarmup)
            {
                return MultiplayerWarmupComponent.RespawnPeriodInWarmup;
            }
            else
            {
                if (peer.Team != null)
                {
                    if (peer.Team.Side == BattleSideEnum.Attacker)
                    {
                        return MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetIntValue();
                    }
                    else if (peer.Team.Side == BattleSideEnum.Defender)
                    {
                        return MultiplayerOptions.OptionType.RespawnPeriodTeam2.GetIntValue();
                    }
                }

                return -1;
            }
        }

        protected override bool IsRoundInProgress()
        {
            return Mission.Current.CurrentState == Mission.State.Continuing;
        }

        private new void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
        {
            Team team = peer.Team;
            bool isTeamOne = team == Mission.AttackerTeam;
            bool isTeamTwo = team == Mission.DefenderTeam;

            var teamCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(isTeamOne
                ? (MultiplayerOptions.OptionType.CultureTeam1.GetStrValue())
                : (MultiplayerOptions.OptionType.CultureTeam2.GetStrValue()));

            var mpClass = MultiplayerClassDivisions.GetMPHeroClasses(teamCulture).ElementAt(peer.SelectedTroopIndex);

            GameMode.ChangeCurrentGoldForPeer(peer, GameMode.GetCurrentGoldForPeer(peer) - mpClass.TroopCasualCost);
        }
    }
}
