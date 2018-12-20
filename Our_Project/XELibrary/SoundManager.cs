using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace XELibrary
{
    public class SoundManager : Microsoft.Xna.Framework.GameComponent, ISoundManager
    {
        enum SoundType
        {
            None, Song, SoundEffect
        };

        private Dictionary<string, Song> songs;

        private Dictionary<string, SoundEffect> soundEffects;
        private List<SoundEffectInstance> playingInstances = new List<SoundEffectInstance>();

        public bool RepeatPlayList = true;
        private string[] playList;
        private bool playListPlaying = false;
        private int currentSong;
        
        public SoundManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ISoundManager), this);
        }
        
        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(string musicPath, string fxPath)
        {
            songs = Directory.EnumerateFiles(Path.Combine(Game.Content.RootDirectory, musicPath))
                .Where(f => Path.GetExtension(f).Equals(".xnb"))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToDictionary(f => f, f => Game.Content.Load<Song>(Path.Combine(musicPath, f)));

            soundEffects = Directory.EnumerateFiles(Path.Combine(Game.Content.RootDirectory, fxPath))
                .Where(f => Path.GetExtension(f).Equals(".xnb"))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToDictionary(f => f, f => Game.Content.Load<SoundEffect>(Path.Combine(fxPath, f)));
        }
        
        
        public override void Update(GameTime gameTime)
        {
            if (playListPlaying && MediaPlayer.State == MediaState.Stopped)
            {
                currentSong++;
                if (currentSong >= playList.Length && RepeatPlayList)
                {
                    StartPlayList(0);
                }
                else
                {
                    StopPlayList();
                }
            }

            foreach(SoundEffectInstance instance in playingInstances)
            {
                if (instance.State == SoundState.Stopped)
                {
                    instance.Dispose();
                }
            }

            playingInstances.RemoveAll(instance => instance.IsDisposed);

            base.Update(gameTime);
        }
        
        protected override void Dispose(bool disposing)
        {
            foreach (Song song in songs.Values)
            {
                song.Dispose();
            }

            foreach (SoundEffectInstance soundEffectInstance in playingInstances)
            {
                soundEffectInstance.Stop();
                soundEffectInstance.Dispose();
            }

            foreach (SoundEffect soundEffect in soundEffects.Values)
            {
                soundEffect.Dispose();
            }

            playList = null;
            songs = null;
            soundEffects = null;
            playingInstances = null;

            base.Dispose(disposing);
        }
                        
        public bool IsSongPlaying(string soundName)
        {
            Song song;
            if (songs.TryGetValue(soundName, out song))
            {
                return (MediaPlayer.Queue.ActiveSong.Equals(song) &&
                        MediaPlayer.State == MediaState.Playing);
            }

            return false;
        }
        
        public void Play(string soundName)
        {
            switch (GetSoundType(soundName))
            {
                case SoundType.Song:
                    MediaPlayer.Play(songs[soundName]);
                    break;
                case SoundType.SoundEffect:
                    SoundEffectInstance effectInstance = soundEffects[soundName].CreateInstance();
                    playingInstances.Add(effectInstance);
                    effectInstance.Play();
                    break;
                default:
                    break;
            }
        }
        
        public void PauseSong()
        {
            MediaPlayer.Pause();
        }
        
        public void ResumeSong()
        {
            MediaPlayer.Resume();
        }
        
        public void StopAll()
        {
            StopSong();

            foreach (SoundEffectInstance instance in playingInstances)
            {
                instance.Stop();
                instance.Dispose();
            }
            playingInstances.Clear();
        }
        
        public void StopSong()
        {
            MediaPlayer.Stop();
        }
        
        public void StartPlayList(string[] playList, int startIndex = 0)
        {
            this.playList = playList;
            StartPlayList(startIndex);
        }

        public void StartPlayList(int startIndex)
        {
            if (playList == null || playList.Length == 0)
                return;

            if (startIndex >= playList.Length)
                startIndex = 0;

            currentSong = startIndex;
            playListPlaying = true;
            Play(playList[currentSong]);
        }

        public void StopPlayList()
        {
            playListPlaying = false;
            StopSong();
        }

        private SoundType GetSoundType(string soundName)
        {
            if (songs.ContainsKey(soundName))
                return SoundType.Song;
            else if (soundEffects.ContainsKey(soundName))
                return SoundType.SoundEffect;
            else return SoundType.None;
        }
    }
}
