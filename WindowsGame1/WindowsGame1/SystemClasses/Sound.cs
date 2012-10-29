using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{
    class Sound
    {
        private SoundEffect effect;

        void LoadSound(String soundname, ContentManager Content)
        {
            effect = Content.Load<SoundEffect>("sounds/" + soundname);
        }

        void PlaySound()
        {
            effect.Play();
        }

        void PlaySound(float volume, float pitch, float pan)
        {
            effect.Play(volume, pitch, pan);
        }
    }
}
