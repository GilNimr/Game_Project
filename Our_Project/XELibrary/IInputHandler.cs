using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XELibrary
{
    public interface IInputHandler
    {
        // Keyboard state
        KeyboardHandler KeyboardHandler { get; }

#if !XBOX360
        MouseHandler MouseHandler { get; }
#endif
    }
}
