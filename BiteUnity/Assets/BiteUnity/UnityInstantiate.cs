using System.Collections.Generic;
using Bite.Runtime.Functions;
using Bite.Runtime.Memory;
using UnityEngine;

namespace BiteUnity
{

public class UnityInstantiate: IBiteVmCallable
{
    public object Call( List < DynamicBiteVariable > arguments )
    {
        if ( arguments.Count == 2 )
        {
            arguments.Reverse();
            GameObject returnVal = GameObject.Instantiate( (GameObject)arguments[0].ToObject() );
            returnVal.name = arguments[1].StringData;

            return returnVal;
        }
        else
        {
            GameObject returnVal = GameObject.Instantiate( (GameObject)arguments[0].ToObject() );
            return returnVal;
        }
    }

    public object Call( DynamicBiteVariable[] arguments )
    {
        if ( arguments.Length == 2 )
        {
            GameObject returnVal = GameObject.Instantiate( (GameObject)arguments[1].ToObject() );
            returnVal.name = arguments[0].StringData;

            return returnVal;
        }
        else
        {
            GameObject returnVal = GameObject.Instantiate( (GameObject)arguments[0].ToObject() );
            return returnVal;
        }
    }
}

}
