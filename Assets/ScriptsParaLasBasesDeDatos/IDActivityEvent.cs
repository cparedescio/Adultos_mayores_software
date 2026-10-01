using System;
using System.Collections.Generic;
using UnityEngine.Events;
namespace ScriptsParaLasBasesDeDatos
{
    [System.Serializable]
    public class IDActivityEvent : UnityEvent<Tuple<string, bool>>
    {

    }
}
