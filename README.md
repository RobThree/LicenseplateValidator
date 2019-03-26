# LicenseplateValidator
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

## Other countries / sidecodes:

The `LicensePlateValidator` supports other countries. All you need to do is pass in the allowed sidecodes. These are codes like `XX-99-XX` which describe where digits (`9`), letters (`X`), digit _or_ letter (`?`) and dashes (`-`) should go in a plate. You can specify these, along with the country codes, in the constructor:

````c#
var lpv = new LicenseplateValidator(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
{
    { "XX", new[] { "XX-99-XX", "X9-9X-X9" } },
    { "YY", new[] { "99-XX-99", "XXX-999" } }
});
````
The above initializes a new `LicenseplateValidator` with two fictional countries ("XX" and "YY) with the sidecodes for each country. Note that because we use an overload of the dictionary that makes the key comparer case-insensitive you can pass `"XX"`, `"Xx"`, `"xX"` or `"xx"` as countrycode to all methods and this will work fine. If you don't want this behaviour then simply use the parameterless dictionary constructor or pass any other `IDictionary<string, string[]>` that implements the required key comparison. Also note that you don't *HAVE* to stick with country codes like "NL", "XX" etc. You can use whatever key you like.
