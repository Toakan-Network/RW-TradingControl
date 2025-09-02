using System;
using System.Collections.Generic;
using TradingControl.functions;
using Verse;
using Verse.AI.Group;

namespace TradingControl.definitions
{

    public class Marketplace : Building
    {
        public SharedActions Actions = new SharedActions();
        private int _count = 0;

        public Marketplace()
        {
            Actions.CheckForPreviousSpot(this);
        }

        protected override void Tick()
        {
            base.Tick();
            if (_count++ > 1)
            {
                _count = 0;
                List<Lord> lords = Map.lordManager.lords;
                Actions.GoToTradeSpot(Position, lords);
            }
        }
    }


    public class TradingSpot : Building
    {
        public SharedActions Actions = new SharedActions();
        private int _count = 0;

        public TradingSpot()
        {
            Actions.CheckForPreviousSpot(this);
        }

        protected override void Tick()
        {
            base.Tick();
            if (_count++ > 1)
            {
                _count = 0;
                List<Lord> lords = Map.lordManager.lords;
                Actions.GoToTradeSpot(Position, lords);
            }

        }
    }

    public class DropSpotIndicator : Building
    {
        public SharedActions Actions = new SharedActions();

        public DropSpotIndicator()
        {
            Actions.CheckForPreviousSpot(this);
        }
    }
}

