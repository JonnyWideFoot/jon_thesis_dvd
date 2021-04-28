using System;

namespace UoB.Methodology.OriginInteraction
{
	/// <summary>
	/// Summary description for PageType.
	/// </summary>
	public enum PageTypes
	{
		None = 0,  
		Dataset = 1,
		Worksheet = 2,
		Graph = 3,
		Variable = 4,
		Matrix = 5,
		Macro = 6,
		Tool = 7,
		Notes = 9,
		Layout = 11		
	}

    public enum ImageType
    {
        JPG,
        GIF,
        PNG,
        BMP,
        TIF
    }

	public enum GraphTypes
	{
		IDM_PLOT_SCATTER = 201
	}
}
