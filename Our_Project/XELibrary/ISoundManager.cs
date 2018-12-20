
namespace XELibrary
{
    public interface ISoundManager
    {
        bool IsSongPlaying(string soundName);
        void Play(string soundName);
        void PauseSong();
        void ResumeSong();
        void StopAll();
        void StopSong();
        void StartPlayList(string[] playList, int startIndex = 0);
        void StartPlayList(int startIndex);
        void StopPlayList();
    }
}
