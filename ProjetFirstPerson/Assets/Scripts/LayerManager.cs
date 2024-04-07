using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : GenericSingletonClass<LayerManager>
{
    public LayerMask groundLayer;
    public LayerMask playerGroundLayer;
}
