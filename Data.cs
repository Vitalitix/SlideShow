using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShragaShow {
	[Serializable]
	internal class Image {
		internal string path;
		internal int showCount;
		internal int priority;
	}
	static internal class Data {
		private static ITwoWayEnumerator<Image> imageEnumerator;
		private static readonly string fileName = "images.dat";
		internal static List<Image> imageList = new();
		private static string[] filters = ".BMP,.EMF,.EXIF,.GIF,.ICON,.JPEG,.PNG,.TIFF,.WMF,.JPG".Split(',');
		internal static void LoadImageList() {
			var random = new Random();
			ReadFromDisk();
			imageEnumerator = TwoWayEnumeratorHelper.GetTwoWayEnumerator(imageList);
			int rand = random.Next(imageList.Count);
			for (int i = 0; i < rand; i++) {
				imageEnumerator.MoveNext();
			}
		}
		internal static void RemoveImage(string path) {
			var ii = imageList.FindIndex(i => i.path == path);
			if (ii >= 0) {
				imageList.RemoveAt(ii);
				imageEnumerator = TwoWayEnumeratorHelper.GetTwoWayEnumerator(imageList);
			}
		}
		internal static void ReadFolder(string path) {
			var files = GetFiles(path);
			foreach (var file in files) {
				if (!imageList.Exists(i => i.path == file) && filters.Contains(Path.GetExtension(file))) {
					imageList.Add(new Image() {
						path = file,
						showCount = 0,
						priority = 0
					});
				}
			}
			imageEnumerator = TwoWayEnumeratorHelper.GetTwoWayEnumerator(imageList);
			SaveToDisk();
		}
		internal static void SaveToDisk() {
			using FileStream fs = File.OpenWrite(fileName);
			var formatter = new BinaryFormatter();
			formatter.Serialize(fs, imageList);
		}
		private static void ReadFromDisk() {
			if (File.Exists(fileName)) {
				using FileStream stream = File.OpenRead(fileName);
				BinaryFormatter formatter = new BinaryFormatter();
				imageList = (List<Image>)formatter.Deserialize(stream);
				stream.Close();
			}
		}
		private static IEnumerable<string> GetFiles(string searchFolder) {
			return Directory.EnumerateFiles(searchFolder, "*", SearchOption.AllDirectories);
		}
		internal static void RemoveAllSlides() {
			imageList.Clear();
			imageEnumerator = TwoWayEnumeratorHelper.GetTwoWayEnumerator(imageList);
		}
		internal static string GetNextImage(bool forward = true) {
			Image im = null;
			var moveFunc = imageEnumerator.MoveNext;
			if (!forward) {
				moveFunc = imageEnumerator.MovePrevious;
			}
			while (moveFunc()) {
				im = imageEnumerator.Current;
				if (File.Exists(im.path)) {
					break;
				} else {
					imageList.Remove(im);
				}
			}
			if (im == null) {
				if (forward) {
					imageEnumerator.Reset();
				}
				return null;
			} else {
				im.showCount++;
				return im.path;
			}
		}
	}
}
