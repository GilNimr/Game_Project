using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Graphics;

namespace Our_Project
{
    public class ParticleService : GameComponent
    {
        public ParticleEffect _particleEffect;
        private Texture2D _particleTexture;

        public ParticleService(Game game, GraphicsDevice graphicsDevice) : base(game)
        {
            game.Services.AddService(typeof(ParticleService), this);
            _particleTexture = new Texture2D(graphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });

            ParticleInit(new TextureRegion2D(_particleTexture));
        }
        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
         

            _particleEffect.Update(deltaTime);
           

           

            base.Update(gameTime);
        }
        public void Trigger(Vector2 pos)
        {
            _particleEffect.Trigger(pos);
        }
        private void ParticleInit(TextureRegion2D textureRegion)
        {
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = new Vector2(400, 240),
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 5000, TimeSpan.FromSeconds(2.5),
                       // Profile.Ring(4f, Profile.CircleRadiation.In))
                    Profile.Line(new Vector2(0,1),Game1.screen_height/20))
                    {
                        AutoTrigger=false,
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 1000,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = new HslColor(180f, 0.5f, 0.5f),
                                        EndValue = new HslColor(190f, 0.9f, 1.0f)
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 80, Height = 800},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f}
                        }
                    }
                }
            };
        }
    }
}
