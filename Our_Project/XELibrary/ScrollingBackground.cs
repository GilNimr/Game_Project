using Microsoft.Xna.Framework;

namespace XELibrary
{
    public class ScrollingBackground
    {
        public Rectangle SourceRect;
        public string TextureName;
        public Vector2 Position;
        public float ScrollRateRatio;
        public float LayerDepth;
        public float ScrollRate;
        public Color Color;
        
        public bool Positive
        {
            get { return (ScrollRate > 0); }
        }
        
        public ScrollingBackground(string textureName, Vector2 position, Rectangle sourceRect, float scrollRateRatio, float layerDepth, Color color)
        {
            TextureName     = textureName;
            Position        = position;
            SourceRect      = sourceRect;
            ScrollRateRatio = scrollRateRatio;
            LayerDepth      = layerDepth;
            Color           = color;
        }
    }
}
