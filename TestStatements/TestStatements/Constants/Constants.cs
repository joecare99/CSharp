// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace TestStatements.Constants
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region "real" Constants
        /// <summary>
        /// The line ending
        /// </summary>
        private const string LineEnding = "\r\n";

        /// <summary>
        /// The famous "Hello World !"
        /// </summary>
        public const string HelloWorld = "Hello World !";

        /// <summary>
        /// The header
        /// </summary>
        public const string Header = "======================================================================" + LineEnding +
            "## {0}" + LineEnding +
            "======================================================================";

        /// <summary>
        /// The header2
        /// </summary>
        public const string Header2 = LineEnding+"+----------------------------------------------------------" +LineEnding+
            "| {0}" + LineEnding+
            "+----------------------------------------------------------";

        public const string LoremIpsum =
@"Lorem ipsum dolor sit amet consectetur adipiscing elit natoque a ultricies 
id viverra ridiculus, felis lectus neque commodo dis blandit fames proin non 
est pellentesque. Dignissim sem enim rhoncus penatibus lacus magnis praesent 
bongue libero, faucibus ullamcorper dapibus per placerat convallis blandit mus 
eristique habitasse, nostra risus velit varius nisl tempor vestibulum cubilia 
remorbi. 

Tempus tristique enim vulputate cras accumsan arcu quisque, phasellus dictum 
res tincidunt netus nec diam aenean, dapibus vestibulum platea convallis 
ornare semper risus, ut laoreet duis varius quam nisi. Penatibus quisque elit 
sed dolor eros vulputate molestie lacus, gravida ornare mi elementum egestas 
tempora rhoncus nibh, scelerisque pellentesque cras magna est tempus. 

Pharetra mi ullamcorper justo leo dis sit viverra, sodales dictumst tellus 
aliquam mattis penatibus mauris at, dapibus bibendum sapien morbi eu quisque. 
Gravida adipiscing vehicula leo fames vitae volutpat himenaeos potenti, duis 
montes semper ipsum urna dignissim neque vulputate, nec nisl mus litora cursus 
ad mi. Tristique lobortis nisl nam metus fusce porttitor bibendum scelerisque 
vel euismod, aptent duis senectus felis sollicitudin dis porta morbi taciti 
phasellus quisque, mattis nisi commodo quis purus urna sapien nec fringilla. 
Cursus leo varius risus cum duis etiam quis, sed nascetur mattis tempus 
habitasse morbi, imperdiet magna enim lobortis metus primis. Rutrum arcu 
pretium lacinia vestibulum a velit neque, ipsum tempor id himenaeos elit turpis 
sodales, interdum enim vel primis libero sociosqu. 

Nibh lorem ullamcorper tempor ridiculus curae sollicitudin porttitor eu, 
dignissim nunc a sociosqu primis himenaeos mollis taciti vulputate, vehicula 
imperdiet aptent aliquam pretium pharetra integer. At aenean arcu faucibus 
dapibus amet varius imperdiet aliquam nulla aptent, est litora ridiculus metus 
commodo sociis sem mauris habitasse dictumst platea, vehicula libero cursus 
velit justo cubilia sit mollis odio. Faucibus praesent enim dis mattis hac 
luctus nam cras, tempus platea neque convallis elit pellentesque pulvinar, 
himenaeos et curabitur bibendum etiam blandit netus. Vehicula praesent mollis 
parturient mauris nullam magna platea, tellus placerat natoque curae eu aliquet 
fermentum nibh, diam quam fusce convallis senectus litora. Eu tristique aptent 
lacinia pulvinar rhoncus nec mollis consectetur amet faucibus lobortis, rutrum 
taciti sodales dignissim diam arcu aliquet congue dolor interdum, class 
habitasse integer vestibulum imperdiet ipsum sollicitudin ad posuere cras. 

Ligula laoreet fermentum quisque primis class tellus, enim nam nullam malesuada 
viverra, metus phasellus blandit posuere placerat. Scelerisque malesuada 
feugiat porta habitasse metus sem nulla etiam tristique morbi, nostra egestas 
fusce amet commodo lacinia porttitor iaculis. Dis integer scelerisque sociosqu 
sodales parturient senectus habitasse euismod, porttitor leo ornare consectetur 
phasellus massa nibh cursus, diam id pretium primis enim eu nascetur. Ultrices 
dignissim elementum imperdiet ornare tincidunt tempus congue eget, odio nibh 
gravida ad leo turpis at, lobortis litora nec et magna senectus erat. 

Orci litora pretium congue etiam dis diam hendrerit rhoncus vitae nisl, 
nascetur nisi felis turpis duis metus parturient malesuada condimentum gravida 
erat, tincidunt volutpat maecenas ad euismod cubilia eget senectus praesent. 
Est ultricies quam tristique senectus dapibus montes, purus integer semper 
ipsum nunc. Condimentum lacinia nisi sociosqu mollis interdum nec ornare velit 
rutrum, fames magnis vivamus vehicula cras lectus ipsum sodales. Curabitur 
interdum pulvinar integer laoreet phasellus placerat pretium fusce praesent, 
nulla enim massa nunc nec cursus dapibus tempor, ornare natoque imperdiet mi 
penatibus sodales eu suspendisse. 

Dictum ut dignissim diam porta molestie nascetur vehicula magna ornare vel non 
congue, tempus metus magnis neque platea duis convallis himenaeos eros euismod 
aenean, scelerisque id est commodo ridiculus blandit fames gravida class 
venenatis urna. Sodales pretium ad sociosqu magnis porta erat luctus potenti, 
venenatis ornare tempor dolor class quis varius rutrum risus, eu volutpat magna 
consectetur semper vivamus ac. Platea massa semper sodales rhoncus nostra 
pharetra dis nunc quisque taciti ad vehicula curae, sed torquent malesuada 
etiam praesent amet lacinia iaculis gravida nascetur accumsan pellentesque. 
Duis ac praesent et natoque sollicitudin sociis eu dapibus dignissim lectus 
imperdiet egestas luctus quam, id interdum accumsan ornare ultrices eros magnis 
magna netus curabitur aptent diam neque. Ligula vulputate massa tincidunt ut 
elit condimentum hendrerit elementum ullamcorper non, convallis vel habitant 
facilisi sed vestibulum tristique metus lobortis tortor superiorem, situs 
vilate inisse savernit, Johannem orate et Cummulus ad rabiat ecclesia.";

        /// <summary>The golden cut.</summary>
        public const double dGoldenCut = 1.6180339887498948482045868343656d;
        #endregion

        #region Static Readonly "Constants"
        public static readonly double dGoldenCut2 = (Math.Sqrt(5d) + 1d) * 0.5d;

        public static readonly int[] iLowPrimes = { 2, 3, 5, 7 };

        public static readonly double[] dMathConst = {dGoldenCut2, Math.PI, Math.E, double.Epsilon,double.MinValue,double.MaxValue,double.NaN,double.PositiveInfinity,double.NegativeInfinity};
        #endregion

    }
}
