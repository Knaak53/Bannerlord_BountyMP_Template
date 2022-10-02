using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BountyMP
{
    public class BountyMPScoreboardData : IScoreboardData
    {
        public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
        {
            return new []
            {
                new MissionScoreboardComponent.ScoreboardHeader("avatar", missionPeer => "", bot => ""),
                new MissionScoreboardComponent.ScoreboardHeader("name", (missionPeer) => missionPeer.GetComponent<MissionPeer>().DisplayedName, bot => new TextObject("{=hvQSOi79}Bot").ToString()),
                new MissionScoreboardComponent.ScoreboardHeader("bounty", missionPeer => missionPeer.GetComponent<BountyMPMissionRepresentative>().Bounty.ToString(), bot => 0.ToString()),
            };
        }
    }
}
