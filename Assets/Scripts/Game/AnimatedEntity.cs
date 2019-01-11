using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntity : EntityComponent
{
	[BindComponent]
	private Animator animator;

	public bool Right, Left, Up, Down, UpRight, UpLeft, DownRight, DownLeft;
	public int State;
	
	void Update () 
	{
		CheckDirection();
		
		animator.SetBool("Right", Right);
		animator.SetBool("Left", Left);
		animator.SetBool("Up", Up);
		animator.SetBool("Down", Down);
		animator.SetBool("UpRight", UpRight);
		animator.SetBool("UpLeft", UpLeft);
		animator.SetBool("DownRight", DownRight);
		animator.SetBool("DownLeft", DownLeft);

		bool isMoving = entity.Velocity.sqrMagnitude > 0f;

		State = 0;
		if (isMoving)
		{
			State = 1; //Running state
		}
		
		animator.SetInteger("State", State);
	}

	private void CheckDirection()
	{
		bool isMoving = entity.Velocity.sqrMagnitude > 0f;
		if (!isMoving)
			return;
		
		Right = false;
		Left = false;
		Up = false;
		Down = false;
		UpRight = false;
		UpLeft = false;
		DownLeft = false;
		DownRight = false;
		
		var vel = entity.Velocity;
		
		var inv = Mathf.Atan2(vel.y, vel.x);

		var degrees = ((Mathf.Rad2Deg * inv) + 360) % 360; //Clip degrees between 0-360
		
		if (degrees <= 30)
		{
			Right = true;
		} 
		else if (degrees > 30 && degrees <= 60)
		{
			UpRight = true;
		} 
		else if (degrees > 60 && degrees <= 120)
		{
			Up = true;
		} 
		else if (degrees > 120 && degrees <= 150)
		{
			UpLeft = true;
		} 
		else if (degrees > 150 && degrees <= 210)
		{
			Left = true;
		} 
		else if (degrees > 210 && degrees <= 240)
		{
			DownLeft = true;
		} 
		else if (degrees > 240 && degrees <= 300)
		{
			Down = true;
		} 
		else if (degrees > 300 && degrees <= 330)
		{
			DownRight = true;
		} 
		else
		{
			Right = true;
		}
	}
}
