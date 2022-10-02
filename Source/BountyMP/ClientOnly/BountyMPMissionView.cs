using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace BountyMP
{
    [ViewCreatorModule]
    public class BountyMPMissionView
    {
        [ViewMethod("BountyMP")]
        public static MissionView[] OpenBountyMPMission(Mission mission)
        {
            try
            {
                List<MissionView> missionViews = new List<MissionView>();

                missionViews.Add(ViewCreator.CreateMissionServerStatusUIHandler());
                missionViews.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
                missionViews.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
                missionViews.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
                missionViews.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
                missionViews.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
                missionViews.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
                missionViews.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("BountyMP"));
                missionViews.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
                missionViews.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
                missionViews.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
                missionViews.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
                missionViews.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
                missionViews.Add(ViewCreator.CreatePollProgressUIHandler());
                missionViews.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
                missionViews.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler());
                missionViews.Add(ViewCreator.CreateOptionsUIHandler());
                missionViews.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
                missionViews.Add(ViewCreator.CreateMissionBoundaryCrossingView());
                missionViews.Add(new MissionBoundaryWallView());
                missionViews.Add(new MissionItemContourControllerView());
                missionViews.Add(new MissionAgentContourControllerView());
                missionViews.Add(new MissionGauntletBountyMPUI());
                return missionViews.ToArray();
            }
            catch (Exception e) {

                string fileName = @"C:\logs\log.txt";

                    // Check if file already exists. If yes, delete it.     
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    // Create a new file     
                    using (FileStream fs = File.Create(fileName))
                    {
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes(e.StackTrace);
                        fs.Write(title, 0, title.Length);
                    }

                    // Open the stream and read it back.    
                    using (StreamReader sr = File.OpenText(fileName))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            Console.Write(e.StackTrace);
                        }
                    }
                return null;
            }
        }
    }
}
  