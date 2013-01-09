﻿using System;
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

        private float fadecounter;
        public float fadespeed;
        private Boolean songplaying;
        private Song nextsong;
        private Boolean crossfading;

        public Sound(String gamepath, ContentManager myContent)
        {
            sfx = new List<SoundEffect>();
            music = new List<Song>();

            fadecounter = 0;
            fadespeed = 0.02f;
            crossfading = false;

            // Load all the music and sfx files that are in the content pipeline

                                                          // TODO: V HIER DIE ZAHL MUSS WENN AUF RELEASE GESTELLT WIRD GEÄNDERT WERDEN 
            #if (DEBUG)
                String path = gamepath.Substring(0, gamepath.Length - 14) + "Content\\music\\";
            #else
                String path = gamepath.Substring(0, gamepath.Length - 16) + "Content\\music\\";
            #endif
            
            Console.WriteLine("Path to look for music in: " + path);

            foreach (string f in Directory.GetFiles(path))
            {
                string filename = f.Substring(f.LastIndexOf(@"\") + 1);
                filename = filename.Substring(0, filename.Length - 4);
                if (filename != "Thumb")
                {
                    Song newsong = myContent.Load<Song>("music\\" + filename);
                    music.Add(newsong);
                }
                //Console.WriteLine(filename);
            }

            // Make the Backgroundmusic loop ifinitly
            MediaPlayer.IsRepeating = true;

        }

        public void Update()
        {
            // FADE STUFF IN AND OUT ADJUSTE THE FADECOUNTER AND STUFF PLS
            if (songplaying && fadecounter < 1 && !crossfading)
            {
                fadecounter += fadespeed;
                if (fadecounter > 1)
                    fadecounter = 1;
                MediaPlayer.Volume = fadecounter;
            }
            else if (!songplaying && fadecounter > 0 || songplaying && crossfading && fadecounter > 0)
            {
                fadecounter -= fadespeed;
                if (fadecounter < 0)
                {
                    fadecounter = 0;

                    MediaPlayer.Stop();
                    if (crossfading)
                    {
                        MediaPlayer.Play(nextsong);
                    }
                    crossfading = false;

                }
                MediaPlayer.Volume = fadecounter;
            }
            Console.WriteLine(MediaPlayer.Volume.ToString());
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
            Song song = FindSong(name);
            if (song != null)
            {
                //Check wether a song is already playing
                if (songplaying)
                {
                    crossfading = true;
                    nextsong = song;
                }
                else
                    MediaPlayer.Play(song);
            }
            else
                Console.WriteLine("Couldn't find that Piece Of Music!");

            songplaying = true;
        }

        public void PlayMusic(Song song)
        {
            
            MediaPlayer.Volume = fadecounter;
            
            //Check wether a song is already playing
            if (songplaying)
            {
                crossfading = true;
                nextsong = song;
            }
            else
                MediaPlayer.Play(song);

            songplaying = true;
        }

        public void StopMusic() { songplaying = false; }

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

        public int FindSongIndex(String name)
        {
            Console.WriteLine("Looking for: " + name);
            int x = 0;
            foreach (Song song in music)
            {
                Console.WriteLine(song.Name);
                if (song.Name == name)
                    return x;
                x++;
            }

            return -1;
        }
    }
}
