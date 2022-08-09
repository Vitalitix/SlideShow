using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShragaShow {
	public partial class Form1 : Form {
		string currentPath = "";
		public Form1(string[] args) {
			InitializeComponent();
			Data.LoadImageList();
			if (args != null) {
				foreach (var path in args) {
					Data.ReadFolder(path);
				}
			}
			timerSlide.Start();
			timerSlide_Tick(this, null);
		}
		private void timerSlide_Tick(object sender, EventArgs e) {
			ShowImage(true);
		}
		private void ShowImage(bool forward) {
			currentPath = Data.GetNextImage(forward);
			if (currentPath != null) {
				try {
					if (pictureBox1.Image != null) {
						pictureBox1.Image.Dispose();
					}
					pictureBox1.Image = new Bitmap(currentPath);
				} catch {
					Data.RemoveImage(currentPath);
				}
				Text = $"Shraga Show {currentPath}";
			}
		}
		private void ShowTT(string msg) {
			toolTip1.Show(msg, pictureBox1);
		}
		private void Form1_KeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Up:
				case Keys.Down:
					if (e.KeyCode == Keys.Up) {
						timerSlide.Interval += 100;
					} else if (timerSlide.Interval > 200) {
						timerSlide.Interval -= 100;
					}
					ShowTT($"זמן תצוגה: {timerSlide.Interval}");
					break;
				case Keys.OemQuestion:
					//ShowTT("Space: Pause\nUP: Increse display time\nDn: Decrese display time\nLeft: Prvious\nRight: Next\nDelete: Remove Image");
					ShowTT("רווח: השהייה\nחץ למעלה: הגדלת זמן התצוגה\nחץ מטה: הקטנת זמן התצוגה\nחץ שמאלה: תמונה קודמת\nחץ ימינה: תמונה הבאה\nDel: הסר תמונה\nCtrl+Del: הסרת כל התמונות");
					break;
				case Keys.Space:
					if (timerSlide.Enabled)
						timerSlide.Stop();
					else
						timerSlide.Start();
					ShowTT($"תצוגה {(timerSlide.Enabled ? "רצה" : "מושהת")}");
					break;
				case Keys.Left:
					ShowImage(false);
					break;
				case Keys.Right:
					ShowImage(true);
					break;
				case Keys.Delete:
					if (e.Control) {
						pictureBox1.Image = null;
						Data.RemoveAllSlides();
						ShowTT("כל התמונות הוסרו");
					} else {
						ShowTT("תמונה הוסרה מהתצוגה");
						Data.RemoveImage(currentPath);
					}
					break;
				case Keys.Escape:
					pictureBox1_DoubleClick(sender, EventArgs.Empty);
					break;

			}
		}
		private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			Form1_KeyDown(sender, new KeyEventArgs(e.KeyData));
		}

		private void Form1_DragDrop(object sender, DragEventArgs e) {
			var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			Data.ReadFolder(path);
		}

		private void Form1_DragEnter(object sender, DragEventArgs e) {
			DragDropEffects effects = DragDropEffects.None;
			var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			if (Directory.Exists(path)) {
				effects = DragDropEffects.Copy;
			}
			e.Effect = effects;
		}

		private void pictureBox1_DoubleClick(object sender, System.EventArgs e) {
			if (WindowState == FormWindowState.Maximized) {
				TopMost = false;
				WindowState = FormWindowState.Normal;
				FormBorderStyle = FormBorderStyle.Sizable;
			} else {
				TopMost = true;
				FormBorderStyle = FormBorderStyle.None;
				WindowState = FormWindowState.Maximized;
			}

		}
		private void toolTip1_Draw(object sender, DrawToolTipEventArgs e) {
			using (e.Graphics) {
				e.Graphics.FillRectangle(SystemBrushes.Info, e.Bounds);
				e.DrawBorder();
				e.DrawText(TextFormatFlags.RightToLeft | TextFormatFlags.Right);
		}
		}
	}
}
