using System;

namespace XYTheta.CoreXYTheta
{
    public static class CoreConsts
    {
        public const decimal StartingXCm = 23;
        public const decimal StartingYCm = 90;
        public const decimal StartingTheta = (decimal)Math.PI / 2M;

        public const decimal RobotRadiusDeg = 127.21875M; // in degrees
        public const decimal RobotDiameterDeg = 254.4375M; // in degrees

        public const decimal DegreesPerCm = 13.3275007291M;

        public const decimal FieldLengthCm = 236.2M;
        public const decimal FieldHeightCm = 114.3M;
    }
}