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
using MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.Security.Cryptography;
using MongoDB.Bson;

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
            char str = Console.ReadKey().KeyChar;
            #region switches for input
            switch (str) {
              case 's':
              _spotify.Skip();
              Thread.Sleep(100);
              break;
              case 'p':
              _spotify.Pause();
              break;
              case 'b':
              _spotify.Previous();
              Thread.Sleep(100);
              break;
              case ' ':
              if (_spotify.GetStatus().Playing) {
                _spotify.Pause();
                PausedSong(_spotify.GetStatus().Track);
              }
              else if (!(_spotify.GetStatus().Playing)) {
                _spotify.Play();
                DisplaySong(_spotify.GetStatus().Track);
              }
              break;
              case 'z':
              DisplaySong(_spotify.GetStatus().Track);
              break;
              case 'a':
              _spotify.Pause();
              x = 1;
              break;
              case 'm':
              connectToMongo();
              break;
            }
            #endregion
          }
        }
      }
      else {
        Console.WriteLine("ERROR: couldnt connect to spotify.");
      }
    }

    public static void PausedSong(Track song) {
      Console.WriteLine("\n[{0}] : Currently paused: {1} - {2} ({3})", DateTime.Now.ToString(), song.TrackResource.Name, song.ArtistResource.Name, song.AlbumResource.Name);
    }

    public static void DisplaySong(Track song) {
      Console.WriteLine("\n[{0}] : Currently playing: {1} - {2} ({3})", DateTime.Now.ToString(), song.TrackResource.Name, song.ArtistResource.Name, song.AlbumResource.Name);

    }

    private static void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e) {
      //do something
      //Console.WriteLine("hello testing the playstate change thing.");
      //playing/not playing toggle this will flag up..
    }
    private static void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e) {
      //this writes the old track to the mongo database following the
      //mongo_writeTrackToMongo(e.OldTrack);

      DisplaySong(e.NewTrack);

    }
    public static void connectToMongo() {
      var client = new MongoClient();
      var db = client.GetDatabase("spotify");
      var collection = db.GetCollection<Post>("Post");
      //collection.DeleteMany("{ }");


      //Create a Post to enter into the database.
      var post = new Post() {
        Title = "My First Post",
        Body = "This isn't a very long post.",
        CharCount = 27,
        Comments = new List<Comment>
        {
            { new Comment() { TimePosted = new DateTime(2010,1,1),
                              Email = "bob_mcbob@gmail.com",
                              Body = "This article is too short!" } },
            { new Comment() { TimePosted = new DateTime(2010,1,2),
                              Email = "Jane.McJane@gmail.com",
                              Body = "I agree with Bob." } }
        }
      };

      //Save the post. This will perform an upsert. As in, if the post
      //already exists, update it, otherwise insert it.
      collection.InsertOne(post);

      var document = collection.Find(new BsonDocument()).FirstOrDefault();
      Console.WriteLine(document.Title);
    }
    public class Post {
      public ObjectId Id { get; set; }
      public string Title { get; set; }
      public string Body { get; set; }
      public int CharCount { get; set; }
      public IList<Comment> Comments { get; set; }
    }

    public class Comment {
      public DateTime TimePosted { get; set; }
      public string Email { get; set; }
      public string Body { get; set; }
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
          //Artist a  = new Artist();
        }

      }
      catch (Exception ex) {

      }
    }
  }
}
