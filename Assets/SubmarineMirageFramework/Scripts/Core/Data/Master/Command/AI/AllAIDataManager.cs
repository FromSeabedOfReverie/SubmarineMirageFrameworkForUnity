//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 全AI情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AllAIDataManager : BaseDataManager<string, AIDataManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>読込書類名の一覧</summary>
		readonly string[] _fileNames = new string[] {
			"TestAI"
		};
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public List<AIData> Get( string fileName, string key ) {
			var data = Get( fileName );
			return data?.Get( key ) ??  new List<AIData>();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			_fileNames.ForEach( name => {
				var data = new AIDataManager( name );
				Register( name, data );
			} );

			await base.Load();
		}
	}
}