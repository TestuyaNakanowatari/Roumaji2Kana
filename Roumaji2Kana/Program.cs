using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace Roumaji2Kana {

	class Program {

		static int Main( string[] args ) {

			/*
			 * chaeck args
			 */
			if ( args.Length == 0 ) {
				Console.WriteLine( "Useage : Roumaji2Kana.exe FilePath [Skip] [Take]" );
				Console.ReadKey();
				return 255;
			}

			/*
			 * check file path
			 */
			if ( !File.Exists( args[ 0 ] ) ) {
				Console.WriteLine( "File Not Found!! [" + args[ 0 ]  + "] ");
				Console.ReadKey();
				return 255;
			}

			/*
			 * input file
			 */
			string text = "";
			using ( StreamReader sr = new StreamReader( args[ 0 ], Encoding.GetEncoding( "utf-8" ) ) ) {
				text = sr.ReadToEnd();
			}

			/*
			 * new RoumajiCollection class
			 */
			RoumajiCollection rc = new RoumajiCollection( text );

			/*
			 * get kana
			 */
			StringBuilder sb = new StringBuilder();
			foreach ( string s in rc ) sb.Append( s );

			/*
			 * output file
			 */
			using ( StreamWriter writer = new StreamWriter( args[ 0 ] + "_out.txt", false, Encoding.GetEncoding( "utf-8" ) ) ) {
				writer.Write( sb );
			}

			// ----------------- ここからお遊び -----------------

			Console.WriteLine( "ひらがな Count = {0}", rc.Count() );
			Console.WriteLine( "「っ」含まれている? =  {0}", rc.Contains( "っ" ) );
			Console.WriteLine( "Min = {0}", rc.Min() );	// 無意味
			Console.WriteLine( "Max = {0}", rc.Max() );	// 無意味

			/*
			 * ひらがなの使用回数
			 */
			sb.Length = 0;
			foreach ( var m in rc.GroupBy( s => s ).OrderByDescending( g => g.Count() ) ) {
				string s = string.Format( "[{0}] Count = {1}", m.Key.Replace( "\r", @"\r" ).Replace( "\n", @"\n" ), m.Count() );
				sb.AppendLine( s );
				Console.WriteLine( s );
			}
			using ( StreamWriter writer = new StreamWriter( args[ 0 ] + "_aggregate.txt", false, Encoding.GetEncoding( "utf-8" ) ) ) {
				writer.Write( sb );
			}

			int skip = -1, take = -1;

			/*
			 * skip take function
			 * ひらがなの切り出し substring みたいな感じ
			 */
			// skip param
			if ( args.Length >= 2 ) {
				try {
					skip = Int32.Parse( args[ 1 ] );
				} catch { /* ignore */ }

			}
			// take param
			if ( args.Length >= 3 ) {
				try {
					take = Int32.Parse( args[ 2 ] );
				} catch { /* ignore */ }

			}
			if ( take == -1 ) take = rc.Count();
			if ( take > rc.Count() ) take = rc.Count();
			Console.WriteLine( "Skip = {0}, Take = {1}", skip, take );
			foreach ( string s in rc.Skip( skip ).Take( take ) ) Console.Write( s );
			Console.WriteLine( "" );

			// finish
			Console.ReadKey();
			return 0;

		}

	}

	class RoumajiCollection : IEnumerable<string> {

		// ローマ字 -> かな Dictionary
		private Dictionary<string, Dictionary<string, string>> r2k = null;

		// property
		public string Roumaji { get; set; }

		// constructor
		public RoumajiCollection() : this( "" ) {
		}

		// constructor
		public RoumajiCollection( string str ) {
			this.Roumaji = str;

			// new r2k
			r2k = new Dictionary<string, Dictionary<string, string>>( StringComparer.CurrentCultureIgnoreCase );

#region Dictionary

			// a
			r2k.Add(
				"a",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "a", "あ" },
				}
			);

			// b
			r2k.Add(
				"b",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ba", "ば" },
					{ "be", "べ" },
					{ "bi", "び" },
					{ "bo", "ぼ" },
					{ "bu", "ぶ" },
					{ "bya", "びゃ" },
					{ "bye", "びぇ" },
					{ "byi", "びぃ" },
					{ "byo", "びょ" },
					{ "byu", "びゅ" },
				}
			);

			// c
			r2k.Add(
				"c",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "cha", "ちゃ" },
					{ "che", "ちぇ" },
					{ "chi", "ち" },
					{ "cho", "ちょ" },
					{ "chu", "ちゅ" },
					{ "cya", "ちゃ" },
					{ "cye", "ちぇ" },
					{ "cyi", "ちぃ" },
					{ "cyo", "ちょ" },
					{ "cyu", "ちゅ" },
				}
			);

			// d
			r2k.Add(
				"d",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "da", "だ" },
					{ "de", "で" },
					{ "di", "ぢ" },
					{ "do", "ど" },
					{ "du", "づ" },
					{ "dwa", "どぁ" },
					{ "dwe", "どぇ" },
					{ "dwi", "どぃ" },
					{ "dwo", "どぉ" },
					{ "dwu", "どぅ" },
					{ "dya", "ぢゃ" },
					{ "dye", "ぢぇ" },
					{ "dyi", "ぢぃ" },
					{ "dyo", "ぢょ" },
					{ "dyu", "ぢゅ" },
					{ "dha", "でゃ" },
					{ "dhi", "でぃ" },
					{ "dhu", "でゅ" },
					{ "dhe", "でぇ" },
					{ "dho", "でょ" },
			}
			);

			// e
			r2k.Add(
				"e",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "e", "え" },
				}
			);

			// f
			r2k.Add(
				"f",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "fa", "ふぁ" },
					{ "fe", "ふぇ" },
					{ "fi", "ふぃ" },
					{ "fo", "ふぉ" },
					{ "fu", "ふ" },
					{ "fwa", "ふぁ" },
					{ "fwe", "ふぇ" },
					{ "fwi", "ふぃ" },
					{ "fwo", "ふぉ" },
					{ "fwu", "ふぅ" },
					{ "fya", "ふゃ" },
					{ "fye", "ふぇ" },
					{ "fyi", "ふぃ" },
					{ "fyo", "ふょ" },
					{ "fyu", "ふゅ" },
				}
			);

			// g
			r2k.Add(
				"g",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ga", "が" },
					{ "ge", "げ" },
					{ "gi", "ぎ" },
					{ "go", "ご" },
					{ "gu", "ぐ" },
					{ "gwa", "ぐぁ" },
					{ "gwe", "ぐぇ" },
					{ "gwi", "ぐぃ" },
					{ "gwo", "ぐぉ" },
					{ "gwu", "ぐぅ" },
					{ "gya", "ぎゃ" },
					{ "gye", "ぎぇ" },
					{ "gyi", "ぎぃ" },
					{ "gyo", "ぎょ" },
					{ "gyu", "ぎゅ" },
				}
			);

			// h
			r2k.Add(
				"h",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ha", "は" },
					{ "he", "へ" },
					{ "hi", "ひ" },
					{ "ho", "ほ" },
					{ "hu", "ふ" },
					{ "hya", "ひゃ" },
					{ "hye", "ひぇ" },
					{ "hyi", "ひぃ" },
					{ "hyo", "ひょ" },
					{ "hyu", "ひゅ" },

				}
			);

			// i
			r2k.Add(
				"i",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "i", "い" },
				}
			);

			// j
			r2k.Add(
				"j",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ja", "じゃ" },
					{ "je", "じぇ" },
					{ "ji", "じ" },
					{ "jo", "じょ" },
					{ "ju", "じゅ" },
					{ "jya", "じゃ" },
					{ "jye", "じぇ" },
					{ "jyi", "じぃ" },
					{ "jyo", "じょ" },
					{ "jyu", "じゅ" },
				}
			);

			// k
			r2k.Add(
				"k",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ka", "か" },
					{ "ke", "け" },
					{ "ki", "き" },
					{ "ko", "こ" },
					{ "ku", "く" },
					{ "kwa", "くぁ" },
					{ "kya", "きゃ" },
					{ "kye", "きぇ" },
					{ "kyi", "きぃ" },
					{ "kyo", "きょ" },
					{ "kyu", "きゅ" },
				}
			);

			// l
			r2k.Add(
				"l",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "la", "ぁ" },
					{ "le", "ぇ" },
					{ "li", "ぃ" },
					{ "lka", "ヵ" },
					{ "lke", "ヶ" },
					{ "lo", "ぉ" },
					{ "ltu", "っ" },
					{ "lu", "ぅ" },
					{ "lya", "ゃ" },
					{ "lyo", "ょ" },
					{ "lyu", "ゅ" },
				}
			);

			// m
			r2k.Add(
				"m",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ma", "ま" },
					{ "me", "め" },
					{ "mi", "み" },
					{ "mo", "も" },
					{ "mu", "む" },
					{ "mya", "みゃ" },
					{ "mye", "みぇ" },
					{ "myi", "みぃ" },
					{ "myo", "みょ" },
					{ "myu", "みゅ" },
				}
			);

			// n
			r2k.Add(
				"n",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "n", "ん" },
					{ "na", "な" },
					{ "ne", "ね" },
					{ "ni", "に" },
					{ "nn", "ん" },
					{ "no", "の" },
					{ "nu", "ぬ" },
					{ "nya", "にゃ" },
					{ "nye", "にぇ" },
					{ "nyi", "にぃ" },
					{ "nyo", "にょ" },
					{ "nyu", "にゅ" },
				}
			);

			// o
			r2k.Add(
				"o",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "o", "お" },
				}
			);

			// p
			r2k.Add(
				"p",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "pa", "ぱ" },
					{ "pe", "ぺ" },
					{ "pi", "ぴ" },
					{ "po", "ぽ" },
					{ "pu", "ぷ" },
					{ "pya", "ぴゃ" },
					{ "pyu", "ぴゅ" },
					{ "pyo", "ぴょ" },
				}
			);

			// q
			r2k.Add(
				"q",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "qa", "くぁ" },
					{ "qe", "くぇ" },
					{ "qi", "くぃ" },
					{ "qo", "くぉ" },
					{ "qwa", "くぁ" },
					{ "qwe", "くぇ" },
					{ "qwi", "くぃ" },
					{ "qwo", "くぉ" },
					{ "qwu", "くぅ" },
					{ "qya", "くゃ" },
					{ "qye", "くぇ" },
					{ "qyo", "くょ" },
					{ "qyu", "くゅ" },
				}
			);

			// r
			r2k.Add(
				"r",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ra", "ら" },
					{ "re", "れ" },
					{ "ri", "り" },
					{ "ro", "ろ" },
					{ "ru", "る" },
					{ "rya", "りゃ" },
					{ "rye", "りぇ" },
					{ "ryi", "りぃ" },
					{ "ryo", "りょ" },
					{ "ryu", "りゅ" },

				}
			);

			// s
			r2k.Add(
				"s",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "sa", "さ" },
					{ "se", "せ" },
					{ "sha", "しゃ" },
					{ "she", "しぇ" },
					{ "shi", "し" },
					{ "sho", "しょ" },
					{ "shu", "しゅ" },
					{ "si", "し" },
					{ "so", "そ" },
					{ "su", "す" },
					{ "swa", "すぁ" },
					{ "swe", "すぇ" },
					{ "swi", "すぃ" },
					{ "swo", "すぉ" },
					{ "swu", "すぅ" },
					{ "sya", "しゃ" },
					{ "sye", "しぇ" },
					{ "syi", "しぃ" },
					{ "syo", "しょ" },
					{ "syu", "しゅ" },
				}
			);

			// t
			r2k.Add(
				"t",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ta", "た" },
					{ "te", "て" },
					{ "tha", "てゃ" },
					{ "the", "てぇ" },
					{ "thi", "てぃ" },
					{ "tho", "てょ" },
					{ "thu", "てゅ" },
					{ "ti", "ち" },
					{ "to", "と" },
					{ "tsa", "つぁ" },
					{ "tse", "つぇ" },
					{ "tsi", "つぃ" },
					{ "tso", "つぉ" },
					{ "tsu", "つ" },
					{ "tu", "つ" },
					{ "twa", "とぁ" },
					{ "twe", "とぇ" },
					{ "twi", "とぃ" },
					{ "two", "とぉ" },
					{ "twu", "とぅ" },
					{ "tya", "ちゃ" },
					{ "tye", "ちぇ" },
					{ "tyi", "ちぃ" },
					{ "tyo", "ちょ" },
					{ "tyu", "ちゅ" },
				}
			);

			// u
			r2k.Add(
				"u",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "u", "う" },
				}
			);

			// v
			r2k.Add(
				"v",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "vu", "ヴ" },
				}
			);

			// w
			r2k.Add(
				"w",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "wa", "わ" },
					{ "wi", "うぃ" },
					{ "wu", "ぅ" },
					{ "we", "うぇ" },
					{ "wo", "を" },
					{ "wya", "わ" },
					{ "wyi", "ゐ" },
					{ "wyu", "ぅ" },
					{ "wye", "ゑ" },
					{ "wyo", "ゑ" },
				}
			);

			// x
			r2k.Add(
				"x",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "xa", "ぁ" },
					{ "xe", "ぇ" },
					{ "xi", "ぃ" },
					{ "xka", "ヵ" },
					{ "xke", "ヶ" },
					{ "xo", "ぉ" },
					{ "xtu", "っ" },
					{ "xu", "ぅ" },
					{ "xya", "ゃ" },
					{ "xyo", "ょ" },
					{ "xyu", "ゅ" },
					{ "xwa", "ゎ" },
					{ "xwi", "ゐ" },
					{ "xwu", "ぅ" },
					{ "xwe", "ゑ" },
					{ "xwo", "を" },
				}
			);

			// y
			r2k.Add(
				"y",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "ya", "や" },
					{ "yo", "よ" },
					{ "yu", "ゆ" },
				}
			);

			// z
			r2k.Add(
				"z",
				new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
					{ "za", "ざ" },
					{ "ze", "ぜ" },
					{ "zi", "じ" },
					{ "zo", "ぞ" },
					{ "zu", "ず" },
					{ "zya", "じゃ" },
					{ "zye", "じぇ" },
					{ "zyi", "じぃ" },
					{ "zyo", "じょ" },
					{ "zyu", "じゅ" },
				}
			);

			// -
			r2k.Add( "-", new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
				{ "-", "ー" },
			} );

			// ,
			r2k.Add( ",", new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
				{ ",", "、" },
			} );

			// .
			r2k.Add( ".", new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ) {
				{ ".", "。" },
			} );

#endregion

		}

		public IEnumerator<string> GetEnumerator() {

			// 子音 ※まだ多いかもしれない...
			const string SHIIN = @"[bcdfghjklmpqrstvwxyz]";	// removed aiueo n

			// っ
			const string TSU = "っ";

			char[] chars = this.Roumaji.ToCharArray();

			for ( int i = 0 ; i < chars.Length ; i++ ) {

				string chr = chars[ i ].ToString().toNarrow();	// narrow char

				// 長い順にリスト追加要
				List<string> list = new List<string>( 3 );
				if ( ( chars.Length - i ) >= 3 ) list.Add( ( chars[ i ].ToString() + chars[ i + 1 ].ToString() + chars[ i + 2 ].ToString() ).toNarrow() );
				if ( ( chars.Length - i ) >= 2 ) list.Add( ( chars[ i ].ToString() + chars[ i + 1 ].ToString() ).toNarrow() );
				list.Add( chars[ i ].ToString().toNarrow() );

				// 子音?
				if ( list.Count > 1 && Regex.IsMatch( chr, SHIIN, RegexOptions.IgnoreCase ) ) {
					// same next char ?
					if ( string.Compare( chr, chars[ i + 1 ].ToString().toNarrow(), true /* true=ignoreCase */ ) == 0 ) {
						// find 「っ」
						yield return TSU;
						continue;
					}
				}

				// find function
				if ( r2k.ContainsKey( chr ) ) {
					Dictionary<string, string> dic = r2k[ chr ];

					var result = list
						.Where( s => dic.ContainsKey( s ) )
						.Select( s => new { key = s, value = dic[ s ] } )
						.FirstOrDefault();

					if ( result != null ) {
						yield return result.value;
						i += result.key.Length - 1;
						continue;
					}

				}

				// not define
				yield return chars[ i ].ToString();
			}

		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

	}

	// Extensions method
	static class Extensions {
		
		// win32 API
		[DllImport( "kernel32.dll" )]
		static extern private int LCMapStringW(
			int Locale,
			uint dwMapFlags,
			[MarshalAs( UnmanagedType.LPWStr )]string lpSrcStr,
			int cchSrc,
			[MarshalAs( UnmanagedType.LPWStr )]string lpDestStr,
			int cchDest
		);

		public enum dwMapFlags : uint {
			NORM_IGNORECASE = 0x00000001,           //大文字と小文字を区別しません。
			NORM_IGNORENONSPACE = 0x00000002,       //送りなし文字を無視します。このフラグをセットすると、日本語アクセント文字も削除されます。
			NORM_IGNORESYMBOLS = 0x00000004,        //記号を無視します。
			LCMAP_LOWERCASE = 0x00000100,           //小文字を使います。
			LCMAP_UPPERCASE = 0x00000200,           //大文字を使います。
			LCMAP_SORTKEY = 0x00000400,             //正規化されたワイド文字並び替えキーを作成します。
			LCMAP_BYTEREV = 0x00000800,             //Windows NT のみ : バイト順序を反転します。たとえば 0x3450 と 0x4822 を渡すと、結果は 0x5034 と 0x2248 になります。
			SORT_STRINGSORT = 0x00001000,           //区切り記号を記号と同じものとして扱います。
			NORM_IGNOREKANATYPE = 0x00010000,       //ひらがなとカタカナを区別しません。ひらがなとカタカナを同じと見なします。
			NORM_IGNOREWIDTH = 0x00020000,          //シングルバイト文字と、ダブルバイトの同じ文字とを区別しません。
			LCMAP_HIRAGANA = 0x00100000,            //ひらがなにします。
			LCMAP_KATAKANA = 0x00200000,            //カタカナにします。
			LCMAP_HALFWIDTH = 0x00400000,           //半角文字にします（適用される場合）。
			LCMAP_FULLWIDTH = 0x00800000,           //全角文字にします（適用される場合）。
			LCMAP_LINGUISTIC_CASING = 0x01000000,   //大文字と小文字の区別に、ファイルシステムの規則（既定値）ではなく、言語上の規則を使います。LCMAP_LOWERCASE、または LCMAP_UPPERCASE とのみ組み合わせて使えます。
			LCMAP_SIMPLIFIED_CHINESE = 0x02000000,  //中国語の簡体字を繁体字にマップします。
			LCMAP_TRADITIONAL_CHINESE = 0x04000000, //中国語の繁体字を簡体字にマップします。
		}

		/// <summary>
		/// アルファベットの半角変換
		/// </summary>
		/// <param name="str">変換文字列</param>
		/// <returns>変換後文字列</returns>
		public static string toNarrow( this string str ) {

			// 自力は面倒なので、win32 api でw
			var ci = System.Globalization.CultureInfo.CurrentCulture;
			string result = new string( ' ', str.Length );
			LCMapStringW( ci.LCID, (uint)dwMapFlags.LCMAP_HALFWIDTH, str, str.Length, result, result.Length );
			return result; 

		}
	}

}
