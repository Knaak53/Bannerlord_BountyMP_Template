using System;
using TaleWorlds.TwoDimension;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace BountyMP
{
    public class MissionGauntletBountyMPUI : MissionView
    {
        private BountyMPVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private SpriteCategory _mphudCategory;

        private MissionMultiplayerBountyMPClient _client;

        private MissionLobbyComponent _lobbyComponent;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            ViewOrderPriority = 15;

            _client = Mission.GetMissionBehavior<MissionMultiplayerBountyMPClient>();

            _dataSource = new BountyMPVM(_client);
            _gauntletLayer = new GauntletLayer(ViewOrderPriority);
            _gauntletLayer.LoadMovie("BountyMP", _dataSource);

            var spriteData = UIResourceManager.SpriteData;
            var resourceContext = UIResourceManager.ResourceContext;
            var resourceDepot = UIResourceManager.UIResourceDepot;
            _mphudCategory = spriteData.SpriteCategories["ui_mpmission"];
            _mphudCategory.Load(resourceContext, resourceDepot);

            MissionScreen.AddLayer(_gauntletLayer);

            _lobbyComponent = Mission.GetMissionBehavior<MissionLobbyComponent>();
            _lobbyComponent.OnPostMatchEnded += OnPostMatchEnded;

            _dataSource.IsEnabled = true;
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            MissionScreen.RemoveLayer(_gauntletLayer);
            _mphudCategory?.Unload();
            _dataSource.OnFinalize();
            _dataSource = null;
            _gauntletLayer = null;

            _lobbyComponent.OnPostMatchEnded -= OnPostMatchEnded;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            _dataSource.Tick(dt);
        }

        private void OnPostMatchEnded()
        {
            _dataSource.IsEnabled = false;
        }
    }
}
