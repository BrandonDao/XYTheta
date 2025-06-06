﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        public List<int> WaypointIndices { get; private set; }
        public RobotState CurrentState => States[currentStateIndex];
        private int currentStateIndex;

        public Modes Mode { get; set; }

        public Texture2D Texture { get; private set; }
        public Texture2D WaypointTexture { get; private set; }

        private readonly Rectangle sourceRectangle;
        private readonly List<DisplayState> displayStates;
        private readonly List<DisplayState> waypointDisplayStates;

        private const int fastStateIncrement = 2;

        private readonly TimeSpan continuousArrowKeyDelay = TimeSpan.FromMilliseconds(500);

        private readonly Color WaypointColor = new(200, 200, 200, 150);

        private TimeSpan playbackTimer;
        private TimeSpan continuousArrowKeyTimer;

        private bool isPaused;
        private bool isShowingAllWaypoints;


        public Robot(string encoderDataFilePath, Texture2D texture, Texture2D waypointTexture, Modes mode)
        {
            Texture = texture;
            WaypointTexture = waypointTexture;
            Mode = mode;
            States = RobotState.ParseEncoderData(encoderDataFilePath, mode);
            WaypointIndices = new List<int>();
            displayStates = new List<DisplayState>(States.Count);
            waypointDisplayStates = new List<DisplayState>();
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
        public string[] ExportWaypoints()
        {
            string[] output = new string[WaypointIndices.Count * 3];

            int outputIndex = 0;
            for (int i = 0; i < WaypointIndices.Count; i++)
            {
                output[outputIndex++] = States[WaypointIndices[i]].X.ToString();
                output[outputIndex++] = Math.Abs(States[WaypointIndices[i]].Y - DisplayConsts.FieldHeightPx).ToString();
                output[outputIndex++] = (States[WaypointIndices[i]].Theta - (decimal)(Math.PI / 2)).ToString();
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
                case Modes.VirtualControl: UpdateVirtualControl(keyboardState, previousKeyboardState); break;
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
        private void UpdateVirtualControl(KeyboardState keyboardState, KeyboardState previousKeyboardState)
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

            if(keyboardState.IsKeyDown(Keys.F) && previousKeyboardState.IsKeyUp(Keys.F))
            {
                WaypointIndices.Add(currentStateIndex);
                waypointDisplayStates.Add(new DisplayState(States[currentStateIndex], DisplayConsts.WaypointSizePx));
            }
            
            isShowingAllWaypoints = keyboardState.IsKeyDown(Keys.Tab);

            // always storing current position even if not moving to preserve pauses in the recording
            States.Insert(currentStateIndex + 1, new RobotState(States[currentStateIndex], new Datapoint(time: (int)playbackTimer.TotalMilliseconds, States[currentStateIndex].Datapoint.LeftPosition + deltaLeft, States[currentStateIndex].Datapoint.RightPosition + deltaRight)));
            currentStateIndex++;
            displayStates.Insert(currentStateIndex, new DisplayState(States[currentStateIndex]));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: Texture,
                destinationRectangle: displayStates[currentStateIndex].DestinationRectangle,
                sourceRectangle: sourceRectangle,
                color: Color.White,
                rotation: displayStates[currentStateIndex].Rotation,
                origin: DisplayConsts.RotationOriginPx,
                effects: SpriteEffects.None,
                layerDepth: 0);

            if(isShowingAllWaypoints)
            {
                for (int i = 0; i < waypointDisplayStates.Count; i++)
                {
                    spriteBatch.Draw(
                        texture: WaypointTexture,
                        destinationRectangle: waypointDisplayStates[i].DestinationRectangle,
                        sourceRectangle: sourceRectangle,
                        color: WaypointColor,
                        rotation: waypointDisplayStates[i].Rotation,
                        origin: DisplayConsts.RotationOriginPx,
                        effects: SpriteEffects.None,
                        layerDepth: 0);

                    if (i == 0) continue;

                    spriteBatch.DrawLine(
                        waypointDisplayStates[i - 1].DestinationRectangle.X,
                        waypointDisplayStates[i - 1].DestinationRectangle.Y,
                        waypointDisplayStates[i].DestinationRectangle.X,
                        waypointDisplayStates[i].DestinationRectangle.Y,
                        Color.Red,
                        thickness: 3);
                }
            }
            else
            {
                for (int i = waypointDisplayStates.Count - 1; i > waypointDisplayStates.Count - 4; i--)
                {
                    if (i < 0) break;

                    spriteBatch.Draw(
                        texture: WaypointTexture,
                        destinationRectangle: waypointDisplayStates[i].DestinationRectangle,
                        sourceRectangle: sourceRectangle,
                        color: WaypointColor,
                        rotation: waypointDisplayStates[i].Rotation,
                        origin: DisplayConsts.RotationOriginPx,
                        effects: SpriteEffects.None,
                        layerDepth: 0);

                    if (i == 0) continue;

                    spriteBatch.DrawLine(
                        waypointDisplayStates[i - 1].DestinationRectangle.X,
                        waypointDisplayStates[i - 1].DestinationRectangle.Y,
                        waypointDisplayStates[i].DestinationRectangle.X,
                        waypointDisplayStates[i].DestinationRectangle.Y,
                        Color.Red,
                        thickness: 3);
                }
            }
        }
    }
}