using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Saving
{
    public interface IJsonSaveable
    {        /// <summary>
             /// Override to return a JToken representing the state of the IJsonSaveable
             /// </summary>
             /// <returns>A JToken</returns>
        JToken CaptureAsJToken();
        /// <summary>
        /// Restore the state of the component using the information in JToken.
        /// </summary>
        /// <param name="state">A JToken object representing the state of the module</param>
        void RestoreFromJToken(JToken state);

    }
}
