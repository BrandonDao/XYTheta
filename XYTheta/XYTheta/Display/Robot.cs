using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using XYTheta.CoreXYTheta;

namespace XYTheta.Display
{
    public class Robot
    {
        public RobotState[] States { get; private set; }
        public RobotState CurrentState => States[currentStateIndex];
        private int currentStateIndex;

        public Texture2D Texture { get; private set; }

        private readonly Rectangle sourceRectangle;
        private readonly DisplayState[] displayStates;


        public Robot(string encoderDataFilePath, Texture2D texture)
        {
            Texture = texture;            
            States = RobotState.ParseEncoderData(encoderDataFilePath);
            displayStates = new DisplayState[States.Length];
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            for (int i = 0; i < displayStates.Length; i++)
            {
                displayStates[i] = new DisplayState(States[i]);
            }
        }

        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (keyboardState.IsKeyUp(Keys.Space) || previousKeyboardState.IsKeyDown(Keys.Space)) return;

            currentStateIndex++;

            if (currentStateIndex >= States.Length)
            {
                currentStateIndex = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
            => spriteBatch.Draw(
                    texture: Texture,
                    destinationRectangle: displayStates[currentStateIndex].DestinationRectangle,
                    sourceRectangle: sourceRectangle,
                    color: Color.White,
                    rotation: displayStates[currentStateIndex].Rotation,
                    origin: DisplayState.RotationOrigin,
                    effects: SpriteEffects.None,
                    layerDepth: 0);

    }
}