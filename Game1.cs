using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.VectorDraw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WindTest.Util;

namespace WindTest
{
    public class Game1 : Game
    {
        public static readonly Random RNG = new Random();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;

        private Crawler[] crawlers;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            crawlers = new Crawler[100];
            for (int i = 0; i < crawlers.Length; i++) crawlers[i] = new Crawler(new Vector2(RNG.Next(10) - 5, RNG.Next(10) - 5), RNG.Next(5) + 5);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var crawler in crawlers) crawler.Move(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var project = Matrix.Identity;
            var view = Matrix.CreateTranslation(-Vector3.UnitX * .5f - Vector3.UnitY * .5f) * Matrix.CreateScale(.05f);

            _primitiveBatch.Begin(ref view, ref project);
            var drawing = new PrimitiveDrawing(_primitiveBatch);
            foreach (var crawler in crawlers) crawler.Draw(drawing);
            _primitiveBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}