using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "PrometheusMission/SaveData")]
public class SaveData : ScriptableObject
{
    public PlayManager.PlayData Variables;
    public List<Land> Lands;
}
