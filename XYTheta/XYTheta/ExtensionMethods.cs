namespace XYTheta
{
    public static class ExtensionMethods
    {
        public static int ToDisplayX(this decimal x)
             => (int)(x * Game1.FieldLengthPx / Robot.DegreesPerCm / Robot.FieldLengthCm);

        public static int ToDisplayY(this decimal y)
             => (int)(y * Game1.FieldHeightPx / Robot.DegreesPerCm / Robot.FieldLengthCm);
    }
}