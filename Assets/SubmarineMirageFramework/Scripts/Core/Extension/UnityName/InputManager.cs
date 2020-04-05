//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 入力の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class InputManager : UnityName<InputManager, InputManager.Name> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>入力名</summary>
		public enum Name {
			/// <summary>移動用のX軸</summary>
			MoveAxisX,
			/// <summary>移動用のY軸</summary>
			MoveAxisY,
			/// <summary>回転用のX軸</summary>
			RotateAxisX,
			/// <summary>回転用のY軸</summary>
			RotateAxisY,
			/// <summary>デバッグ用のX軸</summary>
			DebugAxisX,
			/// <summary>デバッグ用のY軸</summary>
			DebugAxisY,

			/// <summary>視点スクロール</summary>
			CameraScroll,
			/// <summary>視点回転</summary>
			CameraRotate,
			/// <summary>行動</summary>
			Action,
			/// <summary>歩行</summary>
			Walk,
			/// <summary>決定</summary>
			Decide,
			/// <summary>終了</summary>
			Quit,
			/// <summary>リセット</summary>
			Reset,
			/// <summary>デバッグ表示</summary>
			DebugView,
		}
		/// <summary>軸</summary>
		public enum Axis {
			/// <summary>移動用の軸</summary>
			Move,
			/// <summary>回転用の軸</summary>
			Rotate,
			/// <summary>デバッグ用の軸</summary>
			Debug,
		}
		/// <summary>イベント関数定数</summary>
		public enum Event {
			/// <summary>決定キー</summary>
			Decide,
			/// <summary>終了キー</summary>
			Quit,
			/// <summary>再設定キー</summary>
			Reset,
			/// <summary>デバッグ表示</summary>
			DebugView,
			/// <summary>無に触れた</summary>
			Nothing,
			/// <summary>無に長時間触れた</summary>
			LongNothing,
			/// <summary>何らかの操作</summary>
			AnyOperation,
		}
		/// <summary>スワイプ状態</summary>
		// キーパッド数字と方向を対応定義
		public enum Swipe {
			/// <summary>左下にスワイプ</summary>
			DownerLeft = 1,
			/// <summary>下にスワイプ</summary>
			Down,
			/// <summary>右下にスワイプ</summary>
			DownerRight,
			/// <summary>左にスワイプ</summary>
			Left,
			/// <summary>スワイプ無し</summary>
			None,   // キーパッド5が中央の為、この位置で定義
			/// <summary>右にスワイプ</summary>
			Right,
			/// <summary>左上にスワイプ</summary>
			UpperLeft,
			/// <summary>上にスワイプ</summary>
			Up,
			/// <summary>右上にスワイプ</summary>
			UpperRight,
		}

		/// <summary>不動範囲の2乗</summary>
		float _noMoveSqrRange;
		/// <summary>スワイプ範囲の2乗</summary>
		float _swipeSqrRange;
		/// <summary>マウス位置</summary>
		public Vector2 _mousePosition	{ get; private set; }
		/// <summary>各種軸</summary>
		readonly Dictionary<Axis, Vector2> _axes = new Dictionary<Axis, Vector2> {
			{ Axis.Move,	Vector2.zero },
			{ Axis.Rotate,	Vector2.zero },
			{ Axis.Debug,	Vector2.zero },
		};
		/// <summary>各種押下中イベント</summary>
		readonly Dictionary< Event, ReactiveProperty<bool> > _pressEvents =
			new Dictionary< Event, ReactiveProperty<bool> >()
		{
			{ Event.Decide,			new ReactiveProperty<bool>() },
			{ Event.Quit,			new ReactiveProperty<bool>() },
			{ Event.Reset,			new ReactiveProperty<bool>() },
			{ Event.DebugView,		new ReactiveProperty<bool>() },
			{ Event.Nothing,		new ReactiveProperty<bool>() },
			{ Event.LongNothing,	new ReactiveProperty<bool>() },
			{ Event.AnyOperation,	new ReactiveProperty<bool>() },
		};
		/// <summary>スワイプ状態</summary>
		readonly ReactiveProperty<Swipe> _swipe = new ReactiveProperty<Swipe>( Swipe.None );
		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（軸）
		/// </summary>
		public Vector2 GetAxis( Axis axis ) {
			return _axes[axis];
		}
		/// <summary>
		/// ● 取得（押状態変更イベント関数）
		///		UniRxのReactivePropertyの仕様により、同値では通知されず、別値じゃないと通知されない。、
		/// </summary>
		public ReactiveProperty<bool> GetPressEvent( Event event_ ) {
			return _pressEvents[event_];
		}
		/// <summary>
		/// ● 取得（押下時イベント関数）
		/// </summary>
		public IObservable<Unit> GetPressedEvent( Event event_ ) {
			return GetPressEvent( event_ )
				.Where( is_ => is_ )
				.Select( _ => Unit.Default );
		}
		/// <summary>
		/// ● 取得（押上時イベント関数）
		/// </summary>
		public IObservable<Unit> GetPressUpEvent( Event event_ ) {
			return GetPressEvent( event_ )
				.Where( is_ => !is_ )
				.Select( _ => Unit.Default );
		}
		/// <summary>
		/// ● 取得（スワイプ）
		/// </summary>
		public ReactiveProperty<Swipe> GetSwipe() {
			return _swipe;
		}
		/// <summary>
		/// ● 取得（押下時スワイプ）
		/// </summary>
		public IObservable<Unit> GetPressedSwipe( Swipe target ) {
			return GetSwipe()
				.Where( state => state == target )
				.Select( _ => Unit.Default );
		}
		/// <summary>
		/// ● 取得（押下中のキー一覧）
		/// </summary>
		List<KeyCode> GetDownKeys() {
			if ( !Input.anyKeyDown )	{ return new List<KeyCode>(); }
			return EnumUtils.GetValues<KeyCode>()
				.Where( key => Input.GetKeyDown( key ) )
				.ToList();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public InputManager() {
			// ● 初期化
			_initializeEvent += async () => {
				await UniTask.Delay( 0 );
			};

			RegisterEventForUnityInput();		// Unity入力のイベント関数登録
			RegisterEventForMoveRange();		// 移動範囲のイベント関数登録
			RegisterEventForPressNothing();		// 無押下継続のイベント関数登録
			RegisterEventForLongPressNothing();	// 無押下長継続のイベント関数登録
			RegisterEventForSwipe();			// スワイプ入力のイベント関数登録
			RegisterEventForAnyOperation();		// 何か操作のイベント関数登録
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（Unity入力）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForUnityInput() {
			// 軸名一覧を作成
			var axisNames = new Dictionary<Axis, string[]> {
				{ Axis.Move,	new string[] { Get( Name.MoveAxisX ),	Get( Name.MoveAxisY )	} },
				{ Axis.Rotate,	new string[] { Get( Name.RotateAxisX ),	Get( Name.RotateAxisY )	} },
#if DEVELOP
				{ Axis.Debug,	new string[] { Get( Name.DebugAxisX ),	Get( Name.DebugAxisY )	} },
#endif
			};
			// イベント名一覧を作成
			var keyNames = new Dictionary<Event, string> {
				{ Event.Decide,		Get( Name.Decide )		},
				{ Event.Quit,		Get( Name.Quit )		},
				{ Event.Reset,		Get( Name.Reset )		},
#if DEVELOP
				{ Event.DebugView,	Get( Name.DebugView )	},
#endif
			};


			// ● 更新
			_updateEvent.Subscribe( _ => {
				// マウス位置を設定
				_mousePosition = Input.mousePosition;
				// 各種軸を設定
				foreach ( var pair in axisNames ) {
					var axis = new Vector2(
						Input.GetAxis( pair.Value[0] ),
						Input.GetAxis( pair.Value[1] )
					);
					if ( axis.magnitude > 1 )	{ axis.Normalize(); }
					_axes[pair.Key] = axis;
				}
				// 各種キーを設定
				foreach ( var pair in keyNames ) {
					_pressEvents[pair.Key].Value = Input.GetButton( pair.Value );
				}
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（移動範囲）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForMoveRange() {
			// 画面大きさのキャッシュ
			var screenSize = Vector2.zero;

			// ● 更新
			_updateEvent
				// 画面大きさが異なる場合
				.Select( _ => new Vector2( Screen.width, Screen.height ) )
				.Where( size => size != screenSize )
				// 画面の大きさから、各種範囲を設定
				.Subscribe( size => {
					screenSize = size;
					_noMoveSqrRange = ( screenSize / 100 ).sqrMagnitude;
					_swipeSqrRange = ( screenSize / 10 ).sqrMagnitude;
				} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（無押下継続）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForPressNothing() {
			// ● 更新
			_updateEvent.Subscribe( _ => {
				// 起動画面中か、決定キーが押されてない場合
				if (	!SplashScreenExtension.s_instance._isFinish ||
						!GetPressEvent( Event.Decide ).Value
				) {
					_pressEvents[Event.Nothing].Value = false;
					return;
				}

				// イベントシステムから光線を飛ばす
				var pointer = new PointerEventData(EventSystem.current) {
					position = _mousePosition
				};
				var result = new List<RaycastResult>();
				EventSystem.current.RaycastAll( pointer, result );
				// UIに触れていないかで判定
				_pressEvents[Event.Nothing].Value = result.Count == 0;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（無押下長継続）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForLongPressNothing() {
			// 押下位置を設定
			var clickPosition = Vector2.zero;
			GetPressedEvent( Event.Nothing )
				// 押下時にマウス位置を設定
				.Subscribe( _ => clickPosition = _mousePosition );


			// 無を長押ししてるか判定
			const float LONG_NOTHING_SECOND = 1;
			// ● 更新
			_updateEvent
				// 無を押下中で、押下位置が設定中の場合
				.Select( _ => GetPressEvent( Event.Nothing ).Value && clickPosition != Vector2.zero )
				// 無押下で指不動かに変換
				.Select( is_ => {
					// 無に触れている場合、指不動を判定
					if ( is_ ) {
						var delta = _mousePosition - clickPosition;
						if ( delta.sqrMagnitude < _noMoveSqrRange ) {
							return true;
						}
					}
					clickPosition = Vector2.zero;
					return false;
				} )
				// 現在秒に変換
				.Select( is_ => is_ ? TimeManager.s_instance._unscaledDeltaTime : -1 )
				// 現在時間と合計時間から、蓄積秒を設定
				.Scan( ( total, current ) =>
					current != -1	? Mathf.Min( total + current, LONG_NOTHING_SECOND )
									: 0
				)
				// 蓄積時間から、長押し判定
				.Subscribe( time => _pressEvents[Event.LongNothing].Value = time >= LONG_NOTHING_SECOND );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（スワイプ）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForSwipe() {
			// スワイプ開始時の位置を設定
			var clickPosition = Vector2.zero;
			const float CHECK_CLICK_SECOND = 1;
			// ● 更新
			_updateEvent
				// 押下中かに変換
				.Select( _ => Input.GetMouseButton( 0 ) )
				// 押下時に、押下位置を設定
				.Scan( ( last, current ) => {
					if ( !last && current )	{ clickPosition = _mousePosition; }
					return current;
				} )
				// 現在秒に変換
				.Select( is_ => is_ ? TimeManager.s_instance._unscaledDeltaTime : -1 )
				// 現在秒と合計秒から、蓄積秒を設定
				.Scan( ( total, current ) =>
					current != -1	? Mathf.Min( total + current, CHECK_CLICK_SECOND )
									: 0
				)
				// 1秒以上経過した場合
				.Where( time => time >= CHECK_CLICK_SECOND )
				// 押下位置をリセット
				.Subscribe( _ => clickPosition = Vector2.zero );


			// スワイプ状態を設定
			// ● 更新
			_updateEvent
				// マウス押下中で、押下位置が設定中の場合
				.Select( _ => Input.GetMouseButton( 0 ) && clickPosition != Vector2.zero )
				// 押下状態を指移動位置に変換
				.Select( is_ => {
					// マウス押下中で一定以上移動した場合、移動差を返す
					if ( is_ ) {
						var delta = _mousePosition - clickPosition;
						if ( delta.sqrMagnitude > _swipeSqrRange ) {
							clickPosition = Vector2.zero;
							return delta;
						}
					}
					return Vector2.zero;
				} )
				// 移動差から、角度に変換
				.Select( delta => {
					// 無移動の場合、有り得ない角度を返す
					if ( delta == Vector2.zero )	{ return -1; }
					// 移動差から角度を求め、0～360度に補正する
					var angle = Mathf.Atan2( delta.x, delta.y ) * Mathf.Rad2Deg;
					if ( angle < 0 )	{ angle += 360; }
					return angle;
				} )
				// 角度から、スワイプ状態に変換
				.Select( angle => {
					// NONEの率が高いので、先に判定して処理負荷軽減
					if			( angle == -1 )							{ return Swipe.None; }
					else if (	( 337.5	< angle && angle <= 360 ) ||
								( 0		<= angle && angle < 22.5 ) )	{ return Swipe.Up; }
					else if		( 22.5	< angle && angle < 67.5 )		{ return Swipe.UpperRight; }
					else if		( 67.5	< angle && angle < 112.5 )		{ return Swipe.Right; }
					else if		( 112.5	< angle && angle < 157.5 )		{ return Swipe.DownerRight; }
					else if		( 157.5	< angle && angle < 202.5 )		{ return Swipe.Down; }
					else if		( 202.5	< angle && angle < 247.5 )		{ return Swipe.DownerLeft; }
					else if		( 247.5	< angle && angle < 292.5 )		{ return Swipe.Left; }
					else if		( 292.5	< angle && angle < 337.5 )		{ return Swipe.UpperLeft; }
					// 何か返さないとエラーになるので、とりあえず適当に返す
					return Swipe.None;
				} )
				// スワイプ状態を適用
				.Subscribe( state => GetSwipe().Value = state );


			// デバッグ表示キーを設定
			// 上スワイプした場合
			GetPressedSwipe( Swipe.Up )
				// デバッグ表示キーが未押下の場合のみ、判定
				.Where( _ => !GetPressEvent( Event.DebugView ).Value )
				// 2本指タッチ中かに変換
				.Select( _ =>
					// マウスの場合、左右キー押下中の場合
					( Input.GetMouseButton( 0 ) && Input.GetMouseButton( 1 ) ) ||
					Input.touchCount >= 2
				)
				// デバッグ表示キーに適用
				.Subscribe( is_ => _pressEvents[Event.DebugView].Value = is_ );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（何か操作）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForAnyOperation() {
			// 軸一覧を作成
			var axes = EnumUtils.GetValues<Axis>();
			// イベント一覧を作成
			var events = EnumUtils.GetValues<Event>()
				.Where( event_ => event_ != Event.AnyOperation )
				.ToArray();

			// ● 更新
			_updateEvent.Subscribe( _ => {
				// 何か操作したか判定
				_pressEvents[Event.AnyOperation].Value =
					// 軸入力があるか
					axes.Any( axis => GetAxis( axis ) != Vector2.zero ) ||
					// キー入力があるか
					events.Any( event_ => GetPressEvent( event_ ).Value ) ||
					// スワイプしたか？
					GetSwipe().Value != Swipe.None;
			} );
		}
	}
}