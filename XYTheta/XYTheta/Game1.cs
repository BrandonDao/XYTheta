using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using XYTheta.Display;

namespace XYTheta
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D robotTexture;
        Texture2D fieldTexture;
        Texture2D blankTexture;

        readonly Rectangle fieldRectangle;

        KeyboardState previousKeyboardState;

        Robot robot;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            fieldRectangle = new Rectangle(
                x: 0,
                y: 0,
                width: DisplayConsts.FieldLengthPx,
                height: DisplayConsts.FieldHeightPx);

            graphics.PreferredBackBufferWidth = fieldRectangle.Width;
            graphics.PreferredBackBufferHeight = fieldRectangle.Height;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blankTexture = new Texture2D(GraphicsDevice, width: 1, height: 1);
            blankTexture.SetData(new Color[] { Color.Red });

            robotTexture = Content.Load<Texture2D>("PopeMobile");
            fieldTexture = Content.Load<Texture2D>("field");

            robot = new Robot(@"..\..\..\CoreXYTheta\EncoderData.txt", robotTexture, Robot.Modes.Playback);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

            robot.Update(keyboardState, previousKeyboardState, gameTime.ElapsedGameTime);

            if (previousKeyboardState.IsKeyUp(Keys.S) && keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.S))
            {
                File.WriteAllLines("VirtualControlLog.csv", robot.ExportDatapointsAsCSV());
            }

            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(fieldTexture, fieldRectangle, Color.White);

            robot.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}