using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public AnimatedSprite2D mySpriteAnimation;

    public string currentDir = "D";
    public override void _Ready()
    {
        base._Ready();

        Velocity = new Vector2(0, 0);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!mySpriteAnimation.Animation.ToString().StartsWith("Walk"))
        {
            if (!mySpriteAnimation.IsPlaying())
            {
                mySpriteAnimation.Animation = "Walk_" + currentDir;
                mySpriteAnimation.Play();
            }
            else
            {
                return;
            }
        }

        float hInput = Input.GetAxis("p_a", "p_d");
        float vInput = Input.GetAxis("p_s", "p_w");
        Velocity = new Vector2(0,0);
        if (Mathf.Abs(hInput) < 0.1f && Mathf.Abs(vInput) < 0.1f)
        {
            mySpriteAnimation.Frame = 0;
        }
        else if (Mathf.Abs(hInput) > Mathf.Abs(vInput))
        {
            if (hInput > 0)
            {
                Velocity += new Vector2(100, 0);
                mySpriteAnimation.Animation = "Walk_R";
                currentDir = "R";
            }
            else
            {
                Velocity += new Vector2(-100, 0);
                mySpriteAnimation.Animation = "Walk_L";
                currentDir = "L";
            }
        }
        else
        {
            mySpriteAnimation.Play();
            if (vInput > 0)
            {
                //up
                Velocity += new Vector2(0, -100);
                mySpriteAnimation.Animation = "Walk_U";
                currentDir = "U";
            }
            else
            {
                //down
                Velocity += new Vector2(0, 100);
                mySpriteAnimation.Animation = "Walk_D";
                currentDir = "D";
            }
        }

        /*if (Input.IsActionJustPressed(""))
        {
            GD.Print(" pressed!");
            mySpriteAnimation.Animation = "Hurt_" + currentDir;
            mySpriteAnimation.Play();

        }
        
        if (Input.IsActionJustPressed(""))
        {
            GD.Print(" pressed!");
            mySpriteAnimation.Animation = "Bite_" + currentDir;
            mySpriteAnimation.Play();

        }*/

        MoveAndSlide();

    }
}
