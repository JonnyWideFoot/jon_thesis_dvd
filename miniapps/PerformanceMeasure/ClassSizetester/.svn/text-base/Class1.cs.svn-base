using System;

namespace ConsoleApplication2
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			

			Class1 s = new Class1();

		}

		public Class1()
		{
			DateTime start;
			DateTime end;
			TimeSpan timeSpent;

			bool random = false;
			int count = 0;
			while(true)
			{
				random = !random;

				start = DateTime.Now;
				//doFloat(random);
				doFloat2();
				end = DateTime.Now;
				timeSpent = end-start;
				Console.WriteLine("Simple float time: " + timeSpent.ToString() + " Rand : " + random.ToString() );

				start = DateTime.Now;
				//doDouble(random);
				doDouble2();
				end = DateTime.Now;
				 timeSpent = end-start;
				Console.WriteLine("Simple double time: " + timeSpent.ToString() + " Rand : " + random.ToString()  );

				start = DateTime.Now;
				//doFloatBig(random);
				doFloatBig2();
				end = DateTime.Now;
				timeSpent = end-start;
				Console.WriteLine("Simple float big time: " + timeSpent.ToString() + " Rand : " + random.ToString()  );

				start = DateTime.Now;
				//doDoubleBig(random);
				doDoubleBig2();
				end = DateTime.Now;
				timeSpent = end-start;
				Console.WriteLine("Simple double big time: " + timeSpent.ToString() + " Rand : " + random.ToString()  );
			
				count++;

				if( count == 3 )
				{
					Console.ReadLine();
				}
			}
		}

		private int creationNum = 10000;
		private int repeatNum = 1000;
		private Random m_Rand = new Random();

		private void doDouble( bool random )
		{
			miniClassDouble[] doubles = new miniClassDouble[ creationNum ];
			for( int i = 0; i < doubles.Length; i++ )
			{
				doubles[i] = new miniClassDouble( 4.3f, 8.9f, 43.2f );
			}

			if( random )
			{
				for( int j = 0; j < repeatNum; j++ )
				{			   
					double f = 0.0;
					for( int i = 0; i < doubles.Length; i++ )
					{
						f += doubles[m_Rand.Next(creationNum)].d1;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						f -= doubles[m_Rand.Next(creationNum)].d2;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						f += doubles[m_Rand.Next(creationNum)].d3;
					}
				}
			}
			else
			{
				for( int j = 0; j < repeatNum; j++ )
				{				   
					double f = 0.0;
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += doubles[i].d1;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f -= doubles[i].d2;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += doubles[i].d3;
					}
				}
			}
		}

		private void doFloat( bool random )
		{
			miniClassFloat[] floats = new miniClassFloat[ creationNum ];
			for( int i = 0; i < floats.Length; i++ )
			{
				floats[i] = new miniClassFloat( 4.3f, 8.9f, 43.2f );
			}


			if( random )
			{
				for( int j = 0; j < repeatNum; j++ )
				{			   
					float f = 0.0f;
					for( int i = 0; i < floats.Length; i++ )
					{
						f += floats[m_Rand.Next(creationNum)].d1;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						f -= floats[m_Rand.Next(creationNum)].d2;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						f += floats[m_Rand.Next(creationNum)].d3;
					}
				}
			}
			else
			{
				for( int j = 0; j < repeatNum; j++ )
				{				   
					double f = 0.0;
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += floats[i].d1;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f -= floats[i].d2;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += floats[i].d3;
					}
				}
			}
		}

		private void doDoubleBig( bool random )
		{
			bigClassDouble[] doubles = new bigClassDouble[ creationNum ];
			for( int i = 0; i < doubles.Length; i++ )
			{
				doubles[i] = new bigClassDouble( 4.3, 8.9, 43.2 );
			}

			if( random )
			{
				for( int j = 0; j < repeatNum; j++ )
				{			   
					double f = 0.0;
					for( int i = 0; i < doubles.Length; i++ )
					{
						f += doubles[m_Rand.Next(creationNum)].d1;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						f -= doubles[m_Rand.Next(creationNum)].d2;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						f += doubles[m_Rand.Next(creationNum)].d3;
					}
				}
			}
			else
			{
				for( int j = 0; j < repeatNum; j++ )
				{				   
					double f = 0.0;
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += doubles[i].d1;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f -= doubles[i].d2;
					}
					for( int i = 0; i < doubles.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += doubles[i].d3;
					}
				}
			}
		}

		private void doFloatBig( bool random )
		{
			bigClassFloat[] floats = new bigClassFloat[ creationNum ];
			for( int i = 0; i < floats.Length; i++ )
			{
				floats[i] = new bigClassFloat( 4.3f, 8.9f, 43.2f );
			}


			if( random )
			{
				for( int j = 0; j < repeatNum; j++ )
				{			   
					float f = 0.0f;
					for( int i = 0; i < floats.Length; i++ )
					{
						f += floats[m_Rand.Next(creationNum)].d1;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						f -= floats[m_Rand.Next(creationNum)].d2;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						f += floats[m_Rand.Next(creationNum)].d3;
					}
				}
			}
			else
			{
				for( int j = 0; j < repeatNum; j++ )
				{				   
					double f = 0.0;
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += floats[i].d1;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f -= floats[i].d2;
					}
					for( int i = 0; i < floats.Length; i++ )
					{
						m_Rand.Next(creationNum);
						f += floats[i].d3;
					}
				}
			}
		}
	

	
		private void doDouble2()
		{
			miniClassDouble[] doubles = new miniClassDouble[ creationNum ];
			for( int i = 0; i < doubles.Length; i++ )
			{
				doubles[i] = new miniClassDouble( 4.3f, 8.9f, 43.2f );
			}

			for( int j = 0; j < repeatNum; j++ )
			{				   
				double f = 0.0;
				for( int i = 0; i < doubles.Length; i++ )
				{
					f += doubles[i].d1;
				}
				for( int i = 0; i < doubles.Length; i++ )
				{
					f -= doubles[i].d2;
				}
				for( int i = 0; i < doubles.Length; i++ )
				{
					f += doubles[i].d3;
				}
			}
		}

		private void doFloat2()
		{
			miniClassFloat[] floats = new miniClassFloat[ creationNum ];
			for( int i = 0; i < floats.Length; i++ )
			{
				floats[i] = new miniClassFloat( 4.3f, 8.9f, 43.2f );
			}

			for( int j = 0; j < repeatNum; j++ )
			{				   
				double f = 0.0;
				for( int i = 0; i < floats.Length; i++ )
				{
					f += floats[i].d1;
				}
				for( int i = 0; i < floats.Length; i++ )
				{
					f -= floats[i].d2;
				}
				for( int i = 0; i < floats.Length; i++ )
				{
					f += floats[i].d3;
				}
			}
		}

		private void doDoubleBig2()
		{
			bigClassDouble[] doubles = new bigClassDouble[ creationNum ];
			for( int i = 0; i < doubles.Length; i++ )
			{
				doubles[i] = new bigClassDouble( 4.3, 8.9, 43.2 );
			}
			for( int j = 0; j < repeatNum; j++ )
			{				   
				double f = 0.0;
				for( int i = 0; i < doubles.Length; i++ )
				{
					f += doubles[i].d1;
				}
				for( int i = 0; i < doubles.Length; i++ )
				{
					f -= doubles[i].d2;
				}
				for( int i = 0; i < doubles.Length; i++ )
				{
					f += doubles[i].d3;
				}
			}
		}

		private void doFloatBig2()
		{
			bigClassFloat[] floats = new bigClassFloat[ creationNum ];
			for( int i = 0; i < floats.Length; i++ )
			{
				floats[i] = new bigClassFloat( 4.3f, 8.9f, 43.2f );
			}

			for( int j = 0; j < repeatNum; j++ )
			{				   
				double f = 0.0;
				for( int i = 0; i < floats.Length; i++ )
				{
					f += floats[i].d1;
				}
				for( int i = 0; i < floats.Length; i++ )
				{
					f -= floats[i].d2;
				}
				for( int i = 0; i < floats.Length; i++ )
				{
					f += floats[i].d3;
				}
			}
		}
	}

	class bigClassFloat : miniClassFloat
	{
		private string m_String = "bla";
		private int m_Indexer = 545;
		private string m_AnotherString = "blooob";
		private char m_Charness = 'B';

		public bigClassFloat( float d1, float d2, float d3 ) : base ( d1, d2, d3 )
		{
		}
	}

	class bigClassDouble : miniClassDouble
	{
		private string m_String = "bla";
		private int m_Indexer = 545;
		private string m_AnotherString = "blooob";
		private char m_Charness = 'B';

		public bigClassDouble( double d1, double d2, double d3 ) : base ( d1, d2, d3 )
		{
		}
	}

	class miniClassFloat
	{
		private float m_1;
		private float m_2;
		private float m_3;

		public miniClassFloat( float d1, float d2, float d3 )
		{
			m_1 = d1;
			m_2 = d2;
			m_3 = d3;
		}

		public float d1
		{
			get
			{
				return m_1;
			}
		}

		public float d2
		{
			get
			{
				return m_2;
			}
		}

		public float d3
		{
			get
			{
				return m_3;
			}
		}
	}

	class miniClassDouble
	{
		private double m_1;
		private double m_2;
		private double m_3;

		public miniClassDouble( double d1, double d2, double d3 )
		{
			m_1 = d1;
			m_2 = d2;
			m_3 = d3;
		}

		public double d1
		{
			get
			{
				return m_1;
			}
		}

		public double d2
		{
			get
			{
				return m_2;
			}
		}

		public double d3
		{
			get
			{
				return m_3;
			}
		}
	}
}
