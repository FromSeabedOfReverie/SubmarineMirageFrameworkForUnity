//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor {
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;



	public class SMServiceLocatorEditor : EditorWindowSMExtension {
		Vector2 _servicesScrollPosition	{ get; set; }
		Vector2 _detailScrollPosition { get; set; }
		string _focusedText	{ get; set; } = string.Empty;


		public override void Dispose() {}


		[MenuItem( "Window/SubmarineMirage/SMServiceLocator" )]
		static void Open() => GetWindow<SMServiceLocatorEditor>();


		void OnGUI() {
			var name = nameof( SMServiceLocator );
			titleContent = new GUIContent( name );
			ShowHeading1( name );

			if ( SMServiceLocator.s_isDisposed ) {
				ShowDispose();
				return;
			}

			_servicesScrollPosition = EditorGUILayout.BeginScrollView( _servicesScrollPosition );
			ShowServices();
			EditorGUILayout.EndScrollView();

			_detailScrollPosition = EditorGUILayout.BeginScrollView( _detailScrollPosition );
			ShowDetail();
			EditorGUILayout.EndScrollView();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}

		void ShowDispose() {
			ShowHeading2( nameof( SMServiceLocator.s_isDisposed ) );
		}

		void ShowServices() {
			ShowHeading2( nameof( SMServiceLocator.s_container ) );
			EditorGUI.indentLevel++;

			SMServiceLocator.s_container.ForEach( pair => {
				GUI.SetNextControlName( pair.Value.ToString() );
				EditorGUILayout.SelectableLabel(
					$"{pair.Key.GetName()} : {pair.Value.GetName()}",
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