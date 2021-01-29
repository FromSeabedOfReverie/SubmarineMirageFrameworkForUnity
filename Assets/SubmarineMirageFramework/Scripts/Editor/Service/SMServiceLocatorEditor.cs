//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorService {
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;
	using Service;
	using Extension;
	using EditorExtension;



	// TODO : コメント追加、整頓



	public class SMServiceLocatorEditor : EditorWindowSMExtension {
		Vector2 _scrollPosition	{ get; set; }
		string _focusedText	{ get; set; } = string.Empty;


		public override void Dispose() {}


		[MenuItem( "Window/SubmarineMirage/ServiceLocator" )]
		static void Open() => GetWindow<SMServiceLocatorEditor>();


		void OnGUI() {
			var name = nameof( SMServiceLocator );
			titleContent = new GUIContent( name );
			ShowHeading1( name );

			if ( SMServiceLocator.s_isDisposed ) {
				ShowDispose();
				return;
			}

			_scrollPosition = EditorGUILayout.BeginScrollView( _scrollPosition );
			ShowContainer();
			EditorGUILayout.EndScrollView();
			ShowDetail();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}

		void ShowDispose() {
			ShowHeading2( nameof( SMServiceLocator.s_isDisposed ) );
		}

		void ShowContainer() {
			ShowHeading2( nameof( SMServiceLocator.s_container ) );
			EditorGUI.indentLevel++;

			SMServiceLocator.s_container.ForEach( pair => {
				GUI.SetNextControlName(
					pair.Value.GetAboutName()

// TODO : ToStringで無限ループ
//						pair.Value.ToString()

				);
				EditorGUILayout.SelectableLabel(
					$"{pair.Key.GetAboutName()} : {pair.Value.GetAboutName()}",
					GUILayout.Height( 16 )
				);
			} );

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		void ShowDetail() {
			ShowLine();
			ShowHeading2( "Detail" );

			EditorGUI.indentLevel++;
			_focusedText.Split( "\n" )
				.ForEach( s => EditorGUILayout.LabelField( s ) );
			EditorGUI.indentLevel--;
		}

		void ShowLine() {
			GUILayout.Box(
				"",
				GUILayout.Width( EditorGUIUtility.currentViewWidth - 30 ),
				GUILayout.Height( 1 )
			);
		}
		void ShowHeading1( string text ) {
			EditorGUILayout.LabelField( $"● {text}", EditorStyles.boldLabel );
		}
		void ShowHeading2( string text ) {
			EditorGUILayout.LabelField( $"・{text}", EditorStyles.boldLabel );
		}
	}
}