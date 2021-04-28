using System;
using System.Collections;
using UoB.Core.Structure;
using UoB.Core.ForceField.Definitions;
using UoB.Core.ForceField;
using UoB.Core.Primitives;
using UoB.Core.FileIO.PDB;

using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for AtomWrap.
	/// </summary>
	public class AtomDrawWrapper : ColouredVector
	{
		public Atom m_Atom;
		public Label atomLabel;
		private bool m_ShouldDisplay = true;
		public bool isCAlpha;
		private AtomDrawStyle m_DrawStyle;
		private AtomDisplayMode m_DisplayMode;
		private ImagingDetails m_Imaging;

		public AtomDrawWrapper( Atom a, Position psCenter ) : base( a )
		{
			x -= psCenter.x;
			y -= psCenter.y;
			z -= psCenter.z;

			m_Imaging = FFManager.Instance.Imaging;
			m_Atom = a;
			colour = new Colour(0,0,0);

			isCAlpha = ( m_Atom.PDBType == PDBAtom.PDBID_BackBoneCA );
			setDefaultColour(); 
			atomLabel = new Label( a.AtomNumber.ToString(), this );
			DrawStyle = AtomDrawStyle.Lines;
		}

		public void reGetAtomPosition( Position centerPos )
		{
			x = m_Atom.x - centerPos.x;
			y = m_Atom.y - centerPos.y;
			z = m_Atom.z - centerPos.z;
		}

		public AtomDisplayMode DisplayMode
		{
			get
			{
				return m_DisplayMode;
			}
			set
			{
				m_DisplayMode = value;
				setUpShouldDisplay();
			}
		}

		public AtomDrawStyle DrawStyle
		{
			get
			{
				return m_DrawStyle;
			}
			set
			{
				m_DrawStyle = value;
			}
		}

		public bool ShouldDisplay
		{
			get
			{
				return m_ShouldDisplay;
			}
		}

		private void setUpShouldDisplay()
		{
			switch( m_DisplayMode ) // set the visibility for each atom
			{
				case AtomDisplayMode.AllAtoms:
					m_ShouldDisplay = true;
					break;
				case AtomDisplayMode.CAlphaTrace:
					m_ShouldDisplay = isCAlpha;
					break;
				case AtomDisplayMode.HeavyAtom:
					m_ShouldDisplay = ( m_Atom.atomPrimitive.Element != 'H' );
					break;
				case AtomDisplayMode.Backbone:
					m_ShouldDisplay = m_Atom.atomPrimitive.IsBackBone;
					break;
				case AtomDisplayMode.Invisible:
					m_ShouldDisplay = false;
					break;
				default:
					m_ShouldDisplay = true;
					break;
			}
		}

		public void setDefaultColour()
		{
			m_Imaging.SetAtomColour( m_Atom, colour );
		}
	}
}
