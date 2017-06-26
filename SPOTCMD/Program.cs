using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using System.Threading;
using System.Globalization;

namespace SPOTCMD {
  class Program {
    static void Main(string[] args) {
      SpotifyLocalAPI _spotify;
      Track _currentTrack;
      //make a connection with spotify
      _spotify = new SpotifyLocalAPI();

      _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
      _spotify.OnTrackChange += _spotify_OnTrackChange;

      bool successful = _spotify.Connect();
      if (successful) {
        Console.WriteLine("Successfully made a connection with spotify. ");
        _spotify.ListenForEvents = true;

        Console.Write("......\n");

        //TODO: load what is currently playing..
        StatusResponse status = _spotify.GetStatus();
        DisplaySong(status.Track);
        _currentTrack = status.Track;

        if (status.Playing || !(status.Playing)) {
          int x = 0;
          while (x == 0) {
            String str = Console.ReadLine();
            switch(str) {
              case "s":
                _spotify.Skip();
                Thread.Sleep(100);
                break;
              case "p":
                _spotify.Pause();
                break;
              case "b":
                _spotify.Previous();
                Thread.Sleep(100);
                break;
              case " ":
                _spotify.Play();
                DisplaySong(_spotify.GetStatus().Track);
                break;
              case "a":
                _spotify.Pause();
                x = 1;
                break;
            }   
          }
        }
      }

      else {
        Console.WriteLine("ERROR: couldnt connect to spotify.");
      }

      //Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
      //thread.Start();
      //Console.WriteLine("This should be working now ");
      //Console.ReadLine();

    }

    public static void DisplaySong(Track song) {
      Console.WriteLine("\nCurrently playing: {0} - {1} ({2})", song.TrackResource.Name, song.ArtistResource.Name, song.AlbumResource.Name);
    }

    private static void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e) {
      //do something
      //Console.WriteLine("hello testing the playstate change thing.");
      //playing/not playing toggle this will flag up..
    }
    private static void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e) {
      //Console.WriteLine("onTrackChange has been clled" );
      DisplaySong(e.NewTrack);

    }
    public static void WorkThreadFunction() {
      try {
        for (int k = 0; k <= 100; k++) {

          //the player may have to be multithreaded to allow for that to be edited while the system waits for the users input..
          //ThreadStart t = new Thread
          //Thread t1 = new Thread();
          Console.SetCursorPosition(8, 4);
          Console.Write("{0}%", k);
          System.Threading.Thread.Sleep(50);
        }

      }
      catch (Exception ex) {

      }
    }
  }
}
