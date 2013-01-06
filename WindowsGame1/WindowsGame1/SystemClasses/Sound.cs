using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class Sound
    {
        private List<SoundEffect> sfx;
        private List<Song> music;

        void LoadSound(String soundname, ContentManager Content)
        {
            SoundEffect effect = Content.Load<SoundEffect>("sounds/" + soundname);
            sfx.Add(effect);
        }
        void LoadMusic(String musicname, ContentManager Content)
        {
            Song song = Content.Load<Song>("music/" + musicname);
            music.Add(song);
        }

        void PlaySound(String name)
        {
            SoundEffect effect = FindSfx(name);
            if (effect != null)
                effect.Play();
            else
                Console.WriteLine("Couldn't find that Soundeffect!");
        }

        void PlaySound(String name, float volume, float pitch, float pan)
        {
            SoundEffect effect = FindSfx(name);
            if (effect != null)
                effect.Play(volume, pitch, pan);
            else
                Console.WriteLine("Couldn't find that Soundeffect!");
        }

        void PlayMusic(String name)
        {
            Song song = FindSong(name);
            if (song != null)
                MediaPlayer.Play(song);
            else
                Console.WriteLine("Couldn't find that Piece Of Music!");
        }

        SoundEffect FindSfx(String name)
        {
            foreach (SoundEffect fx in sfx)
            {
                if (fx.Name == name)
                    return fx;
            }

            return null;
        }

        Song FindSong(String name)
        {
            foreach (Song song in music)
            {
                if (song.Name == name)
                    return song;
            }

            return null;
        }
    }
}
