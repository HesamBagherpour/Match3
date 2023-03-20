using Garage.Core.DI;
using UnityEngine;

namespace HB.Core.Controllers
{
   
    public abstract class Controller : MonoBehaviour
    {
        protected static IContext Context;
        
        protected virtual void Awake()
        {

        }

    }
}
