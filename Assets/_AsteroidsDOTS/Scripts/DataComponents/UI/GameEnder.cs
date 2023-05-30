using _AsteroidsDOTS.Scripts.NonDOTSBehaviour.UI;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.UI
{
    [GenerateAuthoringComponent]
    public class GameEnder: IComponentData
    {
        public InGameUI InGameUI;

        public void SetPopup()
        {
            InGameUI.SetGameFinishedPopup();
        }
    }
}