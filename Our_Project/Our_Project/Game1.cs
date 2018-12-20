using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace Our_Project
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        CelAnimationManager celAnimationManager;
       // ScrollingBackgroundManager scrollingBackgroundManager;
        InputHandler inputHandler;
        SoundManager soundManager;
        GameStateManager stateManager;

        public ITitleIntroState TitleIntroState;
        public IStartMenuState StartMenuState;
        public IPlayingState PlayingState;
        
        public IPausedState PausedState;
        public IOptionsMenuState OptionsMenuState;


        public bool EnableSoundFx { get; set; }
        public bool EnableMusic { get; set; }

        


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;

            inputHandler = new InputHandler(this);
            Components.Add(inputHandler);

            celAnimationManager = new CelAnimationManager(this, "Textures\\");
            Components.Add(celAnimationManager);

            soundManager = new SoundManager(this);
            Components.Add(soundManager);

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            PlayingState = new PlayingState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);

            EnableSoundFx = true;
            EnableMusic = true;
        }

        protected override void BeginRun()
        {
            stateManager.ChangeState(TitleIntroState.Value);

            base.BeginRun();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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


            // Load sounds
            string musicPath = @"Sounds\Music";
            string fxPath = @"Sounds\SoundFX\";

            soundManager.LoadContent(musicPath, fxPath);

          


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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            


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

            base.Draw(gameTime);


           
            spriteBatch.End();

            
        }
    }
}
