namespace SFA.DAS.Hmrc.Http
{
    public class ResourceNotFoundException : HttpException
    {
        public ResourceNotFoundException(string resourceUri)
            : base(404, "Could not find requested resource - " + resourceUri)
        {
        }
    }
}