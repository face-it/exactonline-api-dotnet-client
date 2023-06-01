using System.Threading.Tasks;

namespace ExactOnline.Client.Sdk.Interfaces
{
	public interface IApiConnection
	{
		Task<string> Get(string parameters);

		Task<string> GetEntity(string keyname, string guid, string parameters);

		Task<string> Post(string data);

		Task<bool> Put(string keyName, string guid, string data);

		Task<bool> Delete(string keyName, string guid);

		Task<int> Count(string parameters);

	}
}
