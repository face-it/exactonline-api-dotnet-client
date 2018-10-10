using System;
using System.IO;
using System.Linq;
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

		private readonly ControllerList _controllers;
		private int _division;

        public EolResponseHeader EolResponseHeader { get; internal set; }

        #region Constructors

	    /// <summary>
	    /// Create instance of ExactClient
	    /// </summary>
	    /// <param name="exactOnlineUrl">The Exact Online URL for your country</param>
	    /// <param name="division">Division number</param>
	    /// <param name="accesstokenDelegate">Delegate that will be executed the access token is expired</param>
	    /// <param name="refreshTokenDelegate">Delegate that will retrieve the amount of retries (a counter), and should return true if the token is refreshed</param>
	    public ExactOnlineClient(string exactOnlineUrl, int division, Func<string> accesstokenDelegate, Func<int, bool> refreshTokenDelegate = null)
		{
			// Set culture for correct deserializing of API Response (comma and points)
			_apiConnector = new ApiConnector(accesstokenDelegate, this, refreshTokenDelegate);
			//Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			if (!exactOnlineUrl.EndsWith("/")) exactOnlineUrl += "/";
			_exactOnlineApiUrl = exactOnlineUrl + "api/v1/";

			_division = (division > 0) ? division : GetDivision();
			string serviceRoot = _exactOnlineApiUrl + _division + "/";

			_controllers = new ControllerList(_apiConnector, serviceRoot);
		}

		/// <summary>
		/// Create instance of ExactClient
		/// </summary>
		/// <param name="exactOnlineUrl">{URI}/</param>
		/// <param name="accesstokenDelegate">Valid oAuth AccessToken</param>
		public ExactOnlineClient(string exactOnlineUrl, Func<string> accesstokenDelegate)
			: this(exactOnlineUrl, 0, accesstokenDelegate)
		{
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the current user data
		/// </summary>
		/// <returns>Me entity</returns>
		public Me CurrentMe()
		{
			var conn = new ApiConnection(_apiConnector, _exactOnlineApiUrl + "current/Me");
			var response = conn.Get("");
			response = ApiResponseCleaner.GetJsonArray(response);
			var converter = new EntityConverter();
			var currentMe = converter.ConvertJsonArrayToObjectList<Me>(response);
			return currentMe.FirstOrDefault();
        }

        /// <summary>
        /// returns the attachment for the given url
        /// </summary>
        /// <returns>Stream</returns>
        public Stream GetAttachment(string url)
        {
            var conn = new ApiConnection(_apiConnector, url);
            return conn.GetFile();
        }

        /// <summary>
        /// return the division number of the current user
        /// </summary>
        /// <returns>Division number</returns>
        public int GetDivision()
		{
			if (_division > 0)
			{
				return _division;
			}

			var currentMe = CurrentMe();
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