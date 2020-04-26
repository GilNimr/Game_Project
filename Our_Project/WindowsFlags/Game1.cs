
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Shared1;
using MonoGame.Shared1.States_and_state_related;
using System.Collections.Generic;
using XELibrary;


namespace WindowFlags
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        readonly CelAnimationManager celAnimationManager;
        readonly InputHandler inputHandler;
        SoundManager soundManager;
        GameStateManager stateManager;



        public ITitleIntroState TitleIntroState; // the first state user see
        public IStartMenuState StartMenuState;  // the state with 'play'
        public IChooseFlagState ChooseFlagState;  // the state with 'play'
        public IPlayingState PlayingState; // the game
        public IPlacingSoldiersState PlacingSoldiersState;
        public IPausedState PausedState;  // not implemented
        public IOptionsMenuState OptionsMenuState; // not implemented
        public IBuildingBoardState BuildingBoardState;


        public bool EnableSoundFx { get; set; }
        public bool EnableMusic { get; set; }

        public static int screen_width;
        public static int screen_height;
        public static float FontScale;
        public static SpriteFont font30static;
        public SpriteFont font30;
        public SpriteFont font300;
        public Texture2D button_texture;
        private ParticleService particleService;
        public List<string> countryList;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);


            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //full screen.
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            //windowed screen.
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1600;

            screen_height = graphics.PreferredBackBufferHeight;
            screen_width = graphics.PreferredBackBufferWidth;

            FontScale = Game1.screen_height * 0.00055f; // for scaling our fonts to different screens.

            inputHandler = new InputHandler(this);
            Components.Add(inputHandler);

            celAnimationManager = new CelAnimationManager(this, "Textures\\Animations");
            Components.Add(celAnimationManager);

            soundManager = new SoundManager(this , 1);
            Components.Add(soundManager);

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);



            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            ChooseFlagState = new ChooseFlagState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);
            BuildingBoardState = new BuildingBoardState(this,1);
            PlacingSoldiersState = new PlacingSoldiersState(this);
            PlayingState = new PlayingState(this);

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
            particleService = new ParticleService(this, GraphicsDevice);
            Components.Add(particleService);

            // Load sounds
            string musicPath = @"Sounds\Music";
            string fxPath = @"Sounds\SoundFX\";
            soundManager.LoadContent(musicPath, fxPath);

            CelCount celCount = new CelCount(5, 25);

           /* celAnimationManager.AddAnimation("Aura", "Aura", celCount, 10);
            celAnimationManager.ResumeAnimation("Aura");*/

            countryList = new List<string>()
           {
               "Australia","Brazil","Canada","China","Ethiopia","France","India","Israel",
               "Italy","Jamaica","Japan","Macedonia","North Korea","Philippines","South Africa",
               "South Korea","Switzerland","Turkey","USA"
           };

            foreach (var country in countryList)
            {
                celCount = new CelCount(5, 25);
                celAnimationManager.AddAnimation(country, country, celCount, 10);
                celAnimationManager.ResumeAnimation(country);
            }

         
            font30 = Content.Load<SpriteFont>(@"Fonts\KaushanScript30");
            font30static = font30;
            font300 = Content.Load<SpriteFont>(@"Fonts\KaushanScript300");
            button_texture = Content.Load<Texture2D>(@"Textures\Controls\Button1");

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

        //translates 2d world coordinates to isometric screen coordinates.
        public static Vector2 TwoD2isometrix(int x, int y)
        {
            int tmpx = x - y;
            int tmpy = (x + y) / 2;
            return new Vector2(tmpx, tmpy);
        }

        //translates 2d world coordinates to isometric screen coordinates.
        public static Point TwoD2isometrix(Point xy)
        {
            int tmpx = xy.X - xy.Y;
            int tmpy = (xy.X + xy.Y) / 2;
            return new Point(tmpx, tmpy);
        }

        //translates isometric screen coordiantes to 2d world coordinates
        public static Vector2 Isometrix2twoD(int x, int y)
        {
            int tmpx = (2 * y + x) / 2;
            int tmpy = (2 * y - x) / 2;
            return new Vector2(tmpx, tmpy);
        }

        //translates isometric screen coordiantes to 2d world coordinates
        public static Point Isometrix2twoD(Point xy)
        {
            int tmpx = (2 * xy.Y + xy.X) / 2;
            int tmpy = (2 * xy.Y - xy.X) / 2;
            return new Point(tmpx, tmpy);
        }
    }
}
