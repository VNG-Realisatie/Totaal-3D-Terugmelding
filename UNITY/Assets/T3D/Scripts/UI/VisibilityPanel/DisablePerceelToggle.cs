using System.Collections;
using System.Collections.Generic;
using T3D.Uitbouw;
using UnityEngine;

public class DisablePerceelToggle : UIToggle
{
    protected override void ToggleAction(bool active)
    {
        RestrictionChecker.ActivePerceel.SetPerceelOutlineActive(active);
    }
}
