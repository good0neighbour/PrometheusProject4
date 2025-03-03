using UnityEngine;

public class TechContentFacilities : TechContentBase
{
    /* ==================== Public Methods ==================== */

    public override void Adopt(byte index)
    {
        // City
        TechElement element = Nodes[index].Data;
        if (element.Maintenance > 0.0f)
        {
            CitiesInfo.Instance.CurrentCity.AnnualFund -= element.Maintenance;
        }
        if (element.Population > 0.0f)
        {
            CitiesInfo.Instance.CurrentCity.PopulationMovement += element.Population;
        }

        // Adopt
        TechTreeScreen.Instance.NodeStatus[(int)TechTreeType.Facilities][index] = TechStatus.Adopted;

        // Global
        base.Adopt(index);
    }



    /* ==================== Protected Methods ==================== */

    protected override void UnlockCheck(TechElement.ElementLink[] unlocks, Vector2 nodePos)
    {
        if (unlocks != null)
        {
            for (byte i = 0; i < unlocks.Length; ++i)
            {
                switch (unlocks[i].Type)
                {
                    case TechTreeType.Facilities:
                        if (RequirementsCheck(
                            TechTreeScreen.Instance.GetRequirements(TechTreeType.Facilities, unlocks[i].Index)
                        ))
                        {
                            // Node status
                            TechTreeScreen.Instance.NodeStatus[(int)TechTreeType.Facilities][unlocks[i].Index] = TechStatus.Available;
                            Nodes[unlocks[i].Index].TechNode.SetNodeStatus(TechStatus.Available);

                            // Gets size.
                            GetMaxSize(nodePos);

                            // Sets content size.
                            transform.parent.GetComponent<RectTransform>().sizeDelta = ContentSize;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }


    protected override GameObject GetSampleNode()
    {
        return Resources.Load<GameObject>("Prefabs/NodeFacility");
    }


    protected override TechTreeData GetTechTreeData()
    {
        return Resources.Load<TechTreeData>("TechTrees/Facilities");
    }
}
