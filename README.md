# <img src="https://github.com/RobThree/LicenseplateValidator/blob/master/LicenseplateValidator/logo.png?raw=true" width="48" height="48"> LicenseplateValidator
Fun little brain teaser turned into a small library

## Main methods:

````c#
var lpv = new LicensePlateValidator(); // By default only knows about dutch license plate formats ("sidecodes")

lpv.IsValidPlate("AB-12-CD", "NL");  // Returns true
lpv.FormatPlate("AB12CD", "NL");     // Returns "AB-12-CD"
lpv.FindSideCode("AB12CD", "NL");    // Returns "XX-99-XX"
````

Most methods have a `ignoreDashes` argument; when true, dashes in the input are ignored. When false, dashes should be in the correct place.

````c#
target.IsValidPlate("AB12CD", "NL", ignoreDashes: false);    // Returns false
target.IsValidPlate("AB-12-CD", "NL", ignoreDashes: false);  // Returns true

target.IsValidPlate("AB12CD", "NL", ignoreDashes: true);     // Returns true
target.IsValidPlate("AB-12-CD", "NL", ignoreDashes: true);   // Returns true
````

Most methods will throw on errors; there are `Try...` methods that will not throw on common errors (unless documented).

## Other countries / [sidecodes](https://nl.wikipedia.org/wiki/Nederlands_kenteken#Sidecodes):

The `LicensePlateValidator` supports other countries. All you need to do is pass in the allowed sidecodes. These are codes like `XX-99-XX` which describe where digits (`9`), letters (`X`), digit _or_ letter (`?`) and dashes (`-`) should go in a plate. You can specify these, along with the country codes, in the constructor:

````c#
var lpv = new LicenseplateValidator(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
{
    { "XX", new[] { "XX-99-XX", "X9-9X-X9", "99-X-99X" } },
    { "YY", new[] { "99-XX-99", "XXX-999" } }
});
````
The above initializes a new `LicenseplateValidator` with two fictional countries (`"XX"` and `"YY"`) with the sidecodes for each country. Note that because we use an overload of the dictionary that makes the key comparer case-insensitive you can pass `"XX"`, `"Xx"`, `"xX"` or `"xx"` as countrycode to all methods and this will work fine. If you don't want this behaviour then simply use the parameterless dictionary constructor or pass any other `IDictionary<string, string[]>` that implements the desired key comparison. Also note that you don't *HAVE* to stick with country codes like "NL", "XX" etc. You can use whatever key you like.

## About actual validation

This project / library was born out of a little brainteaser that got a little out of hand. However; that doesn't mean in any way that this library is 100% correct. In fact, it doesn't even make any _attempt_ to be correct. For example, ANY letter will be allowed where some sidecodes don't allow certain letters (like [the letter `C`](https://nl.wikipedia.org/wiki/Nederlands_kenteken#Sidecode_1) which isn't valid in Sidecode 1). Also some letters/digits are only allowed in certain places. All these minor details have _not_ been added to this library. This is a quick'n'dirty fun little project that works fine for most purposes. If you want all the nitty gritty details supported or implemented then I suggest you look elsewhere (or make a pull request 😉). Methods like `IsValid()` therefore only return a 'minimal effort' result, an 'estimate' of whether a plate _could_ be valid.

<hr>

[Icon](https://www.flaticon.com/free-icon/license-plate_310662) made by [Freepik](http://www.flaticon.com/authors/freepik) from [www.flaticon.com](http://www.flaticon.com) is licensed by [CC 3.0](http://creativecommons.org/licenses/by/3.0/).
