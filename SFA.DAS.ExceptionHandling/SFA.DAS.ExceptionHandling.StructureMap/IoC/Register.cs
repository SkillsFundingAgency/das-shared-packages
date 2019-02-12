using StructureMap;

namespace SFA.DAS.ExceptionHandling.StructureMap.IoC
{
    public class ExceptionHandlingRegister : Registry
    {
        public ExceptionHandlingRegister()
        {
            base.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AddAllTypesOf<IExceptionMessageFormatter>();
            });
        }
    }
}
