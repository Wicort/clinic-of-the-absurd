using System;

[Serializable]
public class WardState
{
    public string doorId;

    public bool hasBeenEntered;

    public string assignedPatientAssetName;

    public bool isCured;

    public WardState(string doorId)
    {
        this.doorId = doorId;
        this.hasBeenEntered = false;
        this.assignedPatientAssetName = null;
        this.isCured = false;
    }
}
