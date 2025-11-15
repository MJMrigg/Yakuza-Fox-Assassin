using Godot;
using System;

public partial class WeaponSlot : TextureRect
{
	//Change the image to a different image
	public void Change(string path)
	{
		Texture = (Texture2D)GD.Load(path);
	}
}
