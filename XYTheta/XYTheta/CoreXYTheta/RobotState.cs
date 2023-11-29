using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using XYTheta.Display;

namespace XYTheta.CoreXYTheta
{
    [DebuggerDisplay("Θ: {ΘDegrees}   ({X}, {Y})   (L: {Datapoint.LeftPosition}, R: {Datapoint.RightPosition})")]
    public readonly struct RobotState
    {
        public static RobotState BaseState { get; } = new(
            angle: CoreConsts.StartingTheta,
            x: (int)(CoreConsts.StartingXCm * CoreConsts.DegreesPerCm),
            y: (int)(CoreConsts.StartingYCm * CoreConsts.DegreesPerCm),
            datapoint: default);

        public Datapoint Datapoint { get; }
        public decimal X { get; } // in degrees
        public decimal Y { get; } // in degrees
        public decimal Theta { get; } // in radians
        public decimal ThetaDegrees => Theta * 180 / (decimal)Math.PI;


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

            decimal deltaTheta = (deltaL - deltaR) / CoreConsts.RobotDiameterDeg;
            Theta = previousState.Theta + deltaTheta;

            decimal deltaCenter = (deltaL + deltaR) / 2M;
            decimal deltaX = (decimal)Math.Cos((double)Theta) * deltaCenter;
            decimal deltaY = (decimal)Math.Sin((double)Theta) * deltaCenter;

            X = previousState.X - deltaX;
            Y = previousState.Y - deltaY;
        }

        public static List<RobotState> ParseEncoderData(string encoderDataFilePath, Robot.Modes mode) => ParseEncoderData(encoderDataFilePath, BaseState, mode);
        public static List<RobotState> ParseEncoderData(string encoderDataFilePath, RobotState baseState, Robot.Modes mode)
        {
            if (mode != Robot.Modes.Playback) return new List<RobotState>() { baseState };

            var datapointsToRead = File.ReadAllLines(encoderDataFilePath);
            var states = new List<RobotState>(datapointsToRead.Length + 1)
            {
                baseState
            };

            for (int i = 0; i < datapointsToRead.Length; i++)
            {
                string[] data = datapointsToRead[i].Split(',');
                states.Add(new(states[i], new Datapoint(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]))));
            }

            return states;
        }
    }
}