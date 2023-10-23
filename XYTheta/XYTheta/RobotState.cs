using System;
using System.Diagnostics;

namespace XYTheta
{
    [DebuggerDisplay("Θ: {ΘDegrees}   ({X}, {Y})   (L: {Datapoint.LeftPosition}, R: {Datapoint.RightPosition})")]
    public readonly struct RobotState
    {
        public static RobotState BaseState { get; }
            = new(
                angle: (decimal)(Math.PI / 2d),
                x: (int)(23 * Robot.DegreesPerCm),
                y: (int)(185 * Robot.DegreesPerCm),
                new Datapoint(time: 0, leftPos: 0, rightPos: 0));
        
        public Datapoint Datapoint { get; }
        public decimal X { get; } // in degrees
        public decimal Y { get; } // in degrees
        public decimal Theta { get; } // in radians
        public int ΘDegrees => (int)(Theta * 180 / (decimal)Math.PI);

        // 13.3275007291 degrees per centimeter
        // field is 236.2 cm in length (3147.95567221 degrees)
        // field is 114.3 cm in height (1523.33333334 degrees)

        private RobotState(decimal angle, int x, int y, Datapoint datapoint)
        {
            Datapoint = datapoint;
            X = x;
            Y = y;
            Theta = angle;
        }

        public RobotState(RobotState previousState, Datapoint datapoint)
        {
            Datapoint = datapoint;

            int deltaL = datapoint.LeftPosition - previousState.Datapoint.LeftPosition;
            int deltaR = datapoint.RightPosition - previousState.Datapoint.RightPosition;

            decimal deltaθ = (deltaL - deltaR) / Robot.WheelDistance;
            Theta = previousState.Theta + deltaθ;

            decimal deltaCenter = (deltaL + deltaR) / 2M;
            decimal deltaX = (decimal)Math.Cos((double)Theta) * deltaCenter;
            decimal deltaY = (decimal)Math.Sin((double)Theta) * deltaCenter;

            X = previousState.X - deltaX;
            Y = previousState.Y + -deltaY;
        }
    }
}