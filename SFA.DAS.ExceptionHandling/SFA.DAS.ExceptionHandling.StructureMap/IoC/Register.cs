using StructureMap;

namespace SFA.DAS.ExceptionHandling.StructureMap.IoC
{
    public class Register : Registry
    {
        public Register()
        {
            base.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AddAllTypesOf<IExceptionMessageFormatter>();
            });
        }
    }
}
