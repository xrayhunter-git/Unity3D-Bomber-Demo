using UnityEngine;
using System.Collections;


//Utility script to assign members to a squad without messing up prefabs.
public class SquadMemberSetter : MonoBehaviour {

    public CaptainScript captain;
    public FormationFollower[] squadMembers;

    // Use this for initialization
    void Awake () {
        captain.squadMembers = squadMembers;
    }

}
