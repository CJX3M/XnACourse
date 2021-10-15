using System;
using System.Collections.Generic;
// Adding linq for collection/enumeration manipulation
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game objects. Using inheritance would make this
        // easier, but inheritance isn't a GDD 1200 topic
        Burger burger;
        List<TeddyBear> bears = new List<TeddyBear>();
        static List<Projectile> projectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();
        List<Message> messages = new List<Message>();

        // projectile and explosion sprites. Saved so they don't have to
        // be loaded every time projectiles or explosions are created
        static Texture2D frenchFriesSprite;
        static Texture2D teddyBearProjectileSprite;
        static Texture2D explosionSpriteStrip;

        // scoring support
        int score = 0;
        string scoreString = GameConstants.ScorePrefix + 0;

        // health support
        string healthString = GameConstants.HealthPrefix +
            GameConstants.BurgerInitialHealth;
        bool burgerDead = false;

        // text display support
        SpriteFont font;

        // sound effects
        SoundEffect burgerDamage;
        SoundEffect burgerDeath;
        SoundEffect burgerShot;
        SoundEffect explosion;
        SoundEffect teddyBounce;
        SoundEffect teddyShot;

        // teddy bear took damage
        bool burgerTookDamage = false;
        bool killedTeddy = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
            graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RandomNumberGenerator.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load audio content
            burgerShot = Content.Load<SoundEffect>(@"audio\BurgerShot");
            teddyShot = Content.Load<SoundEffect>(@"audio\TeddyShot");
            burgerDamage = Content.Load<SoundEffect>(@"audio\BurgerDamage");
            teddyBounce = Content.Load<SoundEffect>(@"audio\TeddyBounce");
            burgerDeath = Content.Load<SoundEffect>(@"audio\BurgerDeath");
            explosion = Content.Load<SoundEffect>(@"audio\Explosion");
            // load sprite font
            font = Content.Load<SpriteFont>(@"fonts\Arial20");
            // load projectile and explosion sprites
            frenchFriesSprite = Content.Load<Texture2D>(@"graphics\frenchfries");
            teddyBearProjectileSprite = Content.Load<Texture2D>(@"graphics\teddybearprojectile");
            explosionSpriteStrip = Content.Load<Texture2D>(@"graphics\explosion");
            // add initial game objects
            burger = new Burger(Content, @"graphics\burger", GameConstants.WindowWidth / 2, (int) (GameConstants.WindowHeight * 7/8), burgerShot);
            // set initial health and score strings
            messages.Add(new Message(healthString, font, GameConstants.HealthLocation));
            messages.Add(new Message(scoreString, font, GameConstants.ScoreLocation));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            killedTeddy = false;
            burgerTookDamage = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // get current mouse state and update burger
            burger.Update(gameTime, Mouse.GetState(), Keyboard.GetState());
            // update other game objects
            foreach (TeddyBear bear in bears)
            {
                bear.Update(gameTime);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // check and resolve collisions between teddy bears
            for (int i = 0; i < bears.Count; i++)
            {
                TeddyBear teddy1 = bears[i];
                if (teddy1.Active)
                {
                    for (int j = 0; i != j && j < bears.Count; j++)
                    {
                        TeddyBear teddy2= bears[j];
                        if (teddy2.Active)
                        {
                            CollisionResolutionInfo colResol = CollisionUtils.CheckCollision(gameTime.ElapsedGameTime.Milliseconds,
                                graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth,
                                teddy1.Velocity, teddy1.CollisionRectangle,
                                teddy2.Velocity, teddy2.CollisionRectangle);
                            if (colResol == null)
                                continue;
                            if (colResol.FirstOutOfBounds)
                            {
                                teddy1.Active = false;
                            }
                            else
                            {
                                teddy1.DrawRectangle = colResol.FirstDrawRectangle;
                                teddy1.Velocity = colResol.FirstVelocity;
                            }
                            if (colResol.SecondOutOfBounds)
                            {
                                teddy2.Active = false;
                            }
                            else
                            {
                                teddy2.DrawRectangle = colResol.SecondDrawRectangle;
                                teddy2.Velocity = colResol.SecondVelocity;
                            }
                        }
                    }
                }
            }
            // check and resolve collisions between burger and teddy bears
            if (!burgerDead)
            {
                foreach (TeddyBear bear in bears)
                {
                    if (burger.CollisionRectangle.Intersects(bear.CollisionRectangle))
                    {
                        burger.Health -= GameConstants.BearDamage;
                        bear.Active = false;
                        Explosion expl = new Explosion(explosionSpriteStrip, bear.DrawRectangle.Center.X, bear.DrawRectangle.Center.Y, explosion);
                        explosions.Add(expl);
                        burgerTookDamage = true;
                    }
                }
            }

            // check and resolve collisions between burger and projectiles
            if (!burgerDead)
            {
                foreach (Projectile proj in projectiles.Where(p => p.Type == ProjectileType.TeddyBear && p.Active))
                {
                    if (burger.CollisionRectangle.Intersects(proj.CollisionRectangle))
                    {
                        proj.Active = false;
                        burger.Health -= GameConstants.TeddyBearProjectileDamage;
                        Explosion expl = new Explosion(explosionSpriteStrip, proj.CollisionRectangle.Center.X, proj.CollisionRectangle.Center.Y, explosion);
                        burgerDamage.Play();
                        burgerTookDamage = true;
                    }
                }
            }

            if(burgerTookDamage)
            {
                healthString = string.Format("{0} {1}", GameConstants.HealthPrefix, burger.Health);
                messages[0].Text = healthString;
                CheckBurgerKill();
            }

            // check and resolve collisions between teddy bears and projectiles
            foreach (TeddyBear bear in bears.Where(b => b.Active))
            {
                foreach (Projectile proj in projectiles.Where(p => p.Type == ProjectileType.FrenchFries && p.Active))
                {
                    if(bear.CollisionRectangle.Intersects(proj.CollisionRectangle))
                    {
                        bear.Active = false;
                        proj.Active = false;
                        Explosion expl = new Explosion(explosionSpriteStrip, bear.DrawRectangle.Center.X, bear.DrawRectangle.Center.Y, explosion);
                        explosions.Add(expl);
                        killedTeddy = true;
                        score += GameConstants.BearPoints;
                    }
                }
            }

            if(killedTeddy)
            {
                scoreString = string.Format("{0} {1}", GameConstants.ScorePrefix, score);
                messages[1].Text = scoreString;
            }
            // clean out inactive teddy bears and add new ones as necessary
            for(int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                    bears.RemoveAt(i);
            }
            // clean out inactive projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (!projectiles[i].Active)
                    projectiles.RemoveAt(i);
            }

            // clean out finished explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (explosions[i].Finished)
                    explosions.RemoveAt(i);
            }
            // Spawn new bear whenever there's less than MaxBears
            if (bears.Count < GameConstants.MaxBears)
            {
                SpawnBear();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw game objects
            burger.Draw(spriteBatch);
            foreach (TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            // draw score and health
            foreach(Message msg in messages)
            {
                msg.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public methods

        /// <summary>
        /// Gets the projectile sprite for the given projectile type
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <returns>the projectile sprite for the type</returns>
        public static Texture2D GetProjectileSprite(ProjectileType type)
        {
            // replace with code to return correct projectile sprite based on projectile type
            Texture2D returnSprite = null;
            switch (type)
            {
                case ProjectileType.FrenchFries:
                    returnSprite = frenchFriesSprite;
                    break;
                case ProjectileType.TeddyBear:
                    returnSprite = teddyBearProjectileSprite;
                    break;
                default:
                    break;
            }
            return returnSprite;
        }

        /// <summary>
        /// Adds the given projectile to the game
        /// </summary>
        /// <param name="projectile">the projectile to add</param>
        public static void AddProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        private void SpawnBear()
        {
            // generate random location
            int x = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.SpawnBorderSize);
            int y = GetRandomLocation(GameConstants.SpawnBorderSize, GameConstants.SpawnBorderSize);
            // generate random velocity
            float speed = GameConstants.MinBearSpeed + RandomNumberGenerator.NextFloat(GameConstants.BearSpeedRange);
            // generate randome angle
            float angle = RandomNumberGenerator.NextFloat((float)Math.PI*2);
            Vector2 vector = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
            // create new bear
            TeddyBear newBear = new TeddyBear(Content, @"graphics\teddybear", x, y, vector, teddyBounce, teddyShot);
            // make sure we don't spawn into a collision
            var collisionRectagles = GetCollisionRectangles();
            // If the bear collisions, wait for the rest of the object to move and don't spawn a new bear
            if (!CollisionUtils.IsCollisionFree(newBear.CollisionRectangle, GetCollisionRectangles()))
            {
                newBear = null;
                return;
            }
            // add new bear to list
            bears.Add(newBear);
        }

        /// <summary>
        /// Gets a random location using the given min and range
        /// </summary>
        /// <param name="min">the minimum</param>
        /// <param name="range">the range</param>
        /// <returns>the random location</returns>
        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        /// <summary>
        /// Gets a list of collision rectangles for all the objects in the game world
        /// </summary>
        /// <returns>the list of collision rectangles</returns>
        private List<Rectangle> GetCollisionRectangles()
        {
            List<Rectangle> collisionRectangles = new List<Rectangle>();
            collisionRectangles.Add(burger.CollisionRectangle);
            foreach (TeddyBear bear in bears)
            {
                collisionRectangles.Add(bear.CollisionRectangle);
            }
            foreach (Projectile projectile in projectiles)
            {
                collisionRectangles.Add(projectile.CollisionRectangle);
            }
            foreach (Explosion explosion in explosions)
            {
                collisionRectangles.Add(explosion.CollisionRectangle);
            }
            return collisionRectangles;
        }

        /// <summary>
        /// Checks to see if the burger has just been killed
        /// </summary>
        private void CheckBurgerKill()
        {
            if (burger.Health == 0)
            {
                burgerDead = true;
                burgerDeath.Play();
            }
        }

        #endregion
    }
}
