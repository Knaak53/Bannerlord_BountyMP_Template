using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace BountyMP
{
    public class BountyMPPlayerVM : MPPlayerVM
    {
        public BountyMPPlayerVM(MissionPeer peer, int teamIndex) : base(peer)
        {

            TeamIndex = teamIndex;
        }

        #region Properties
        private int _bounty;
        private int _teamIndex;

        [DataSourceProperty]
        public int Bounty
        {
            get { return _bounty; }
            set
            {
                if (value != _bounty)
                {
                    _bounty = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public int TeamIndex
        {
            get { return _teamIndex; }
            set
            {
                if (value != _teamIndex)
                {
                    _teamIndex = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }
        #endregion
    }
}
