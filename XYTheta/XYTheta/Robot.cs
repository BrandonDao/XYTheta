using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace XYTheta
{
    public class Robot
    {
        public const decimal FieldLengthCm = 236.2M;
        public const decimal FieldHeightCm = 114.3M;

        public const decimal CenterToWheelDistance = 127.21875M; // in degrees
        public const decimal WheelDistance = 254.4375M; // in degrees
        public const decimal DegreesPerCm = 13.3275007M;

        public RobotState[] States { get; private set; }
        public RobotState CurrentState => States[currentStateIndex];
        private int currentStateIndex;

        private readonly Rectangle[] drawRectangles;

        public Texture2D Texture { get; private set; }

        public Robot(string endcoderDataFilePath, Texture2D texture)
            : this(endcoderDataFilePath, RobotState.BaseState, texture)
        { }

        public Robot(string encoderDataFilePath, RobotState baseState, Texture2D texture)
        {
            Texture = texture;

            var datapointsToRead = File.ReadAllLines(encoderDataFilePath);
            States = new RobotState[datapointsToRead.Length + 1];
            drawRectangles = new Rectangle[States.Length];

            States[0] = baseState;

            for (int i = 0; i < datapointsToRead.Length; i++)
            {
                drawRectangles[i] = new Rectangle(States[i].X.ToDisplayX(), States[i].Y.ToDisplayY(), Game1.RobotSizePx, Game1.RobotSizePx);
                
                string[] data = datapointsToRead[i].Split(',');
                States[i + 1] = new(States[i], new Datapoint(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2])));
            }
            drawRectangles[^1] = new Rectangle(States[^1].X.ToDisplayX(), States[^1].Y.ToDisplayY(), Game1.RobotSizePx, Game1.RobotSizePx);
        }

        public void UpdateState()
        {
            currentStateIndex++;

            if(currentStateIndex >= States.Length)
            {
                currentStateIndex = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
            =>  spriteBatch.Draw(
                    texture: Texture,
                    destinationRectangle: drawRectangles[currentStateIndex],
                    sourceRectangle: new Rectangle(0, 0, Texture.Width, Texture.Height),
                    color: Color.White,
                    rotation: (float)((float)States[currentStateIndex].Theta - Math.PI / 2),
                    origin: new Vector2(100, 100),
                    effects: SpriteEffects.None,
                    layerDepth: 0);

    }
}