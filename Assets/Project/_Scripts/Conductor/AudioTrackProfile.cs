using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AudioTrackProfile", menuName = "Music/AudioTrackProfile", order = 1)]
public class AudioTrackProfile : ScriptableObject
{
    public AudioClip audioClip;
    public uint bpm;
    public float offset;
    
    /*
     * The number of beat subdivisions of a beat
     */
    public uint beatSubdivision = 4;

    /*
     * How many beats in a bar
     */
    public uint beatPerBar = 4;
    
    public uint GetSubBPM()
    {
        return bpm * beatSubdivision;
    }
}