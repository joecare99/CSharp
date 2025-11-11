using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Models;

/// <summary>
/// Represents all token kinds produced by the scanner / lexer.
/// </summary>
public enum EToken
{
    /// <summary>End of input.</summary>
    tkEOF,
    /// <summary>Whitespace (space, tab, etc.).</summary>
    tkWhitespace,
    /// <summary>Identifier.</summary>
    tkIdentifier,
    /// <summary>Label (identifier followed by colon in some languages).</summary>
    tkLabel,
    /// <summary>String constant.</summary>
    tkStringConst,
    /// <summary>Numeric literal.</summary>
    tkNumber,
    /// <summary>Control character / character literal (^A .. ^Z).</summary>
    tkCharacter, // ^A .. ^Z
    /// <summary>Line ending (newline sequence).</summary>
    tkLineEnding, // normal LineEnding
    /// <summary>Tabulator character.</summary>
    tkTab, // a Tabulator-Key

    /// <summary>C style line comment //.</summary>
    tkLineComment, // //
    /// <summary>C style block comment /* ... */.</summary>
    tkComment, // /* ... */

    // Simple (one-character) tokens
    /// <summary>'{'</summary>
    tkCurlyBraceOpen, // '{'
    /// <summary>'}'</summary>
    tkCurlyBraceClose, // '}'
    /// <summary>'('</summary>
    tkBraceOpen, // '('
    /// <summary>')'</summary>
    tkBraceClose, // ')'
    /// <summary>'*'</summary>
    tkMul, // '*'
    /// <summary>'+'</summary>
    tkPlus, // '+'
    /// <summary>','</summary>
    tkComma, // ','
    /// <summary>'-'</summary>
    tkMinus, // '-'
    /// <summary>'.'</summary>
    tkDot, // '.')
    /// <summary>'/'</summary>
    tkDivision, // '/'
    /// <summary>':'</summary>
    tkColon, // ':'
    /// <summary>';'</summary>
    tkSemicolon, // ';'
    /// <summary>'%lt;'</summary>
    tkLessThan, // '<'
    /// <summary>'=' (single assign)</summary>
    tkAssign, // '='
    /// <summary>'>'</summary>
    tkGreaterThan, // '>'
                   // tkAt, // '@'
    /// <summary>'['</summary>
    tkSquaredBraceOpen, // '['
    /// <summary>']'</summary>
    tkSquaredBraceClose, // ']'
    /// <summary>'^' (xor operator)</summary>
    tkXor, // '^' (xor operator)
    /// <summary>'\'</summary>
    tkBackslash, // '\'
    /// <summary>'&amp;'</summary>
    tkSingleAnd, // '&'
    /// <summary>'|'</summary>
    tkSingleOr, // '|'
    /// <summary>'!'</summary>
    tkNot, // '!'
    /// <summary>'?'</summary>
    tkAsk, // '?'
    /// <summary>'%'</summary>
    tkMod, // '%'
    /// <summary>'~'</summary>
    tkKomplement, // '~'
                  // Two-character tokens
    /// <summary>'==' equality operator.</summary>
    tkEqual, // '=='
    /// <summary>'=>' lambda operator.</summary>
    tkLambda, // '=>'
    /// <summary>'..' range / spread operator.</summary>
    tkDotDot, // '..'
    /// <summary>'!=' not equal operator.</summary>
    tkNotEqual, // '!='
    /// <summary>'&lt;=' less or equal operator.</summary>
    tkLessEqualThan, // '<='
    /// <summary>'&gt;=' greater or equal operator.</summary>
    tkGreaterEqualThan, // '>='
    /// <summary>'**' power operator.</summary>
    tkPower, // '**'
    /// <summary>'>&lt;' symmetrical difference operator.</summary>
    tkSymmetricalDifference, // '><'
    /// <summary>'??' null-coalescing operator.</summary>
    tkAskAsk, // '??'
    /// <summary>'++' increment operator.</summary>
    tkPlusPlus, // '++'
    /// <summary>'--' decrement operator.</summary>
    tkMinusMinus, // '--'
    /// <summary>'+=' addition assignment.</summary>
    tkAssignPlus, // '+='
    /// <summary>'-=' subtraction assignment.</summary>
    tkAssignMinus, // '-='
    /// <summary>'*=' multiplication assignment.</summary>
    tkAssignMul, // '*='
    /// <summary>'/=' division assignment.</summary>
    tkAssignDivision, // '/='
    /// <summary>'%=' modulo assignment.</summary>
    tkAssignModulo, // '%='
    /// <summary>'&amp;=' bitwise and assignment.</summary>
    tkAssignAnd, // '&='
    /// <summary>'|=' bitwise or assignment.</summary>
    tkAssignOr, // '|='
    /// <summary>'^=' bitwise xor assignment.</summary>
    tkAssignXor, // '^='
    /// <summary>'&amp;&amp;' logical and operator.</summary>
    tkAnd, // '&&'
    /// <summary>'||' logical or operator.</summary>
    tkOr, // '||'
    /// <summary>'%lt;%lt;' shift-left operator.</summary>
    tkShl, // '<<'
    /// <summary>'>>' shift-right operator.</summary>
    tkShr, // '>>'
           // Three-Character token
    /// <summary>'&lt;%lt;=' shift-left assignment.</summary>
    tkAssignShl, // '<<='
    /// <summary>'>>=' shift-right assignment.</summary>
    tkAssignShr, // '>>='
    /// <summary>'??=' null-coalescing assignment.</summary>
    tkAssignAsk, // '??='

    // Reserved words
    /// <summary>'abstract' keyword.</summary>
    tkAbstract,
    /// <summary>'as' keyword.</summary>
    tkAs,
    /// <summary>'base' keyword.</summary>
    tkBase,
    /// <summary>'bool' keyword.</summary>
    tkBool,
    /// <summary>'break' keyword.</summary>
    tkBreak,
    /// <summary>'byte' keyword.</summary>
    tkByte,
    /// <summary>'case' keyword.</summary>
    tkCase,
    /// <summary>'catch' keyword.</summary>
    tkCatch,
    /// <summary>'char' keyword.</summary>
    tkChar,
    /// <summary>'checked' keyword.</summary>
    tkChecked,
    /// <summary>'class' keyword.</summary>
    tkClass,
    /// <summary>'const' keyword.</summary>
    tkConst,
    /// <summary>'continue' keyword.</summary>
    tkContinue,
    /// <summary>'decimal' keyword.</summary>
    tkDecimal,
    /// <summary>'default' keyword.</summary>
    tkDefault,
    /// <summary>'delegate' keyword.</summary>
    tkDelegate,
    /// <summary>'do' keyword.</summary>
    tkDo,
    /// <summary>'double' keyword.</summary>
    tkDouble,
    /// <summary>'else' keyword.</summary>
    tkElse,
    /// <summary>'enum' keyword.</summary>
    tkEnum,
    /// <summary>'event' keyword.</summary>
    tkEvent,
    /// <summary>'explicit' keyword.</summary>
    tkExplicit,
    /// <summary>'extern' keyword.</summary>
    tkExtern,
    /// <summary>'false' literal.</summary>
    tkFalse,
    /// <summary>'finally' keyword.</summary>
    tkFinally,
    /// <summary>'fixed' keyword.</summary>
    tkFixed,
    /// <summary>'float' keyword.</summary>
    tkFloat,
    /// <summary>'for' keyword.</summary>
    tkFor,
    /// <summary>'foreach' keyword.</summary>
    tkForeach,
    /// <summary>'goto' keyword.</summary>
    tkGoto,
    /// <summary>'if' keyword.</summary>
    tkIf,
    /// <summary>'implicit' keyword.</summary>
    tkImplicit,
    /// <summary>'in' keyword.</summary>
    tkIn,
    /// <summary>'int' keyword.</summary>
    tkInt,
    /// <summary>'interface' keyword.</summary>
    tkInterface,
    /// <summary>'internal' keyword.</summary>
    tkInternal,
    /// <summary>'is' keyword.</summary>
    tkIs,
    /// <summary>'lock' keyword.</summary>
    tkLock,
    /// <summary>'long' keyword.</summary>
    tkLong,
    /// <summary>'namespace' keyword.</summary>
    tkNamespace,
    /// <summary>'new' keyword.</summary>
    tkNew,
    /// <summary>'null' literal.</summary>
    tkNull,
    /// <summary>'object' keyword.</summary>
    tkObject,
    /// <summary>'operator' keyword.</summary>
    tkOperator,
    /// <summary>'out' keyword.</summary>
    tkOut,
    /// <summary>'override' keyword.</summary>
    tkOverride,
    /// <summary>'params' keyword.</summary>
    tkParams,
    /// <summary>'private' keyword.</summary>
    tkPrivate,
    /// <summary>'protected' keyword.</summary>
    tkProtected,
    /// <summary>'public' keyword.</summary>
    tkPublic,
    /// <summary>'readonly' keyword.</summary>
    tkReadonly,
    /// <summary>'ref' keyword.</summary>
    tkRef,
    /// <summary>'return' keyword.</summary>
    tkReturn,
    /// <summary>'sbyte' keyword.</summary>
    tkSbyte,
    /// <summary>'sealed' keyword.</summary>
    tkSealed,
    /// <summary>'short' keyword.</summary>
    tkShort,
    /// <summary>'sizeof' keyword.</summary>
    tkSizeof,
    /// <summary>'stackalloc' keyword.</summary>
    tkStackalloc,
    /// <summary>'static' keyword.</summary>
    tkStatic,
    /// <summary>'string' keyword.</summary>
    tkString,
    /// <summary>'struct' keyword.</summary>
    tkStruct,
    /// <summary>'switch' keyword.</summary>
    tkSwitch,
    /// <summary>'this' keyword.</summary>
    tkThis,
    /// <summary>'throw' keyword.</summary>
    tkThrow,
    /// <summary>'true' literal.</summary>
    tkTrue,
    /// <summary>'try' keyword.</summary>
    tkTry,
    /// <summary>'typeof' keyword.</summary>
    tkTypeof,
    /// <summary>'uint' keyword.</summary>
    tkUint,
    /// <summary>'ulong' keyword.</summary>
    tkUlong,
    /// <summary>'unchecked' keyword.</summary>
    tkUnchecked,
    /// <summary>'unsafe' keyword.</summary>
    tkUnsafe,
    /// <summary>'ushort' keyword.</summary>
    tkUshort,
    /// <summary>'using' keyword.</summary>
    tkUsing,
    /// <summary>'virtual' keyword.</summary>
    tkVirtual,
    /// <summary>'void' keyword.</summary>
    tkVoid,
    /// <summary>'volatile' keyword.</summary>
    tkVolatile,
    /// <summary>'while' keyword.</summary>
    tkWhile
}
