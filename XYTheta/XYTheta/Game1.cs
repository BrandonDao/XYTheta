using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XYTheta
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Texture2D circleTexture;

        Texture2D fieldTexture;
        readonly Rectangle fieldRectangle;

        public Game1()
        {
            Datapoint x;
            x.LeftPosition;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            fieldRectangle = new Rectangle(0, 0, 2790 / 2, 1350 / 2);

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

            circleTexture = Content.Load<Texture2D>("circle");
            fieldTexture = Content.Load<Texture2D>("field");
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(fieldTexture, fieldRectangle, Color.White);
            spriteBatch.Draw(circleTexture, new Rectangle(125 - 10, 600 - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}