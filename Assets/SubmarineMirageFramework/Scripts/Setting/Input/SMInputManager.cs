//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Service;
	using Event;
	using Task;
	using Extension;
	using Utility;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 入力の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMInputManager : SMUnityName<SMInputName>, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		SMTimeManager _timeManager	{ get; set; }

		/// <summary>不動範囲の2乗</summary>
		float _noMoveSqrRange { get; set; }
		/// <summary>スワイプ範囲の2乗</summary>
		float _swipeSqrRange { get; set; }

		/// <summary>マウス位置</summary>
		public Vector2 _mousePosition	{ get; private set; }
		/// <summary>軸の一覧</summary>
		readonly Dictionary<SMInputAxis, Vector2> _axes = EnumUtils.GetValues<SMInputAxis>()
			.ToDictionary( e => e, e => Vector2.zero );

		/// <summary>入力中か？の一覧</summary>
		readonly Dictionary<SMInputEvent, bool> _isPressKeys = EnumUtils.GetValues<SMInputEvent>()
			.ToDictionary( e => e, e => false );
		/// <summary>押下中イベントの一覧</summary>
		readonly Dictionary<SMInputEvent, SMSubject> _pressEvents = EnumUtils.GetValues<SMInputEvent>()
			.ToDictionary( e => e, e => new SMSubject() );

		/// <summary>スワイプ状態</summary>
		SMInputSwipe _swipe { get; set; } = SMInputSwipe.None;
		/// <summary>スワイプイベント</summary>
		public readonly ReactiveProperty<SMInputSwipe> _swipeEvent = new ReactiveProperty<SMInputSwipe>();
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMInputManager() {
			_disposables.AddLast( () => {
				_mousePosition = Vector2.zero;
				_axes.Clear();

				_isPressKeys.Clear();
				_pressEvents.ForEach( pair => pair.Value.Dispose() );
				_pressEvents.Clear();

				_swipe = SMInputSwipe.None;
				_swipeEvent.Dispose();

				_timeManager = null;
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			_timeManager = SMServiceLocator.Resolve<SMTimeManager>();

			SetUnityInput();
			SetMoveRange();
			SetPressNothing();
			SetLongPressNothing();
			SetSwipeInput();
			RegisterEventForAnyOperation();
			RegisterEventForDebug();

			_isPressKeys
				.Where( pair => pair.Value )
				.ForEach
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（Unity入力）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetUnityInput() {
			// 軸名一覧を作成
			var axisNames = new Dictionary<SMInputAxis, string[]> {
				{
					SMInputAxis.Move,
					new string[] { Get( SMInputName.MoveAxisX ), Get( SMInputName.MoveAxisY ) }
				}, {
					SMInputAxis.Rotate,
					new string[] { Get( SMInputName.RotateAxisX ), Get( SMInputName.RotateAxisY ) }
#if DEVELOP
				}, {
					SMInputAxis.Debug,
					new string[] { Get( SMInputName.DebugAxisX ), Get( SMInputName.DebugAxisY ) }
#endif
				},
			};
			// イベント名一覧を作成
			var keyNames = new Dictionary<SMInputEvent, string> {
				{ SMInputEvent.Decide,		Get( SMInputName.Decide ) },
				{ SMInputEvent.Quit,		Get( SMInputName.Quit ) },
				{ SMInputEvent.Reset,		Get( SMInputName.Reset ) },
#if DEVELOP
				{ SMInputEvent.DebugView,	Get( SMInputName.DebugView ) },
#endif
			};

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				// マウス位置を設定
				_mousePosition = Input.mousePosition;

				// 各種軸を設定
				axisNames.ForEach( pair => {
					var axis = new Vector2(
						Input.GetAxis( pair.Value[0] ),
						Input.GetAxis( pair.Value[1] )
					);
					if ( axis.magnitude > 1 )	{ axis.Normalize(); }
					_axes[pair.Key] = axis;
				} );

				// 各種キーを設定
				keyNames.ForEach( pair => {
					_isPressKeys[pair.Key] = Input.GetButton( pair.Value );
				} );
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（移動範囲）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetMoveRange() {
			// 画面大きさのキャッシュ
			var lastSize = Vector2.zero;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				var size = new Vector2( Screen.width, Screen.height );
				if ( size == lastSize )	{ return; }
				lastSize = size;

				// 画面の大きさから、各種範囲を設定
				_noMoveSqrRange = ( size / 100 ).sqrMagnitude;
				_swipeSqrRange = ( size / 10 ).sqrMagnitude;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（無押下継続）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetPressNothing() {
			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				// 決定キーが押されてない場合
				if ( !IsPressEvent( SMInputEvent.Decide ).Value ) {
					_pressEvents[SMInputEvent.Nothing].Value = false;
					return;
				}

				// イベントシステムから光線を飛ばす
				var pointer = new PointerEventData( EventSystem.current ) {
					position = _mousePosition
				};
				var result = new List<RaycastResult>();
				EventSystem.current.RaycastAll( pointer, result );
				// UIに触れていないかで判定
				_pressEvents[SMInputEvent.Nothing].Value = result.IsEmpty();
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（無押下長継続）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetLongPressNothing() {
			// 押下位置を設定
			var clickPosition = Vector2.zero;
			GetPressedEvent( SMInputEvent.Nothing )
				// 押下時にマウス位置を設定
				.Subscribe( _ => clickPosition = _mousePosition );


			// 無を長押ししてるか判定
			const float LONG_NOTHING_SECOND = 1;
			var seconds = 0f;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				// 無を押下中で、押下中で、指不動の場合
				if (	IsPressEvent( SMInputEvent.Nothing ).Value &&
						clickPosition != Vector2.zero &&
						( _mousePosition - clickPosition ).sqrMagnitude < _noMoveSqrRange
				) {
					seconds = Mathf.Min( seconds + _timeManager._unscaledDeltaTime, LONG_NOTHING_SECOND );
				} else {
					clickPosition = Vector2.zero;
					seconds = 0;
				}

				// 蓄積秒から、長押し判定
				_pressEvents[SMInputEvent.LongNothing].Value = seconds >= LONG_NOTHING_SECOND;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（スワイプ）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetSwipeInput() {
			// スワイプ開始時の位置を設定
			var clickPosition = Vector2.zero;
			const float CHECK_CLICK_SECOND = 1;

			// ● 更新
			_updateEvent.AddLast()
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
					if			( angle == -1 )							{ return SMInputSwipe.None; }
					else if (	( 337.5	< angle && angle <= 360 ) ||
								( 0		<= angle && angle < 22.5 ) )	{ return SMInputSwipe.Up; }
					else if		( 22.5	< angle && angle < 67.5 )		{ return SMInputSwipe.UpperRight; }
					else if		( 67.5	< angle && angle < 112.5 )		{ return SMInputSwipe.Right; }
					else if		( 112.5	< angle && angle < 157.5 )		{ return SMInputSwipe.DownerRight; }
					else if		( 157.5	< angle && angle < 202.5 )		{ return SMInputSwipe.Down; }
					else if		( 202.5	< angle && angle < 247.5 )		{ return SMInputSwipe.DownerLeft; }
					else if		( 247.5	< angle && angle < 292.5 )		{ return SMInputSwipe.Left; }
					else if		( 292.5	< angle && angle < 337.5 )		{ return SMInputSwipe.UpperLeft; }
					// 何か返さないとエラーになるので、とりあえず適当に返す
					return SMInputSwipe.None;
				} )
				// スワイプ状態を適用
				.Subscribe( state => _swipe.Value = state );


			// デバッグ表示キーを設定
			// 上スワイプした場合
			GetPressedSwipe( SMInputSwipe.Up )
				// デバッグ表示キーが未押下の場合のみ、判定
				.Where( _ => !IsPressEvent( SMInputEvent.DebugView ).Value )
				// 2本指タッチ中かに変換
				.Select( _ =>
					// マウスの場合、左右キー押下中の場合
					( Input.GetMouseButton( 0 ) && Input.GetMouseButton( 1 ) ) ||
					Input.touchCount >= 2
				)
				// デバッグ表示キーに適用
				.Subscribe( is_ => _pressEvents[SMInputEvent.DebugView].Value = is_ );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（何か操作）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForAnyOperation() {
			// 軸一覧を作成
			var axes = EnumUtils.GetValues<SMInputAxis>();
			// イベント一覧を作成
			var events = EnumUtils.GetValues<SMInputEvent>()
				.Where( event_ => event_ != SMInputEvent.AnyOperation )
				.ToArray();

			// ● 更新
			_updateEvent.Subscribe( _ => {
				// 何か操作したか判定
				_pressEvents[SMInputEvent.AnyOperation].Value =
					// 軸入力があるか
					axes.Any( axis => GetAxis( axis ) != Vector2.zero ) ||
					// キー入力があるか
					events.Any( event_ => IsPressEvent( event_ ).Value ) ||
					// スワイプしたか？
					_swipe.Value != SMInputSwipe.None;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数登録（デバッグ）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEventForDebug() {
#if false
			if ( !SMDebugManager.IS_DEVELOP ) { return; }

			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			var dataManager = SMServiceLocator.Resolve<AllDataManager>();

			// デバッグ表示キー押下の場合
			GetPressedEvent( Event.DebugView ).Subscribe( _ => {
				// 設定を保存
				var setting = AllDataManager.s_instance._save._setting;
				setting._data._isViewDebug = !setting._data._isViewDebug;
				setting.Save().Forget();

				// 描画切り替え
				displayLog._isDraw = setting._data._isViewDebug;
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（軸）
		/// </summary>
		public Vector2 GetAxis( SMInputAxis axis )
			=> _axes[axis];

		/// <summary>
		/// ● 取得（押状態変更イベント関数）
		///		UniRxのReactivePropertyの仕様により、同値では通知されず、別値じゃないと通知されない。
		/// </summary>
		public ReactiveProperty<bool> IsPressEvent( SMInputEvent @event )
			=> _pressEvents[@event];

		/// <summary>
		/// ● 取得（押下中イベント関数）
		/// </summary>
		public IObservable<Unit> GetPressEvent( SMInputEvent @event )
			=> _updateEvent.AddLast()
				.Select( _ => _pressEvents[@event].Value )
				.Where( @is => @is )
				.Select( _ => Unit.Default );

		/// <summary>
		/// ● 取得（押下時イベント関数）
		/// </summary>
		public IObservable<Unit> GetPressedEvent( SMInputEvent @event )
			=> _pressEvents[@event]
				.Where( @is => @is )
				.Select( _ => Unit.Default );

		/// <summary>
		/// ● 取得（押上時イベント関数）
		/// </summary>
		public IObservable<Unit> GetPressUpEvent( SMInputEvent @event )
			=> _pressEvents[@event]
				.Where( @is => !@is )
				.Select( _ => Unit.Default );

		/// <summary>
		/// ● 取得（押下時スワイプ）
		/// </summary>
		public IObservable<Unit> GetPressedSwipe( SMInputSwipe target )
			=> _swipe
				.Where( state => state == target )
				.Select( _ => Unit.Default );

		/// <summary>
		/// ● 取得（押下中のキー一覧）
		/// </summary>
		IEnumerable<KeyCode> GetDownKeys() {
			if ( !Input.anyKeyDown ) { yield break; }

			foreach ( var key in EnumUtils.GetValues<KeyCode>() ) {
				if ( Input.GetKeyDown( key ) ) {
					yield return key;
				}
			}
		}
	}
}