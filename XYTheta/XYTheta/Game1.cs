using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XYTheta.Display;

namespace XYTheta
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Texture2D robotTexture;
        Texture2D fieldTexture;

        readonly Rectangle fieldRectangle;

        KeyboardState previousKeyboardState;

        Robot robot;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            fieldRectangle = new Rectangle(0, 0, DisplayConsts.FieldLengthPx, DisplayConsts.FieldHeightPx);

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

            robotTexture = Content.Load<Texture2D>("wonkypope");
            fieldTexture = Content.Load<Texture2D>("field");

            robot = new Robot(@"..\..\..\CoreXYTheta\EncoderData.txt", robotTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

            robot.Update(keyboardState, previousKeyboardState);

            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(fieldTexture, fieldRectangle, Color.White);
            robot.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}