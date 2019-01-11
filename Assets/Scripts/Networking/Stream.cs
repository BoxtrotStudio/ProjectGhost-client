using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stream {

	/// <summary>
	/// This stream is used to test updates as they're made. This means only developers can access
	/// these servers. These server should never be made public and should only be hosted on localhost or a private
	/// network. This represents 0 as the default allowed stream should be 0.
	/// 
	/// This could also represent a private server.
	/// </summary>
	TEST = 0,


	/// <summary>
	/// This stream is used to indicate a server has entered the testing phase. Only the inner-team of testers
	/// can access these servers
	/// </summary>
	ALPHA = 1,


	/// <summary>
	/// This stream is used to indicate a server has passed the alpha phase. Only players in the beta program
	/// can access these servers.
	/// </summary>
	BETA = 2,

	/// <summary>
	/// This stream is used to indicate a server is pending to go live. This could be for a number of reasons:
	/// * The server is out of date and must update before going live
	/// * This server includes a patch that has not been released yet
	/// * This server is experiencing technical problems and shouldn't be live until those problems are fixed
	/// </summary>
	BUFFERED = 3,

	
	/// <summary>
	/// This stream is used to indicate a server is live.
	/// </summary>
	LIVE = 4
}

public static class StreamExtensions
{
	public static bool Allowed(this Stream act, Stream level)
	{
		if (level == Stream.BUFFERED)
			return false;

		int highest = (int) (4 - level);
		int current = 4 - (int)act;

		return current <= highest;
	}

	public static Stream AsStream(this int value)
	{
		switch (value)
		{
			case 0:
				return Stream.TEST;
			case 1:
				return Stream.ALPHA;
			case 2:
				return Stream.BETA;
			case 3:
				return Stream.BUFFERED;
			case 4:
				return Stream.LIVE;
			default:
				throw new ArgumentException("Invalid stream value (must be between 0-4)");
		}
	}
}