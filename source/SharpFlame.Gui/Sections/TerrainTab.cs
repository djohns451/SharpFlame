using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Sections
{
	public class TerrainTab : Panel
	{
		RadioButtonList placeFillRadios;
		RadioButtonList roadRadios;
		RadioButtonList cliffRadios;

		public TerrainTab ()
		{
			placeFillRadios = new RadioButtonList ();
			placeFillRadios.Spacing = new Size(0, 0);
			placeFillRadios.Orientation = RadioButtonListOrientation.Vertical;
			placeFillRadios.Items.Add(new ListItem { Text = "Place" });
			placeFillRadios.Items.Add(new ListItem { Text = "Fill" });

			roadRadios = new RadioButtonList ();
			roadRadios.Spacing = new Size(0, 0);
			roadRadios.Orientation = RadioButtonListOrientation.Vertical;
			roadRadios.Items.Add(new ListItem { Text = "Sides" });
			roadRadios.Items.Add(new ListItem { Text = "Lines" });
			roadRadios.Items.Add(new ListItem { Text = "Remove" });

			cliffRadios = new RadioButtonList ();
			cliffRadios.Spacing = new Size(0, 0);
			cliffRadios.Orientation = RadioButtonListOrientation.Vertical;
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Triangle" });
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Brush" });
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Remove" });

			placeFillRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0) 
				{
					roadRadios.SelectedIndex = 0;
					cliffRadios.SelectedIndex = 0;
				}
			};

			roadRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0)
				{
					placeFillRadios.SelectedIndex = 0;
					cliffRadios.SelectedIndex = 0;
				}
			};

			cliffRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0)
				{
					placeFillRadios.SelectedIndex = 0;
					roadRadios.SelectedIndex = 0;
				}
			};


			var layout = new DynamicLayout();
            layout.Add (GroundTypeSection ());
			layout.Add (RoadTypeSection ());
			layout.Add (CliffSection ());
            layout.Add (null);
			Content = layout;
		}

		Control CliffSection () {
			var control = new GroupBox { Text = "Cliff:" };

			var mainLayout = new DynamicLayout ();

			var nLayout1 = new DynamicLayout ();
			nLayout1.Padding = Padding.Empty;
			nLayout1.AddRow(new Label { Text = "Cliff Angle", VerticalAlign = VerticalAlign.Middle },
							new NumericUpDown { Size = new Size(-1, -1), Value = 35, MaxValue = 360, MinValue = 1 },
							null);

			var nLayout2 = new DynamicLayout ();
			nLayout2.Padding = Padding.Empty;
			nLayout2.AddRow(new CheckBox { Text = "Set Tris" }, null);

			var nLayout3 = new DynamicLayout ();
			nLayout3.Padding = Padding.Empty;
			nLayout3.Spacing = Size.Empty;
			nLayout3.Add (nLayout1);
			nLayout3.Add (nLayout2);
			nLayout3.Add (null);

			var nLayout4 = new DynamicLayout ();
			nLayout4.Padding = Padding.Empty;
			nLayout4.Spacing = Size.Empty;
			nLayout4.AddRow (cliffRadios, null, nLayout3, null);

			mainLayout.AddRow (nLayout4);

			var circularButton = new Button { Text = "Circular" };
			circularButton.Enabled = false;
			var squareButton = new Button { Text = "Square" };
			circularButton.Click += (sender, e) => { 
				circularButton.Enabled = false;
				squareButton.Enabled = true;
			};
			squareButton.Click += (sender, e) => { 
				squareButton.Enabled = false;
				circularButton.Enabled = true;
			};

			var nLayout5 = new DynamicLayout ();
			nLayout5.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
							new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = 512, MinValue = 1 }, 
						    circularButton, 
							squareButton,
							null);

			mainLayout.AddRow (nLayout5, null);

			control.Content = mainLayout;
			return control;
		}

		Control RoadTypeSection () {
			var control = new GroupBox { Text = "Road Type:" };

			var mainLayout = new DynamicLayout ();
			
			var roadTypeListBox = RoadTypeListBox ();
			var eraseButton = new Button { Text = "Erase", Size = new Size(80, 27) };
			eraseButton.Click += delegate {
				roadTypeListBox.SelectedIndex = -1;
			};

			mainLayout.AddRow (roadTypeListBox,
			                   TableLayout.AutoSized (eraseButton),
			                   null);

			mainLayout.AddRow (roadRadios, null);

			control.Content = mainLayout;
			return control;
		}

        Control GroundTypeSection () {
            var control = new GroupBox { Text = "Ground Type:" };

            var circularButton = new Button { Text = "Circular" };
            circularButton.Enabled = false;
            var squareButton = new Button { Text = "Square" };
            circularButton.Click += (sender, e) => { 
                circularButton.Enabled = false;
                squareButton.Enabled = true;
            };
            squareButton.Click += (sender, e) => { 
                squareButton.Enabled = false;
                circularButton.Enabled = true;
            };

            var mainLayout = new DynamicLayout ();
            mainLayout.BeginVertical();
            mainLayout.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle }, 
            				new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = 512, MinValue = 1 }, 
            				circularButton, 
            				squareButton,
            				null);
            mainLayout.EndVertical ();

            var gdLayout2 = new DynamicLayout ();
			gdLayout2.Padding = Padding.Empty;
			gdLayout2.Spacing = Size.Empty;

			var gdLayout3 = new DynamicLayout ();
			gdLayout3.Padding = Padding.Empty;
			gdLayout3.Spacing = Size.Empty;
			var gdLayout4 = new DynamicLayout ();
			gdLayout4.Padding = Padding.Empty;
			gdLayout4.Spacing = Size.Empty;

			var groundTypeListBox = GroundTypeListBox ();
			var eraseButton = new Button { Text = "Erase", Size = new Size(80, 26) };
			eraseButton.Click += delegate {
				groundTypeListBox.SelectedIndex = -1;
			};

			gdLayout3.AddRow (new CheckBox { Checked = true, Text = "Make Invalid Tiles" },
							  null);
			gdLayout4.AddRow (eraseButton, null);

			gdLayout2.AddRow (gdLayout3);
            gdLayout2.Add (null);
            gdLayout2.AddRow (gdLayout4);

            var groundTypeLayout = new DynamicLayout ();
            groundTypeLayout.BeginHorizontal ();
			groundTypeLayout.Add (groundTypeListBox);
            groundTypeLayout.Add (gdLayout2);
			groundTypeLayout.Add (null);
			groundTypeLayout.EndHorizontal ();
            mainLayout.AddRow (groundTypeLayout);

			var gdCliffRadios = new RadioButtonList ();
			gdCliffRadios.Spacing = new Size(0, 0);
			gdCliffRadios.Orientation = RadioButtonListOrientation.Vertical;
			gdCliffRadios.Items.Add(new ListItem { Text = "Ignore Cliff" });
			gdCliffRadios.Items.Add(new ListItem { Text = "Stop Before Cliff" });
			gdCliffRadios.Items.Add(new ListItem { Text = "Stop After Cliff" });
			gdCliffRadios.SelectedIndex = 0;

			var gdLayout6 = new DynamicLayout ();
			gdLayout6.AddRow(new CheckBox { Text = "Stop Before Edge" }, null);
			gdLayout6.AddRow (null);

			var gdLayout5 = new DynamicLayout();
			gdLayout5.BeginHorizontal ();
			gdLayout5.Add (placeFillRadios);
			gdLayout5.Add (gdCliffRadios);
			gdLayout5.Add (gdLayout6);
			gdLayout5.Add (null);
			gdLayout5.EndHorizontal ();

			mainLayout.Add (gdLayout5);

            control.Content = mainLayout;
            return control;
        }

		ListBox GroundTypeListBox() {
            var control = new ListBox ();
			control.Size = new Size (200, 150);
            control.Items.Add (new ListItem { Text = "Grass" });
            control.Items.Add (new ListItem { Text = "Gravel" });
            control.Items.Add (new ListItem { Text = "Dirt" });
            control.Items.Add (new ListItem { Text = "Grass Snow" });
            control.Items.Add (new ListItem { Text = "Gravel Snow" });
            control.Items.Add (new ListItem { Text = "Snow" });
            control.Items.Add (new ListItem { Text = "Concrete" });
            control.Items.Add (new ListItem { Text = "Water" });

            return control;
        }

		ListBox RoadTypeListBox() {
			var control = new ListBox ();
			control.Size = new Size (200, 48);
			control.Items.Add (new ListItem { Text = "Road" });
			control.Items.Add (new ListItem { Text = "Track" });

			return control;
		}
	}
}