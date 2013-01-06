using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace WindowsGame1
{
    public class Sound
    {
        private List<SoundEffect> sfx;
        public List<Song> music;

        public Sound(String gamepath, ContentManager myContent)
        {
            sfx = new List<SoundEffect>();
            music = new List<Song>();

            // Load all the music and sfx files that are in the content pipeline

                                                          // TODO: V HIER DIE ZAHL MUSS WENN AUF RELEASE GESTELLT WIRD GEÄNDERT WERDEN 
            String path = gamepath.Substring(0, gamepath.Length - 14) + "Content\\music\\";
            Console.WriteLine(path);
            foreach (string f in Directory.GetFiles(path))
            {
                string filename = f.Substring(f.LastIndexOf(@"\") + 1);
                filename = filename.Substring(0, filename.Length - 4);
                Song newsong = myContent.Load<Song>("music\\" + filename);
                Console.WriteLine(filename);
            }

        }

        public void LoadSound(String soundname, ContentManager Content)
        {
            SoundEffect effect = Content.Load<SoundEffect>("sounds/" + soundname);
            sfx.Add(effect);
        }
        public void LoadMusic(String musicname, ContentManager Content)
        {
            Song song = Content.Load<Song>("music/" + musicname);
            music.Add(song);
        }

        public void PlaySound(String name)
        {
            SoundEffect effect = FindSfx("sound/" + name);
            if (effect != null)
                effect.Play();
            else
                Console.WriteLine("Couldn't find that Soundeffect!");
        }

        public void PlaySound(String name, float volume, float pitch, float pan)
        {
            SoundEffect effect = FindSfx(name);
            if (effect != null)
                effect.Play(volume, pitch, pan);
            else
                Console.WriteLine("Couldn't find that Soundeffect!");
        }

        public void PlayMusic(String name)
        {
            Song song = FindSong("music\\" + name);
            if (song != null)
                MediaPlayer.Play(song);
            else
                Console.WriteLine("Couldn't find that Piece Of Music!");
        }

        public void PlayMusic(Song song)
        {
            MediaPlayer.Play(song);
        }

        public void StopMusic() { MediaPlayer.Stop(); }

        public void PauseMusic() { MediaPlayer.Pause(); }

        public void ResumeMusic() { MediaPlayer.Resume(); }

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
            Console.WriteLine("Looking for: " + name);
            foreach (Song song in music)
            {
                if (song.Name == name)
                    return song;
            }

            return null;
        }
    }
}
