namespace XYTheta.CoreXYTheta
{
    public readonly struct Datapoint
    {
        public int Time { get; }
        public int LeftPosition { get; }
        public int RightPosition { get; }

        public Datapoint(int time, int leftPos, int rightPos)
        {
            Time = time;
            LeftPosition = leftPos;
            RightPosition = rightPos;
        }
    }
}