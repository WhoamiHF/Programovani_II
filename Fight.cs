using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode;

namespace Dave
{
    public class FollowingBot

    {
        public class Dave : AdvancedRobot
        {
            public override void Run()
            {
               SetTurnRightRadians(double.PositiveInfinity);
               
            }
           
            public override void OnScannedRobot(ScannedRobotEvent e)
            {
                

                if (e.Distance <= 100)
                {
                    Fire(3);
                }
                else
                {
                    SetAhead(50);
                    Fire(1);
                }
                double RadarTurn = e.Bearing;
                SetTurnRightRadians(RadarTurn);
            }
        }
    }
}
