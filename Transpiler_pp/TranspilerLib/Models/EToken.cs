using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Models;

public enum EToken
{
    tkEOF,
    tkWhitespace,
    tkIdentifier,
    tkLabel,
    tkStringConst,
    tkNumber,
    tkCharacter, // ^A .. ^Z
    tkLineEnding, // normal LineEnding
    tkTab, // a Tabulator-Key

    tkLineComment,  // //
    tkComment,      // /* ... */

    // Simple (one-character) tokens
    tkCurlyBraceOpen,        // '{'
    tkCurlyBraceClose,       // '}'
    tkBraceOpen,             // '('
    tkBraceClose,            // ')'
    tkMul,                   // '*'
    tkPlus,                  // '+'
    tkComma,                 // ','
    tkMinus,                 // '-'
    tkDot,                   // '.'
    tkDivision,              // '/'
    tkColon,                 // ':'
    tkSemicolon,             // ';'
    tkLessThan,              // '<'
    tkAssign,                // '='
    tkGreaterThan,           // '>'
                             //        tkAt,                    // '@'
    tkSquaredBraceOpen,      // '['
    tkSquaredBraceClose,     // ']'
    tkXor,                   // '^' (xor operator)
    tkBackslash,             // '\'
    tkSingleAnd,             // '&'
    tkSingleOr,              // '|'
    tkNot,                   // '!'
    tkAsk,                   // '?'
    tkMod,                   // '%'
    tkKomplement,            // '~'
                             // Two-character tokens
    tkEqual,                 // '=='
    tkLambda,                // '=>'
    tkDotDot,                // '..'
    tkNotEqual,              // '!='
    tkLessEqualThan,         // '<='
    tkGreaterEqualThan,      // '>='
    tkPower,                 // '**'
    tkSymmetricalDifference, // '><'
    tkAskAsk,                // '??'
    tkPlusPlus,              // '++'
    tkMinusMinus,            // '--'
    tkAssignPlus,            // '+='
    tkAssignMinus,           // '-='
    tkAssignMul,             // '*='
    tkAssignDivision,        // '/='
    tkAssignModulo,          // '%='
    tkAssignAnd,             // '&='
    tkAssignOr,              // '|='
    tkAssignXor,             // '^='
    tkAnd,                   // '&&'
    tkOr,                    // '||'
    tkShl,                   // '<<'
    tkShr,                   // '>>'
                             // Three-Character token
    tkAssignShl,             // '<<='
    tkAssignShr,             // '>>='
    tkAssignAsk,             // '??='

    // Reserved words
    tkAbstract, tkAs, tkBase, tkBool,
    tkBreak, tkByte, tkCase, tkCatch,
    tkChar, tkChecked, tkClass, tkConst,
    tkContinue, tkDecimal, tkDefault, tkDelegate,
    tkDo, tkDouble, tkElse, tkEnum,
    tkEvent, tkExplicit, tkExtern, tkFalse,
    tkFinally, tkFixed, tkFloat, tkFor,
    tkForeach, tkGoto, tkIf, tkImplicit,
    tkIn, tkInt, tkInterface, tkInternal,
    tkIs, tkLock, tkLong, tkNamespace,
    tkNew, tkNull, tkObject, tkOperator,
    tkOut, tkOverride, tkParams, tkPrivate,
    tkProtected, tkPublic, tkReadonly, tkRef,
    tkReturn, tkSbyte, tkSealed, tkShort,
    tkSizeof, tkStackalloc, tkStatic, tkString,
    tkStruct, tkSwitch, tkThis, tkThrow,
    tkTrue, tkTry, tkTypeof, tkUint,
    tkUlong, tkUnchecked, tkUnsafe, tkUshort,
    tkUsing, tkVirtual, tkVoid, tkVolatile,
    tkWhile
}

