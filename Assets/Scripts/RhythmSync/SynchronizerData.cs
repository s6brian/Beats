/*
 * source: 
 *      - https://christianfloisand.wordpress.com/2014/01/23/beat-synchronization-in-unity/
 *      - https://github.com/cfloisand/beat-synchronizer-unity
 */

using System.Collections;
using System.Collections.Generic;

namespace SynchronizerData
{
	// BeatValue determines which beat to synchronize with, and is specified for each BeatCounter instance.
	// (A sequence of beat values are specified for PatternCounter instances).
	public enum BeatValue
    {
		None,
		SixteenthBeat,
		SixteenthDottedBeat,
		EighthBeat,
		EighthDottedBeat,
		QuarterBeat,
		QuarterDottedBeat,
		HalfBeat,
		HalfDottedBeat,
		WholeBeat,
		WholeDottedBeat
	}

	// BeatType is used to specify whether the beat value is an off-beat, on-beat, up-beat, or a down-beat.
	// This value is sent along with the notify message when a beat occurs so that different action
	// may be taken for the different beat types.
	// This information is stored in a beatMask field in each BeatObserver instance.
	public enum BeatType
    {
		None		= 0,
		OffBeat		= 1 << 0,
        OnBeat		= 1 << 1,
        UpBeat		= 1 << 2,
        DownBeat	= 1 << 3
	};

	// The decimal values associated with each beat value are used in calculating the sample position in the audio
	// where the beat will occur. The values array acts as a LUT, with index positions corresponding to BeatValue.
	// These values are relative to quarter beats (which have a value of 1).
	public struct BeatDecimalValues
    {
		private static float dottedBeatModifier = 1.5f;
		public static float[] values = {
			0f,
			4f, 4f/dottedBeatModifier,			// sixteenth, dotted sixteenth
			2f, 2f/dottedBeatModifier,			// eighth, dotted eighth
			1f, 1f/dottedBeatModifier,			// quarter, dotted quarter
			0.5f, 0.5f/dottedBeatModifier,		// half, dotted half
			0.25f, 0.25f/dottedBeatModifier		// whole, dotted whole
		};
	}

    // see: http://www.teachmeaudio.com/mixing/techniques/audio-spectrum
    public enum FrequencyRange
    {
        None       = 0,
        All        = ~0,
        SBass      = 1 << 0, // 20  - 60  Hz
        Bass       = 1 << 1, // 60  - 250 Hz
        LoMid      = 1 << 2, // 250 - 500 Hz
        Mid        = 1 << 3, // 0.5 - 2   kHz
        HiMid      = 1 << 4, // 2   - 4   kHz
        Pressence  = 1 << 5, // 4   - 6   kHz
        Brilliance = 1 << 6  // 6   - 20  kHz
    }

public struct FrequencyRangeValues
{
    public static Dictionary<FrequencyRange, float[]> values = new Dictionary<FrequencyRange, float[]>()
    {
        { FrequencyRange.SBass,      new float[]{   20f,    60f }},
        { FrequencyRange.Bass,       new float[]{   60f,   250f }},
        { FrequencyRange.LoMid,      new float[]{  250f,   500f }},
        { FrequencyRange.Mid,        new float[]{  500f,  2000f }},
        { FrequencyRange.HiMid,      new float[]{ 2000f,  4000f }},
        { FrequencyRange.Pressence,  new float[]{ 4000f,  6000f }},
        { FrequencyRange.Brilliance, new float[]{ 6000f, 20000f }}
    };
}
}
