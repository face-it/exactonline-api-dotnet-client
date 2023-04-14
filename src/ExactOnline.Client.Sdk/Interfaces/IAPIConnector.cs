using System.IO;
using System.Threading.Tasks;

namespace ExactOnline.Client.Sdk.Interfaces
{
	public interface IApiConnector
	{
		Task<string> DoGetRequest(string endpoint, string parameters);
	    Task<Stream> DoGetFileRequest(string endpoint);


        Task<string> DoPostRequest(string endpoint, string postdata);

		Task<string> DoPutRequest(string endpoint, string putData);

		Task<string> DoDeleteRequest(string endpoint);

		Task<string> DoCleanRequest(string uri); // Request without Content-Type for $count function
		Task<string> DoCleanRequest(string uri, string oDataQuery); // Request without Content-Type for $count function, including parameters

		Task<int> GetCurrentDivision(string website);
	}
}
