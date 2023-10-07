using System.IO;

namespace XYTheta
{
    public class Robot
    {
        public const float CenterToWheelDistance = 127.21875f;
        public const float WheelDistance = 254.4375f;

        public Datapoint[] Datapoints { get; private set; }


        public Robot(string encoderDataFilePath)
        {
            var datapointsToRead = File.ReadAllLines(encoderDataFilePath);
            Datapoints = new Datapoint[datapointsToRead.Length];

            for (int i = 0; i < datapointsToRead.Length; i++)
            {
                string[] data = datapointsToRead[i].Split(',');

                Datapoints[i] = new Datapoint(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]));
            }
        }

        public void UpdateState()
    }
}