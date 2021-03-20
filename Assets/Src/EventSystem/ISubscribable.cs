using UnityEngine;
using UnityEditor;

namespace Wizard.Events
{
    public interface ISubscribable
    {
        void Unsubscribe();
    }
}