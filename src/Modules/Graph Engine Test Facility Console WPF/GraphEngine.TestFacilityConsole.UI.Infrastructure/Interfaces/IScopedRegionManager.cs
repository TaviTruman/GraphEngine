using Prism.Regions;

namespace GraphEngine.TestFacilityConsole.UIInfrastructure.Interfaces
{
    public interface IScopedRegionManager : IRegionManager
    {
        IRegionManager ScopedRegionManager { get; set; }
    }
}