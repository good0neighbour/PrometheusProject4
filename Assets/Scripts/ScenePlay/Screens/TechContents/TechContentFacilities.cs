using UnityEngine;

public class TechContentFacilities : TechContentBase
{
    /* ==================== Protected Methods ==================== */

    protected override GameObject GetSampleNode()
    {
        return Resources.Load<GameObject>("Prefabs/NodeFacility");
    }


    protected override TechTreeData GetTechTreeData()
    {
        return Resources.Load<TechTreeData>("TechTrees/Facilities");
    }
}
