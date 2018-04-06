using Prism.Modularity;
using Prism.Regions;

namespace GraphEngine.TestFacilityConsole.UIInfrastructure.Interfaces
{
    public interface IModuleBase : IModule
    {
        void Initialize(IRegionManager theScopedRegionManager);
    }
}