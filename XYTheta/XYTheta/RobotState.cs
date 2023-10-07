using System;

namespace XYTheta
{
    public struct RobotState
    {
        Datapoint Datapoint { get; }
        public float X { get; }
        public float Y { get; }
        public float Theta { get; }

        public RobotState(RobotState previousState, Datapoint datapoint)
        {
            Datapoint = datapoint;

            int deltaL = datapoint.LeftPosition - previousState.Datapoint.LeftPosition;
            int deltaR = datapoint.RightPosition - previousState.Datapoint.RightPosition;

            float deltaθ = (float)((deltaL - deltaR) * 180 / Math.PI / Robot.WheelDistance);

            float deltaCenter = (deltaL + deltaR) >> 2;
            float deltaX = (float)(Math.Cos(deltaθ) * deltaCenter);
            float deltaY = (float)(Math.Sin(deltaθ) * deltaCenter);

            X = previousState.X + deltaX;
            Y = previousState.Y + deltaY;
            Theta = previousState.Theta + deltaθ;
        }
    }
}