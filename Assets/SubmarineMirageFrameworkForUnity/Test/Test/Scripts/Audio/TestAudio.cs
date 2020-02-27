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
namespace SubmarineMirageFramework.Test.Audio {
	using UniRx;
	using SubmarineMirageFramework.Process;
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
				Log.Debug( $"Play {TestAudioManager.BGM.TestTitle}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.BGM.TestTitle );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.BGM.TestBattle}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.BGM.TestBattle );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.BGM>();
			} );
#endif
#if BGS
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.BGS.TestWater}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.BGS.TestWater );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.BGS.TestWind}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.BGS.TestWind );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.BGS>();
			} );
#endif
#if JINGLE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.Jingle.TestGameClear}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.Jingle.TestGameClear );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.Jingle.TestGameOver}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.Jingle.TestGameOver );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.Jingle>();
			} );
#endif
#if VOICE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.Voice.TestRidicule}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.Voice.TestRidicule );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.Voice.TestScream}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.Voice.TestScream );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.Voice>();
			} );
#endif
#if LOOP_SE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.LoopSE.TestTalk1}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.LoopSE.TestTalk1 );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.LoopSE.TestTalk2}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.LoopSE.TestTalk2 );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.LoopSE>();
			} );
#endif
#if SE
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.SE.TestDecision}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.SE.TestDecision );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				Log.Debug( $"Play {TestAudioManager.SE.TestGun}", Log.Tag.Audio );
				TestAudioManager.s_instance.Play( TestAudioManager.SE.TestGun );
			} );
			InputManager.s_instance.GetPressedEvent( InputManager.Event.Reset ).Subscribe( _ => {
				Log.Debug( "Stop", Log.Tag.Audio );
				TestAudioManager.s_instance.Stop<TestAudioManager.SE>();
			} );
#endif
		}
	}
}