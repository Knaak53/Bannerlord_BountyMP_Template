using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ModuleManager;

namespace BountyMP
{
    public class BountyMPSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddMultiplayerGameMode(new MissionMultiplayerBountyMPMode("BountyMP"));
            Console.WriteLine("BountyMP mode added");
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.GameTextManager.LoadGameTexts();
        }
    }
}
