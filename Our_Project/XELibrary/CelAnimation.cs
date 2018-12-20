
namespace XELibrary
{
    /// <summary>
    /// A grid-based representation of cels in an animation.
    /// The number of frames is the number of rows times the number of columns.
    /// </summary>
    public struct CelCount
    {
        public int NumberOfColumns;
        public int NumberOfRows;
        public CelCount(int numberOfColumns, int numberOfRows)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
        }
    }

    /// <summary>
    /// A pixel range-based representation of the cels of the animation in a sprite sheet.
    /// </summary>
    public struct CelRange
    {
        public int FirstCelX;
        public int FirstCelY;
        public int LastCelX;
        public int LastCelY;
        
        public CelRange(int firstCelX, int firstCelY, int lastCelX, int lastCelY)
        {
            FirstCelX = firstCelX;
            FirstCelY = firstCelY;
            LastCelX = lastCelX;
            LastCelY = lastCelY;
        }
    }

    /// <summary>
    /// A cel animation object contains the data on how a spritesheet is used for cel animation.
    /// Does not include the actual sprite sheet (texture) data 
    /// </summary>
    public class CelAnimation
    {
        private string textureName;
        private CelRange celRange;
        private int framesPerSecond;
        private float timePerFrame;
        public float TotalElapsedTime = 0.0f;
        public int CelWidth;
        public int CelHeight;
        public int NumberOfCels;
        public int CelsPerRow;
        public int Frame;
        public bool Paused = false;

        public CelAnimation(string textureName, CelRange celRange, int framesPerSecond)
        {
            this.textureName = textureName;
            this.celRange = celRange;
            this.framesPerSecond = framesPerSecond;
            this.timePerFrame = 1.0f / (float)framesPerSecond;
            this.Frame = 0;
        }

        public string TextureName
        {
            get { return (textureName); }
        }
        public CelRange CelRange
        {
            get { return (celRange); }
        }
        public int FramesPerSecond
        {
            get { return (framesPerSecond); }
        }
        public float TimePerFrame
        {
            get { return (timePerFrame); }
        }
    }
}
