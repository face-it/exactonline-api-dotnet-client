using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExactOnline.Client.Sdk.Interfaces
{
	public interface IController<T>
	{
		Task<T> GetEntity(string guid, string parameters);

        Task<Tuple<bool, T>> Create(T entity);

		Task<bool> Update(T entity);

		Task<bool> Delete(T entity);

		Task<int> Count(string query); // For $count function API

		void RegistrateLinkedEntityField(string fieldname);
    
        Task<GetResult<T>> Get(string query, string skipToken);
	}

    public class GetResult<TModel>
    {
        public GetResult(List<TModel> result, string skipToken)
        {
            this.Result = result;
            this.SkipToken = skipToken;
        }

        public List<TModel> Result { get; }

        public string SkipToken { get; }
    }
}
