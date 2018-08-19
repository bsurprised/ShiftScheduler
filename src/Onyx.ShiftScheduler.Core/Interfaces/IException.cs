using System.Net;

namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IException
    {
        string Error { get; set; }

        HttpStatusCode StatusCode { get; set; }
    }
}