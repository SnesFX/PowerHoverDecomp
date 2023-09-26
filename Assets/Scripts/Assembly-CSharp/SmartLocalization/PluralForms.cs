using System;
using System.Collections.Generic;

namespace SmartLocalization
{
	internal class PluralForms
	{
		public static Dictionary<string, Func<int, int>> Languages = new Dictionary<string, Func<int, int>>
		{
			{
				"ar",
				delegate(int n)
				{
					int result21;
					switch (n)
					{
					case 0:
						result21 = 0;
						break;
					case 1:
						result21 = 1;
						break;
					case 2:
						result21 = 2;
						break;
					default:
						result21 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result21;
				}
			},
			{
				"bg",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ca",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"zh-CHS",
				(int n) => 0
			},
			{
				"cs",
				delegate(int n)
				{
					int result20;
					switch (n)
					{
					case 1:
						result20 = 0;
						break;
					case 2:
					case 3:
					case 4:
						result20 = 1;
						break;
					default:
						result20 = 2;
						break;
					}
					return result20;
				}
			},
			{
				"da",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"de",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"el",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"en",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fi",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"he",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"hu",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"is",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? 1 : 0
			},
			{
				"it",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ja",
				(int n) => 0
			},
			{
				"ko",
				(int n) => 0
			},
			{
				"nl",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"no",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"pl",
				(int n) => (n != 1) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"pt",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ro",
				(int n) => (n != 1) ? ((n == 0 || (n % 100 > 0 && n % 100 < 20)) ? 1 : 2) : 0
			},
			{
				"ru",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"hr",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"sk",
				delegate(int n)
				{
					int result19;
					switch (n)
					{
					case 1:
						result19 = 0;
						break;
					case 2:
					case 3:
					case 4:
						result19 = 1;
						break;
					default:
						result19 = 2;
						break;
					}
					return result19;
				}
			},
			{
				"sq",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"sv",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"th",
				(int n) => 0
			},
			{
				"tr",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"id",
				(int n) => 0
			},
			{
				"uk",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"be",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"sl",
				(int n) => (n % 100 == 1) ? 1 : ((n % 100 == 2) ? 2 : ((n % 100 == 3 || n % 100 == 4) ? 3 : 0))
			},
			{
				"et",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"lv",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n != 0) ? 1 : 2) : 0
			},
			{
				"lt",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"fa",
				(int n) => 0
			},
			{
				"vi",
				(int n) => 0
			},
			{
				"hy",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"eu",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"mk",
				(int n) => (n != 1 && n % 10 != 1) ? 1 : 0
			},
			{
				"af",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ka",
				(int n) => 0
			},
			{
				"fo",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"hi",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"sw",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"gu",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ta",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"te",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"kn",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"mr",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"gl",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"kok",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-SA",
				delegate(int n)
				{
					int result18;
					switch (n)
					{
					case 0:
						result18 = 0;
						break;
					case 1:
						result18 = 1;
						break;
					case 2:
						result18 = 2;
						break;
					default:
						result18 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result18;
				}
			},
			{
				"bg-BG",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ca-ES",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"zh-TW",
				(int n) => 0
			},
			{
				"cs-CZ",
				delegate(int n)
				{
					int result17;
					switch (n)
					{
					case 1:
						result17 = 0;
						break;
					case 2:
					case 3:
					case 4:
						result17 = 1;
						break;
					default:
						result17 = 2;
						break;
					}
					return result17;
				}
			},
			{
				"da-DK",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"de-DE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"el-GR",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"en-US",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fi-FI",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr-FR",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"he-IL",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"hu-HU",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"is-IS",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? 1 : 0
			},
			{
				"it-IT",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ja-JP",
				(int n) => 0
			},
			{
				"ko-KR",
				(int n) => 0
			},
			{
				"nl-NL",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"nb-NO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"pl-PL",
				(int n) => (n != 1) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"pt-BR",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"ro-RO",
				(int n) => (n != 1) ? ((n == 0 || (n % 100 > 0 && n % 100 < 20)) ? 1 : 2) : 0
			},
			{
				"ru-RU",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"hr-HR",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"sk-SK",
				delegate(int n)
				{
					int result16;
					switch (n)
					{
					case 1:
						result16 = 0;
						break;
					case 2:
					case 3:
					case 4:
						result16 = 1;
						break;
					default:
						result16 = 2;
						break;
					}
					return result16;
				}
			},
			{
				"sq-AL",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"sv-SE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"th-TH",
				(int n) => 0
			},
			{
				"tr-TR",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"id-ID",
				(int n) => 0
			},
			{
				"uk-UA",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"be-BY",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"sl-SI",
				(int n) => (n % 100 == 1) ? 1 : ((n % 100 == 2) ? 2 : ((n % 100 == 3 || n % 100 == 4) ? 3 : 0))
			},
			{
				"et-EE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"lv-LV",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n != 0) ? 1 : 2) : 0
			},
			{
				"lt-LT",
				(int n) => (n % 10 != 1 || n % 100 == 11) ? ((n % 10 >= 2 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2) : 0
			},
			{
				"fa-IR",
				(int n) => 0
			},
			{
				"vi-VN",
				(int n) => 0
			},
			{
				"hy-AM",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"eu-ES",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"mk-MK",
				(int n) => (n != 1 && n % 10 != 1) ? 1 : 0
			},
			{
				"af-ZA",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ka-GE",
				(int n) => 0
			},
			{
				"fo-FO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"hi-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"sw-KE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"gu-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ta-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"te-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"kn-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"mr-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"gl-ES",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"kok-IN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-IQ",
				delegate(int n)
				{
					int result15;
					switch (n)
					{
					case 0:
						result15 = 0;
						break;
					case 1:
						result15 = 1;
						break;
					case 2:
						result15 = 2;
						break;
					default:
						result15 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result15;
				}
			},
			{
				"zh-CN",
				(int n) => 0
			},
			{
				"de-CH",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"en-GB",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-MX",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr-BE",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"it-CH",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"nl-BE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"nn-NO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"pt-PT",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"sv-FI",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-EG",
				delegate(int n)
				{
					int result14;
					switch (n)
					{
					case 0:
						result14 = 0;
						break;
					case 1:
						result14 = 1;
						break;
					case 2:
						result14 = 2;
						break;
					default:
						result14 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result14;
				}
			},
			{
				"zh-HK",
				(int n) => 0
			},
			{
				"de-AT",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"en-AU",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-ES",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr-CA",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"ar-LY",
				delegate(int n)
				{
					int result13;
					switch (n)
					{
					case 0:
						result13 = 0;
						break;
					case 1:
						result13 = 1;
						break;
					case 2:
						result13 = 2;
						break;
					default:
						result13 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result13;
				}
			},
			{
				"zh-SG",
				(int n) => 0
			},
			{
				"de-LU",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"en-CA",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-GT",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr-CH",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"ar-DZ",
				delegate(int n)
				{
					int result12;
					switch (n)
					{
					case 0:
						result12 = 0;
						break;
					case 1:
						result12 = 1;
						break;
					case 2:
						result12 = 2;
						break;
					default:
						result12 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result12;
				}
			},
			{
				"zh-MO",
				(int n) => 0
			},
			{
				"en-NZ",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-CR",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"fr-LU",
				(int n) => (n > 1) ? 1 : 0
			},
			{
				"ar-MA",
				delegate(int n)
				{
					int result11;
					switch (n)
					{
					case 0:
						result11 = 0;
						break;
					case 1:
						result11 = 1;
						break;
					case 2:
						result11 = 2;
						break;
					default:
						result11 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result11;
				}
			},
			{
				"en-IE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-PA",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-TN",
				delegate(int n)
				{
					int result10;
					switch (n)
					{
					case 0:
						result10 = 0;
						break;
					case 1:
						result10 = 1;
						break;
					case 2:
						result10 = 2;
						break;
					default:
						result10 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result10;
				}
			},
			{
				"en-ZA",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-DO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-OM",
				delegate(int n)
				{
					int result9;
					switch (n)
					{
					case 0:
						result9 = 0;
						break;
					case 1:
						result9 = 1;
						break;
					case 2:
						result9 = 2;
						break;
					default:
						result9 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result9;
				}
			},
			{
				"es-VE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-YE",
				delegate(int n)
				{
					int result8;
					switch (n)
					{
					case 0:
						result8 = 0;
						break;
					case 1:
						result8 = 1;
						break;
					case 2:
						result8 = 2;
						break;
					default:
						result8 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result8;
				}
			},
			{
				"es-CO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-SY",
				delegate(int n)
				{
					int result7;
					switch (n)
					{
					case 0:
						result7 = 0;
						break;
					case 1:
						result7 = 1;
						break;
					case 2:
						result7 = 2;
						break;
					default:
						result7 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result7;
				}
			},
			{
				"es-PE",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-JO",
				delegate(int n)
				{
					int result6;
					switch (n)
					{
					case 0:
						result6 = 0;
						break;
					case 1:
						result6 = 1;
						break;
					case 2:
						result6 = 2;
						break;
					default:
						result6 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result6;
				}
			},
			{
				"en-TT",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-AR",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-LB",
				delegate(int n)
				{
					int result5;
					switch (n)
					{
					case 0:
						result5 = 0;
						break;
					case 1:
						result5 = 1;
						break;
					case 2:
						result5 = 2;
						break;
					default:
						result5 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result5;
				}
			},
			{
				"en-ZW",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-EC",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-KW",
				delegate(int n)
				{
					int result4;
					switch (n)
					{
					case 0:
						result4 = 0;
						break;
					case 1:
						result4 = 1;
						break;
					case 2:
						result4 = 2;
						break;
					default:
						result4 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result4;
				}
			},
			{
				"en-PH",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-CL",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-AE",
				delegate(int n)
				{
					int result3;
					switch (n)
					{
					case 0:
						result3 = 0;
						break;
					case 1:
						result3 = 1;
						break;
					case 2:
						result3 = 2;
						break;
					default:
						result3 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result3;
				}
			},
			{
				"es-UY",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-BH",
				delegate(int n)
				{
					int result2;
					switch (n)
					{
					case 0:
						result2 = 0;
						break;
					case 1:
						result2 = 1;
						break;
					case 2:
						result2 = 2;
						break;
					default:
						result2 = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result2;
				}
			},
			{
				"es-PY",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"ar-QA",
				delegate(int n)
				{
					int result;
					switch (n)
					{
					case 0:
						result = 0;
						break;
					case 1:
						result = 1;
						break;
					case 2:
						result = 2;
						break;
					default:
						result = ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 < 11) ? 5 : 4));
						break;
					}
					return result;
				}
			},
			{
				"es-BO",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-SV",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-HN",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-NI",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"es-PR",
				(int n) => (n != 1) ? 1 : 0
			},
			{
				"zh-CHT",
				(int n) => 0
			},
			{
				"ms",
				(int n) => 0
			}
		};
	}
}
