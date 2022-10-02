using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace BountyMP
{
    public class MissionMultiplayerBountyMPMode : MissionBasedMultiplayerGameMode
    {
        public MissionMultiplayerBountyMPMode(string name) : base(name) { }

        public override void StartMultiplayerGame(string scene)
        {
            MissionState.OpenNew(
                "BountyMP",
                new MissionInitializerRecord(scene),
                missionController =>
                {
                    if (GameNetwork.IsServer)
                    {
                        return new MissionBehavior[]
                        {
                        MissionLobbyComponent.CreateBehavior(),
                        new MissionMultiplayerBountyMP(),
                        new MissionMultiplayerBountyMPClient(),
                        new MultiplayerTimerComponent(),
                        new MultiplayerMissionAgentVisualSpawnComponent(),
                        new ConsoleMatchStartEndHandler(),
                        new SpawnComponent(new BountyMPSpawnFrameBehavior(), new BountyMPSpawningBehavior()),
                        new MissionLobbyEquipmentNetworkComponent(),
                        new MultiplayerTeamSelectComponent(),
                        new MissionHardBorderPlacer(),
                        new MissionBoundaryPlacer(),
                        new MissionBoundaryCrossingHandler(),
                        new MultiplayerPollComponent(),
                        new MultiplayerAdminComponent(),
                        new MultiplayerGameNotificationsComponent(),
                        new MissionOptionsComponent(),
                        new MissionScoreboardComponent(new BountyMPScoreboardData()),
                        new MissionAgentPanicHandler(),
                        new AgentHumanAILogic(),
                        new EquipmentControllerLeaveLogic(),
                        new MultiplayerPreloadHelper(),
                        };
                    }
                    else
                    {
                        return new MissionBehavior[]
                        {
                        MissionLobbyComponent.CreateBehavior(),
                        new MissionMultiplayerBountyMPClient(),
                        new MultiplayerAchievementComponent(),
                        new MultiplayerTimerComponent(),
                        new MultiplayerMissionAgentVisualSpawnComponent(),
                        new ConsoleMatchStartEndHandler(),
                        new MissionLobbyEquipmentNetworkComponent(),
                        new MultiplayerTeamSelectComponent(),
                        new MissionHardBorderPlacer(),
                        new MissionBoundaryPlacer(),
                        new MissionBoundaryCrossingHandler(),
                        new MultiplayerPollComponent(),
                        new MultiplayerGameNotificationsComponent(),
                        new MissionOptionsComponent(),
                        new MissionScoreboardComponent(new BountyMPScoreboardData()),
                        new MissionMatchHistoryComponent(),
                        new EquipmentControllerLeaveLogic(),
                        new MissionRecentPlayersComponent(),
                        new MultiplayerPreloadHelper(),
                        };
                    }
                }
            );
        }
    }
}
