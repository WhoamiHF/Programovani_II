using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode;

namespace Dave
{
    public class Class1
    {
        public class Dave : AdvancedRobot
        {
            public override void Run()
            {
               SetTurnRightRadians(double.PositiveInfinity);
            }
           
            public override void OnScannedRobot(ScannedRobotEvent e)
            {
                double RadarTurn = Heading + e.Bearing -RadarHeading;
                SetTurnRightRadians(RadarTurn);
                SetAhead(50);
            }
        }
    }
}
