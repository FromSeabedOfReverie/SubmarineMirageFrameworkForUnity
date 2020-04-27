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
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 型の拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class TypeExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 継承しているか？
		///		※等しい場合も含む。
		///		参考URL : https://stackoverflow.com/questions/10718364/check-if-t-inherits-or-implements-a-class-interface
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsInheritance<T>( this Type self ) {
			return typeof( T ).IsAssignableFrom( self );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ジェネリック関数を実行
		///		※ジェネリック型が不明で、objectインスタンスのみ所持している場合、
		///			object.GetType()から、ジェネリック型を推測して、実行する。
		///		※IL2CPPビルド後、動的コード生成が認められず、エラーになるかも？
		///		参考URL : https://devlights.hatenablog.com/entry/20081107/p2
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static object ExecuteGenericMethod( this Type self, object classInstance, string methodName,
													object methodGenericObject, params object[] methodArguments )
		{
			// ジェネリック型を設定
			var genericType = methodGenericObject.GetType();
			// デフォルト引数追加を考慮し、引数配列をリスト化
			var arguments = methodArguments.ToList();


			// ジェネリック関数を検索
			var methodInfo = self.GetMethods()
				// 条件に一致する関数を走査
				.Where( mi =>
					mi.IsGenericMethod &&			// ジェネリック関数か？
					mi.IsGenericMethodDefinition &&	// ジェネリック関数定義か？
					mi.ContainsGenericParameters &&	// ジェネリック変数を所持か？
					mi.Name == methodName			// 指定関数名か？
				)

				// 関数の引数が、それぞれ一致するか判定
				.Where( mi => mi.GetParameters()
					.Select( ( info, i ) => {
						// 引数型が、ジェネリックか、等しい場合、一致
						if ( i < arguments.Count &&
							(
								info.ParameterType.IsGenericParameter ||
								info.ParameterType == arguments[i].GetType()
							)
						) {
							return true;

						// 引数は無いが、デフォルト引数が設定してある場合、一致
						} else if ( info.HasDefaultValue ) {
							arguments.Add( info.DefaultValue );	// デフォルト引数が未登録だと、エラーになる
							return true;
						}

						// それ以外は、不一致
						return false;
					} )
					// 引数が、全て一致するか
					.All( isEqual => isEqual )
				)

				// 最初に一致した関数を取得
				.First();


			// ジェネリック関数を生成し、実行
			return methodInfo
				.MakeGenericMethod( genericType )
				.Invoke( classInstance, arguments.ToArray() );
		}
	}
}