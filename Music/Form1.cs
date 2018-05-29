﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using WMPLib;

namespace Music
{
    public partial class fMusic : Form
    {
        private static IWMPPlaylist playlistLocalFile;
        private static IWMPPlaylist playlistCurrent;
        int status;
        Image pause;
        Image play;
        Image repeat;
        Image repeat_one;
        Image volume_off;
        Image volume_up;
        delegate void Func();
        Func func;
        public static IWMPPlaylist PlaylistLocalFile { get => playlistLocalFile; set => playlistLocalFile = value; }
        public static IWMPPlaylist PlaylistCurrent { get => playlistCurrent; set => playlistCurrent = value; }

        public fMusic()
        {
            InitializeComponent();
            LoadData();
            func = open;
        }

        public void LoadData()
        {
            #region loadGUI
            panelLeft.Width = 223;
            panelPlay.Location = new Point(455, 21);
            repeat = Music.Properties.Resources.repeat;
            repeat_one = Music.Properties.Resources.repeat_one;
            btnRepeat.Iconimage = repeat;
            volume_off = Music.Properties.Resources.volume_off;
            volume_up = Music.Properties.Resources.volume_up;
            btnVolume.Iconimage = volume_up;
            pause = Music.Properties.Resources.pause;
            play = Music.Properties.Resources.play;
            btnPlay.Image = play;
            myMusic.BringToFront();
            labelTitle.Text = "My music";
            #endregion
            status = 0;
            LoadLocalFile();

            foreach (Control item in myMusic.listControl)
            {
                item.Width = panel.Width - 25;
            }
            playlistCurrent = playlistLocalFile;

            //cần cập nhập lại playlistCurrent mỗi khi phát ở 1 playlist mới
        }
        public void LoadLocalFile()
        {
            myMusic.Clear();
            PlaylistLocalFile = MediaPlayer.Instance.CreatePlaylistForLocalFile();
            string[] listFile = MediaPlayer.Instance.LoadLocalFile(PlaylistLocalFile);
            int i = 0;
            foreach (var item in listFile)
            {
                MediaFile file = new MediaFile(item);
                IWMPMedia media = MediaPlayer.Instance.CreateMedia(file.FilePath);

                Song song = new Song();
                song.index = i++;
                song.Path = file.FilePath;
                song.ButtonPlay_Click += Song_ButtonPlay_Click;

                song.ImageSong = SongInfo.Instance.LoadImageSong(file.FilePath);
                song.SongName = SongInfo.Instance.Song(file.FilePath);
                song.ArtistName = SongInfo.Instance.Artist(file.FilePath);
                song.CategoryName = SongInfo.Instance.Genrne(file.FilePath);
                song.TotalTime = ConvertToMinute(media.duration);
                media.name = song.SongName;

                if (i % 2 == 0)
                    song.BackColor = Color.Silver;
                else
                    song.BackColor = Color.Gainsboro;
                myMusic.song = song;
                GC.Collect();
            }
            MediaPlayer.Instance.SelectCurrentPlaylist(PlaylistLocalFile);
            LoadCurrentMedia();
        }
        private void Song_ButtonPlay_Click(object sender, EventArgs e)
        {
            Song song = sender as Song;
            IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
            song.ImageButton = pause;
            MediaPlayer.Instance.PlayMediaFromPlaylist(PlaylistCurrent, song.index);
            btnPlay.Image = pause;
            LoadCurrentMedia();
            timer1.Start();
            timer2.Start();
        }
        public void LoadCurrentMedia()
        {
            IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
            string path = media.sourceURL;

            pictureBoxSong.Image = SongInfo.Instance.LoadImageSong(path);
            lblSongName.Text = SongInfo.Instance.Song(path);
            lblArtistName.Text = SongInfo.Instance.Artist(path);
            double duration = media.duration;

            labelTimeFrom.Text = "00:00";
            labelTimeTo.Text = ConvertToMinute(duration);

            UISort.pathSongPlay = path;
          
            switch (status)
            {
                case 0:
                    {
                        Song song = myMusic.listSong.FindAll(UISort.FindSongNamePlay)[0];
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPlaying")
                            song.ImageButton = Music.Properties.Resources.pause;
                        else
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPaused")
                            song.ImageButton = Music.Properties.Resources.play;
                        myMusic.ScrollControl = song;
                        break;
                    }
                case 2:
                    {
                        Song song = nowPlaying.listSong.FindAll(UISort.FindSongNamePlay)[0];
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPlaying")
                            song.ImageButton = Music.Properties.Resources.pause;
                        else
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPaused")
                            song.ImageButton = Music.Properties.Resources.play;
                        nowPlaying.ScrollControl = song;
                        break;
                    }
                case 3:
                    {
                        Song song = playlistDetail.listSong.FindAll(UISort.FindSongNamePlay)[0];
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPlaying")
                            song.ImageButton = Music.Properties.Resources.pause;
                        else
                        if (MediaPlayer.Instance.GetPlayState() == "wmppsPaused")
                            song.ImageButton = Music.Properties.Resources.play;
                        playlistDetail.ScrollControl = song;
                        break;
                    }
            }
            GC.Collect();
        }
        public void LoadLyrics()
        {
            IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
            string path = media.sourceURL;
            lyrics.SongImage = SongInfo.Instance.LoadImageSong(path);
            lyrics.ArtistName = SongInfo.Instance.Artist(path);
            lyrics.SongName = SongInfo.Instance.Song(path);

            lyrics.LyricsText = SongInfo.Instance.Lyrics(path);
            //if (lyrics.LyricsText == string.Empty)
            //    lyrics.LyricsText=(string)Lyric.LyricSong.Instance.GetLyric(lyrics.ArtistName, lyrics.SongName)?? "";
            //timer4.Start();
        }
        public void LoadNowPlaying()
        {
            nowPlaying.Clear();
            List<string> listFile = MediaPlayer.Instance.LoadCurrentPlaylist(PlaylistCurrent);
            int i = 0;
            foreach (var item in listFile)
            {
                MediaFile file = new MediaFile(item);
                IWMPMedia media = MediaPlayer.Instance.CreateMedia(file.FilePath);

                Song song = new Song();
                song.index = i++;
                song.ButtonPlay_Click += Song_ButtonPlay_Click;
                song.ImageSong = SongInfo.Instance.LoadImageSong(file.FilePath);
                song.Path = file.FilePath;
                song.SongName = SongInfo.Instance.Song(file.FilePath);
                song.ArtistName = SongInfo.Instance.Artist(file.FilePath);
                song.CategoryName = SongInfo.Instance.Genrne(file.FilePath);
                song.TotalTime = ConvertToMinute(media.duration);
                media.name = song.SongName;

                if (i % 2 == 0)
                    song.BackColor = Color.Silver;
                else
                    song.BackColor = Color.Gainsboro;
                nowPlaying.song = song;
            }
            LoadCurrentMedia();
            GC.Collect();
        }
        public void LoadListPlaylist()
        {
            playlist.Clear();
            List<IWMPPlaylist> listPlaylist = MediaPlayer.Instance.LoadListPlaylist();
            foreach (var item in listPlaylist)
            {
                Myplaylist myplaylist = new Myplaylist();
                myplaylist.Tag = item;
                myplaylist.BtnImage_Click += Myplaylist_BtnImage_Click;
                myplaylist.PlaylistName = item.name.Split('_')[0];
                myplaylist.PlaylistImage = SongInfo.Instance.LoadImagePlaylist(item);
                playlist.myplaylist = myplaylist;
            }
        }

        private void Myplaylist_BtnImage_Click(object sender, EventArgs e)
        {
            Myplaylist myplaylist = sender as Myplaylist;
            playlistDetail.PlaylistImage = myplaylist.PlaylistImage;
            playlistDetail.PlaylistName = myplaylist.PlaylistName;

            playlistDetail.Clear();
            List<string> listFile = MediaPlayer.Instance.LoadCurrentPlaylist(myplaylist.Tag as IWMPPlaylist);
            int i = 0;
            if(listFile.Count>0)
            foreach (var item in listFile)
            {

                MediaFile file = new MediaFile(item);
                IWMPMedia media = MediaPlayer.Instance.CreateMedia(file.FilePath);

                Song song = new Song();
                song.index = i++;
                song.ButtonPlay_Click += Song_ButtonPlay_Click;
                song.ImageSong = SongInfo.Instance.LoadImageSong(file.FilePath);
                song.Path = file.FilePath;
                song.SongName = SongInfo.Instance.Song(file.FilePath);
                song.ArtistName = SongInfo.Instance.Artist(file.FilePath);
                song.CategoryName = SongInfo.Instance.Genrne(file.FilePath);
                song.TotalTime = ConvertToMinute(media.duration);
                media.name = song.SongName;

                if (i % 2 == 0)
                    song.BackColor = Color.Silver;
                else
                    song.BackColor = Color.Gainsboro;
                playlistDetail.song = song;
            }
            foreach (Control item in playlistDetail.listControl)
            {
                item.Width = panel.Width - 20;
            }
            MediaPlayer.Instance.SelectCurrentPlaylist(myplaylist.Tag as IWMPPlaylist);
            
            LoadCurrentMedia();
            GC.Collect();

            playlistDetail.BringToFront();
        }

        public string ConvertToMinute(double Second)
        {
            int minute = (int)Second / 60;
            int second = (int)Second % 60;
            return minute.ToString("00") + ":" + second.ToString("00");
        }
        public void ChangeNormalColorOnPanel1(object sender)
        {
            BunifuFlatButton btn = sender as BunifuFlatButton;
            btn.Normalcolor = Color.FromArgb(239, 108, 1);
            //if (panelLeft.Width == 55)
            //    btn.Width = 40;
            //else
            //    btn.Width = 205;
            foreach (Control item in panel1.Controls)
            {
                if (item.Name != btn.Name && item.BackColor != Color.Transparent)
                {
                    BunifuFlatButton btn1 = item as BunifuFlatButton;
                    btn1.Normalcolor = Color.Transparent;

                    //if (panelLeft.Width == 55)
                    //    item.Width = 205;
                    //else
                    //    item.Width = 40;

                }
            }
        }
        private void btnBack_Click(object sender, EventArgs e)
        {

        }
        private void btnNavigationPanel_Click_1(object sender, EventArgs e)
        {
            if (panelLeft.Width == 223)
            {
                panelLeft.Width = 55;
            }
            else
            {
                panelLeft.Width = 223;
            }
        }
        private void btnMyMusic_Click_1(object sender, EventArgs e)
        {
            status = 0;
            PlaylistCurrent = PlaylistLocalFile;
            labelTitle.Text = "My music";
            ChangeNormalColorOnPanel1(sender);
            myMusic.BringToFront();
            foreach (Control item in myMusic.listControl)
            {
                item.Width = panel.Width - 25;
            }
        }

        private void btnRecentPlays_Click_1(object sender, EventArgs e)
        {
            status = 1;
            nowPlaying.Clear();
            ChangeNormalColorOnPanel1(sender);
        }
        private void btnNowPlaying_Click_1(object sender, EventArgs e)
        {
            labelTitle.Text = "Now playing";
            status = 2;
            ChangeNormalColorOnPanel1(sender);
            nowPlaying.BringToFront();
            LoadNowPlaying();
            foreach (Control item in nowPlaying.listControl)
            {
                item.Width = panel.Width - 20;
            }
        }

        private void btnPlayList_Click_1(object sender, EventArgs e)
        {
            status = 3;
            labelTitle.Text = "Playlist";
            LoadListPlaylist();
            playlist.BringToFront();
            ChangeNormalColorOnPanel1(sender);
        }

        private void btnSetting_Click_1(object sender, EventArgs e)
        {

            ChangeNormalColorOnPanel1(sender);
        }

        private void btnAbout_Click_1(object sender, EventArgs e)
        {
            ChangeNormalColorOnPanel1(sender);
        }
        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Maximized;
                panelPlay.Location = new Point(601, 21);
                //1143  624
                //1135 thông số của panel 1143, 624 của panel lúc full
                //bunifuSlider1.Value = 50; cần cập nhật value cho control này khi maxmize để tránh làm nát form
            }
            else
            {
                WindowState = FormWindowState.Normal;
                panelPlay.Location = new Point(455, 21);
            }

        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bunifuFlatButton1_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }



        private void btnShuffle_Click(object sender, EventArgs e)
        {
            MediaPlayer.Instance.Shuffle();
            if (btnShuffle.Normalcolor == Color.Transparent)
            {
                btnShuffle.Normalcolor = Color.FromArgb(239, 108, 1);
            }
            else
            {
                btnShuffle.Normalcolor = Color.Transparent;
            }
        }
        private void btnRepeat_Click(object sender, EventArgs e)
        {
            if (btnRepeat.Normalcolor == Color.Transparent)
            {
                btnRepeat.Normalcolor = Color.FromArgb(239, 108, 1);
                MediaPlayer.Instance.Repeat();
                timer3.Stop();
            }
            else
            {
                if (btnRepeat.Iconimage == repeat)
                {
                    IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
                    btnRepeat.Iconimage = repeat_one;
                    timer3.Start();
                }
                else
                {
                    btnRepeat.Iconimage = repeat;
                    btnRepeat.Normalcolor = Color.Transparent;
                    timer3.Stop();
                }
            }
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (btnPlay.Image == pause)
            {
                btnPlay.Image = play;
                MediaPlayer.Instance.Pause();
                timer1.Stop();
                timer2.Stop();
            }
            else
            {
                btnPlay.Image = pause;
                MediaPlayer.Instance.Play();
                timer1.Start();
                timer2.Start();
            }
            LoadCurrentMedia();
        }
        private void btnVolume_Click(object sender, EventArgs e)
        {
            if (btnVolume.Iconimage == volume_up)
            {
                btnVolume.Iconimage = volume_off;
                MediaPlayer.Instance.Mute();
            }
            else
            {
                btnVolume.Iconimage = volume_up;
                MediaPlayer.Instance.MuteOff();
            }
        }

        private void myMusic1_Load(object sender, EventArgs e)
        {

        }

        private void fMusic_FormClosing(object sender, FormClosingEventArgs e)
        {

            //MediaPlayer.Instance.RemovePlaylist(playlistLocalFile);
            MediaPlayer.Instance.DeleteLocalFile();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            MediaPlayer.Instance.Next();
            LoadCurrentMedia();
            LoadLyrics();
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            MediaPlayer.Instance.Previous();
            LoadCurrentMedia();
            LoadLyrics();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
            double duration = media.duration;
            sliderDuration.MaximumValue = (int)duration;
            sliderDuration.Value = (int)MediaPlayer.Instance.GetCurrentPosition();
            labelTimeFrom.Text = ConvertToMinute(MediaPlayer.Instance.GetCurrentPosition());
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if ((int)MediaPlayer.Instance.GetCurrentPosition() == 0 && MediaPlayer.Instance.GetPlayState() == "wmppsPlaying")
            {
                LoadCurrentMedia();
                LoadLyrics();
            }
        }

        private void sliderDuration_ValueChanged(object sender, EventArgs e)
        {
            MediaPlayer.Instance.SetCurrentPosition(sliderDuration.Value);
        }

        private void sliderVolumn_ValueChanged(object sender, EventArgs e)
        {
            btnVolume.Iconimage = volume_up;
            MediaPlayer.Instance.MuteOff();
            MediaPlayer.Instance.SetVolumn(sliderVolumn.Value);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            IWMPMedia media = MediaPlayer.Instance.GetCurrentMedia();
            int result = (int)media.duration - (int)MediaPlayer.Instance.GetCurrentPosition();
            if (result == 0)
                foreach (Control item in myMusic.listSong)
                    if ((item as Song).SongName == media.name)
                        MediaPlayer.Instance.PlayMediaFromPlaylist(PlaylistCurrent, (item as Song).index);
        }

        private void panel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control item in myMusic.listControl)
            {
                item.Width = panel.Width - 25;
            }
            foreach (Control item in nowPlaying.listControl)
            {
                item.Width = panel.Width - 20;
            }
            foreach (Control item in playlistDetail.listControl)
            {
                item.Width = panel.Width - 20;
            }
        }
        void open()
        {
            LoadLyrics();
            lyrics.BringToFront();
            func = close;
        }
        void close()
        {
            lyrics.SendToBack();
            func = open;
        }
        private void btnLyric_Click(object sender, EventArgs e)
        {
            func();
        }

        private void lyrics_btnBack_click(object sender, EventArgs e)
        {
            lyrics.SendToBack();
            func = open;
            timer4.Stop();
        }
        public Bitmap rotateImage(Bitmap bitmap, float angle)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
                graphics.RotateTransform(angle);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2);
                graphics.DrawImage(bitmap, new Point(0, 0));
                graphics.Dispose();
            }
            return bitmap;
        }
        private void timer4_Tick(object sender, EventArgs e)
        {
            lyrics.SongImage = rotateImage(new Bitmap(lyrics.SongImage), 1);
        }

        private void playlist_NewPlaylist_Click(object sender, EventArgs e)
        {
            fNewPlaylist fNewPlaylist = new fNewPlaylist();
            fNewPlaylist.ShowDialog();
            MediaPlayer.Instance.CreatePlaylist(fNewPlaylist.playlistName);
            LoadListPlaylist();
        }
    }
}
