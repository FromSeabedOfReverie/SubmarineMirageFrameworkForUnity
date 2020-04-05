//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define BGM
//#define BGS
//#define JINGLE
//#define VOICE
//#define LOOP_SE
//#define SE
namespace SubmarineMirage.Test {
	using UniRx;
	using Process;
	using Audio;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 音の試験クラス
	///----------------------------------------------------------------------------------------------------
	///		キー入力で、音が鳴るテストを行う。
	/// </summary>
	///====================================================================================================
	public class TestAudio : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void Constructor() {
			base.Constructor();

#if BGM
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.BGM.TestTitle}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.BGM.TestTitle );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.BGM.TestBattle}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.BGM.TestBattle );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.BGM>();
			} );
#endif
#if BGS
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.BGS.TestWater}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.BGS.TestWater );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.BGS.TestWind}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.BGS.TestWind );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.BGS>();
			} );
#endif
#if JINGLE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.Jingle.TestGameClear}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.Jingle.TestGameClear );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.Jingle.TestGameOver}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.Jingle.TestGameOver );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.Jingle>();
			} );
#endif
#if VOICE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.Voice.TestRidicule}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.Voice.TestRidicule );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.Voice.TestScream}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.Voice.TestScream );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.Voice>();
			} );
#endif
#if LOOP_SE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.LoopSE.TestTalk1}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.LoopSE.TestTalk1 );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.LoopSE.TestTalk2}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.LoopSE.TestTalk2 );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.LoopSE>();
			} );
#endif
#if SE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.SE.TestDecision}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.SE.TestDecision );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {GameAudioManager.SE.TestGun}", Log.Tag.Audio );
				GameAudioManager.s_instance.Play( GameAudioManager.SE.TestGun );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				GameAudioManager.s_instance.Stop<GameAudioManager.SE>();
			} );
#endif
		}
	}
}