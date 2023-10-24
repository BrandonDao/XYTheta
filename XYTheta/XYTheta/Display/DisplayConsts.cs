using XYTheta.CoreXYTheta;

namespace XYTheta.Display
{
    public static class DisplayConsts
    {
        public const int FieldLengthPx = 2790 >> 1; // original resolution divided by 2
        public const int FieldHeightPx = 1350 >> 1;

        public const int RobotSizePx = 100;

        public static int ToDisplayX(this decimal x)
             => (int)(x * FieldLengthPx / CoreConsts.DegreesPerCm / CoreConsts.FieldLengthCm);

        public static int ToDisplayY(this decimal y)
             => (int)(y * FieldHeightPx / CoreConsts.DegreesPerCm / CoreConsts.FieldHeightCm);
    }
}