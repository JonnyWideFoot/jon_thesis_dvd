using System;
using UoB.Core;
using System.Drawing;
using System.Diagnostics;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;

namespace UoB.CoreControls.OpenGLView
{
	/// <summary>
	/// Summary description for Perspective.
	/// </summary>
	public class Perspective
	{
		protected MatrixRotation m_GlobalMatrixRotation;
		protected MatrixRotation m_ChangesMatrixRotation;
		protected MatrixRotation m_CurrentMatrixRotation;
		private float m_ZoomPercentage;
		private Position m_RenderOffset = new Position();
		private Position m_RotationOffset = new Position();
		private bool changes = false;

		public event EventHandler PerspectiveUpdate;

		public Perspective( int initialZoom )
		{
			m_GlobalMatrixRotation = new MatrixRotation(); // holds the total transform for the viewer
			m_ChangesMatrixRotation = new MatrixRotation(); // while the mouse is movning, this stores the rotation changes
			m_CurrentMatrixRotation = new MatrixRotation(); // holds the transform of the rotation start point
			m_ZoomPercentage = initialZoom;
			
			PerspectiveUpdate = new EventHandler( nullfunc );
		}

		public bool hasChanges
		{
			get
			{
				return changes;
			}
		}

		public MatrixRotation GlobalRotMat
		{
			get
			{
				return m_GlobalMatrixRotation;
			}
			set
			{
				MatrixRotation donor = value;
				m_ChangesMatrixRotation.setToIdentity();
				m_CurrentMatrixRotation.setTo( donor.r );
				m_GlobalMatrixRotation.setTo( donor.r );
			}
		}

		public void Transform( Double[] points )
		{
			//Debug.WriteLine("Performing total transform");
			m_GlobalMatrixRotation.transform( points );
		}

		private void nullfunc( object Sender, EventArgs args )
		{
		}

		public Position RenderOffset
		{
			get
			{
				return m_RenderOffset;
			}
		}

		public Position RotationOffset
		{
			get
			{
				return m_RotationOffset;
			}
			set
			{
				m_RotationOffset = value;
			}
		}

		public void RenderTranslate( float xChange, float yChange )
		{
			m_RenderOffset.x += xChange;
			m_RenderOffset.y += yChange;
			PerspectiveUpdate( this, new EventArgs() );
		}

		public void ApplyChanges()
		{
			if ( changes )
			{
				m_CurrentMatrixRotation.doPreMultiply( m_ChangesMatrixRotation );
				m_ChangesMatrixRotation.setToIdentity();
				changes = false;
				PerspectiveUpdate( this, new EventArgs() );
			}
		}

		public void RotateTick( MatrixRotation changesRot )
		{
			m_CurrentMatrixRotation.doPreMultiply( changesRot );
			m_GlobalMatrixRotation.setToMultiplyMatrix( m_ChangesMatrixRotation, m_CurrentMatrixRotation );
			PerspectiveUpdate( this, new EventArgs() );
		}

		public void setAngleChanges ( double changeScreenX, double changeScreenY, double originalX, double originalY )
		{
			changes = true;
			double[] axis = new double[3];

			axis[0] = changeScreenY - originalY;
			axis[1] = changeScreenX - originalX;
			axis[2] = 0.0;

			double angle = 0.008 * Math.Sqrt( axis[0] * axis[0] + axis[1] * axis[1] );
			m_ChangesMatrixRotation.setToAxisRot( axis, angle );
			m_GlobalMatrixRotation.setToMultiplyMatrix( m_ChangesMatrixRotation, m_CurrentMatrixRotation );

			PerspectiveUpdate( this, new EventArgs() );
		}

		public void setAngleChanges( double changeScreenX, double originalZ )
		{
			changes = true;
			double[] axis = new double[3];

			axis[0] = 0.0;
			axis[1] = 0.0;
			axis[2] = changeScreenX - originalZ;
			double angle = 0.008 * axis[2];
			if( axis[2] < 0.0 )
			{
				axis[2] = -axis[2];
			}
			m_ChangesMatrixRotation.setToAxisRot( axis, angle );
			m_GlobalMatrixRotation.setToMultiplyMatrix( m_ChangesMatrixRotation, m_CurrentMatrixRotation );

			PerspectiveUpdate( this, new EventArgs() );
		}

		public float ZoomPercentage
		{
			get
			{
				return m_ZoomPercentage;
			}
			set
			{
				m_ZoomPercentage = value;
				PerspectiveUpdate( this, new EventArgs() );
			}
		}
	}
}