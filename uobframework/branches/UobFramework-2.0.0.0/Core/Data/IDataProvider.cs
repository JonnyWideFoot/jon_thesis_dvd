using System;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for IDataProvider.
	/// </summary>
	public interface IDataProvider
	{
		DataManager DataStore
		{
			get;
		}
	}
}
