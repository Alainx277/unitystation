﻿using UnityEngine;
using UI.Core.NetUI;
using Objects;

namespace UI.Objects
{
	public class GUI_Jukebox : NetTab
	{
		[SerializeField]
		private NetLabel labelSong = null;

		[SerializeField]
		private NetLabel labelArtist = null;

		[SerializeField]
		private NetLabel labelTrack = null;

		[SerializeField]
		private NetPrefabImage prefabImagePlayStop = null;

		[SerializeField]
		private NetSlider sliderVolume = null;

		private Jukebox jukebox;
		private Jukebox Jukebox => jukebox ??= Provider.GetComponent<Jukebox>();

		public void OnTabOpenedHandler(ConnectedPlayer connectedPlayer)
		{
			labelTrack.Value = Jukebox.TrackPosition;
			labelSong.Value = Jukebox.SongName;
			labelArtist.Value = Jukebox.Artist;
			prefabImagePlayStop.Value = Jukebox.PlayStopButtonPrefabImage;
		}

		public void PlayOrStop()
		{
			if (Jukebox.IsPlaying)
			{
				Jukebox.Stop();
			}
			else
			{
				_ = Jukebox.Play();
			}
		}

		public void PreviousSong()
		{
			Jukebox.PreviousSong();
		}

		public void NextSong()
		{
			Jukebox.NextSong();
		}

		public void ClosePanel()
		{
			ControlTabs.CloseTab(Type, Provider);
		}

		public void VolumeChange()
		{
			Jukebox.VolumeChange(float.Parse(sliderVolume.Value) / 100);
		}
	}
}