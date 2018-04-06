using Prism.Modularity;
using Prism.Regions;

namespace GraphEngine.TestFacilityConsole.UIInfrastructure.UI.ShellServices
{
    public interface IModuleBase : IModule
    {
        void Initialize(IRegionManager theScopedRegionManager);
    }
}