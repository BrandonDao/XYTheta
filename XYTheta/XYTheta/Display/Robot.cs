using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using XYTheta.CoreXYTheta;

namespace XYTheta.Display
{
    public class Robot
    {
        public enum Modes
        {
            Playback,
            VirtualControl
        }

        public List<RobotState> States { get; private set; }
        public RobotState CurrentState => States[currentStateIndex];
        private int currentStateIndex;

        public Modes Mode { get; private set; }

        public Texture2D Texture { get; private set; }

        private readonly Rectangle sourceRectangle;
        private readonly List<DisplayState> displayStates;

        private const int fastStateIncrement = 2;

        private readonly TimeSpan continuousArrowKeyDelay = TimeSpan.FromMilliseconds(500);

        private TimeSpan playbackTimer;
        private TimeSpan continuousArrowKeyTimer;

        private bool isPaused;


        public Robot(string encoderDataFilePath, Texture2D texture, Modes mode)
        {
            Texture = texture;
            Mode = mode;
            States = RobotState.ParseEncoderData(encoderDataFilePath, mode);
            displayStates = new List<DisplayState>(States.Count);
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            for (int i = 0; i < displayStates.Capacity; i++)
            {
                displayStates.Add(new DisplayState(States[i]));
            }

            isPaused = false;
        }

        public string[] ExportDatapointsAsCSV()
        {
            string[] output = new string[States.Count];

            for (int i = 0; i < States.Count; i++)
            {
                output[i] = $"{States[i].Datapoint.Time},{States[i].Datapoint.LeftPosition},{States[i].Datapoint.RightPosition}";
            }
            return output;
        }

        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState, TimeSpan elapsedGameTime)
        {
            playbackTimer += elapsedGameTime;
            continuousArrowKeyTimer += elapsedGameTime;

            switch (Mode)
            {
                case Modes.Playback: UpdatePlayback(keyboardState, previousKeyboardState); break;
                case Modes.VirtualControl: UpdateVirtualControl(keyboardState); break;
            }
        }

        private void UpdatePlayback(KeyboardState keyboardState, KeyboardState previousKeyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                isPaused = !isPaused;
            }

            if (isPaused)
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                {
                    continuousArrowKeyTimer = TimeSpan.Zero;
                    return;
                }

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    if (continuousArrowKeyTimer < continuousArrowKeyDelay && previousKeyboardState.IsKeyDown(Keys.Left)) return;

                    if(keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                    {
                        currentStateIndex -= fastStateIncrement;
                    }
                    else
                    {
                        currentStateIndex--;
                    }

                    if (currentStateIndex < 0)
                    {
                        currentStateIndex = States.Count;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    if (continuousArrowKeyTimer < continuousArrowKeyDelay && previousKeyboardState.IsKeyDown(Keys.Right)) return;

                    if(keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                    {
                        currentStateIndex += fastStateIncrement;
                    }
                    else
                    {
                        currentStateIndex++;
                    }

                    if (currentStateIndex >= States.Count)
                    {
                        currentStateIndex = 0;
                    }
                }
            }
            else
            {
                if (playbackTimer.TotalMilliseconds < States[currentStateIndex].Datapoint.Time) return;

                currentStateIndex++;

                if (currentStateIndex >= States.Count)
                {
                    currentStateIndex = 0;
                    playbackTimer = TimeSpan.Zero;
                }
            }

        }
        private void UpdateVirtualControl(KeyboardState keyboardState)
        {
            int deltaLeft = 0;
            int deltaRight = 0;

            if (keyboardState.IsKeyDown(Keys.LeftControl)) return;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                deltaLeft = 10;
                deltaRight = 10;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                deltaLeft = -10;
                deltaRight = -10;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                deltaLeft -= 5;
                deltaRight += 5;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                deltaLeft += 5;
                deltaRight -= 5;
            }

            // always storing current position even if not moving to preserve pauses in the recording
            States.Add(new RobotState(States[currentStateIndex], new Datapoint(time: (int)playbackTimer.TotalMilliseconds, States[currentStateIndex].Datapoint.LeftPosition + deltaLeft, States[currentStateIndex].Datapoint.RightPosition + deltaRight)));
            currentStateIndex++;
            displayStates.Add(new DisplayState(States[currentStateIndex]));
        }

        public void Draw(SpriteBatch spriteBatch)
            => spriteBatch.Draw(
                    texture: Texture,
                    destinationRectangle: displayStates[currentStateIndex].DestinationRectangle,
                    sourceRectangle: sourceRectangle,
                    color: Color.White,
                    rotation: displayStates[currentStateIndex].Rotation,
                    origin: DisplayConsts.RotationOriginPx,
                    effects: SpriteEffects.None,
                    layerDepth: 0);

    }
}