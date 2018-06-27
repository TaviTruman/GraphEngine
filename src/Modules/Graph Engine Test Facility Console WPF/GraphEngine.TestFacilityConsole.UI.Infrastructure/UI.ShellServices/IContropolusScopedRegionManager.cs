using Prism.Regions;

namespace GraphEngine.TestFacilityConsole.UIInfrastructure.UI.ShellServices
{
    public interface IContropolusScopedRegionManager : IRegionManager
    {
        IRegionManager ScopedRegionManager { get; set; }
    }
}