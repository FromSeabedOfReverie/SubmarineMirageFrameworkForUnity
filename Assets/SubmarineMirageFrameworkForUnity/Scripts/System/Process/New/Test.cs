//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using UniRx;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestProcess : BaseProcess {
		public override void Create() {
			_loadEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "1" ).Subscribe( _ => {
				Log.Debug( "update 1" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
	}

	public class TestProcess2 : TestProcess {
		public override void Create() {
			base.Create();
			_loadEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "2" ).Subscribe( _ => {
				Log.Debug( "update 2" );
			} );
			_updateEvent.AddFirst( "0" ).Subscribe( _ => {
				Log.Debug( "update 0" );
			} );
			_updateEvent.InsertFirst( "1", "0.5" ).Subscribe( _ => {
				Log.Debug( "update 0.5" );
			} );
			_updateEvent.InsertLast( "1", "1.5" ).Subscribe( _ => {
				Log.Debug( "update 1.5" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
		~TestProcess2() {
			Log.Debug( $"Delete {this.GetAboutName()}" );
		}
	}

	public class TestProcess3 : BaseProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
		public override void Create() {}
	}
}