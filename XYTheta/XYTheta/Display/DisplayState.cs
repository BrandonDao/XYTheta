﻿using Microsoft.Xna.Framework;
using System;
using XYTheta.CoreXYTheta;

namespace XYTheta.Display
{
    public readonly struct DisplayState
    {
        public static Vector2 RotationOrigin { get; } = new Vector2(DisplayConsts.RobotSizePx);

        public Rectangle DestinationRectangle { get; }
        public float Rotation { get; }

        public DisplayState(RobotState robotState)
        {
            DestinationRectangle = new Rectangle(robotState.X.ToDisplayX(), robotState.Y.ToDisplayY(), DisplayConsts.RobotSizePx, DisplayConsts.RobotSizePx);
            Rotation = (float)((float)robotState.Theta - (MathF.PI / 2));
        }
    }
}