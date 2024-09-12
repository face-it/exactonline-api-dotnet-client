using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExactOnline.Client.Sdk.Helpers;
using ExactOnline.Client.Sdk.Models;

namespace ExactOnline.Client.Sdk.Controllers
{
    using ExactOnline.Client.Models.Current;

    /// <summary>
	/// Front Controller for working with Exact Online Entities
	/// </summary>
	public class ExactOnlineClient
	{
		private readonly ApiConnector _apiConnector;

		// https://start.exactonline.nl/api/v1
		private readonly string _exactOnlineApiUrl;

		private ControllerList _controllers;
		private int _division;

        public EolResponseHeader EolResponseHeader { get; internal set; }

        #region Constructors

        /// <summary>
        /// Create instance of ExactClient
        /// </summary>
        /// <param name="exactOnlineUrl">The Exact Online URL for your country</param>
        /// <param name="accesstokenDelegate">Delegate that will be executed the access token is expired</param>
        /// <param name="refreshTokenDelegate">Delegate that will retrieve the amount of retries (a counter), and should return true if the token is refreshed</param>
        /// <param name="delayFunc"></param>
        public ExactOnlineClient(string exactOnlineUrl, Func<Task<string>> accesstokenDelegate, Func<int, Task<bool>> refreshTokenDelegate = null, Action<double> delayFunc = null)
		{
			_apiConnector = new ApiConnector(accesstokenDelegate, this, refreshTokenDelegate, delayFunc);

			if (!exactOnlineUrl.EndsWith("/")) exactOnlineUrl += "/";
			_exactOnlineApiUrl = exactOnlineUrl + "api/v1/";
		}

        public async Task Initialize(int division)
        {
            _division = (division > 0) ? division : await GetDivision();

            var serviceRoot = _exactOnlineApiUrl + _division + "/";
            _controllers = new ControllerList(_apiConnector, serviceRoot);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the current user data
        /// </summary>
        /// <returns>Me entity</returns>
        public async Task<Me> CurrentMe()
		{
			var conn = new ApiConnection(_apiConnector, _exactOnlineApiUrl + "current/Me");
			var response = await conn.Get("");
			response = ApiResponseCleaner.GetJsonArray(response);
			var converter = new EntityConverter();
			var currentMe = converter.ConvertJsonArrayToObjectList<Me>(response);
			return currentMe.FirstOrDefault();
        }

        /// <summary>
        /// returns the attachment for the given url
        /// </summary>
        /// <returns>Stream</returns>
        public async Task<Stream> GetAttachment(string url)
        {
            if (_controllers == null)
                await Initialize(0);

            var conn = new ApiConnection(_apiConnector, url);
            return await conn.GetFile();
        }

        /// <summary>
        /// return the division number of the current user
        /// </summary>
        /// <returns>Division number</returns>
        private async Task<int> GetDivision()
		{
			if (_division > 0)
			{
				return _division;
			}

			var currentMe = await CurrentMe();
			if (currentMe != null)
			{
				_division = currentMe.CurrentDivision;
				return _division;
			}

			throw new Exception("Cannot get division. Please specify division explicitly via the constructor.");
		}

		/// <summary>
		/// Returns instance of ExactOnlineQuery that can be used to manipulate data in Exact Online
		/// </summary>
		public ExactOnlineQuery<T> For<T>() where T : class
		{
			var controller = _controllers.GetController<T>();
			return new ExactOnlineQuery<T>(controller);
		}

		#endregion
	}
}