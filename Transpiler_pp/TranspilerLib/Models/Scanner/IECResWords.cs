﻿namespace TranspilerLib.Models.Scanner;

public enum IECResWords
{
    rw_ABS, rw_ABSTRACT, rw_ACOS, rw_ACTION, rw_ADD,
    rw_AND, rw_ARRAY, rw_ASIN, rw_AT, rw_ATAN,
    rw_ATAN2, rw_BEGIN, rw_BOOL, rw_BY, rw_BYTE, rw_CASE,
    rw_CHAR, rw_CLASS, rw_CONCAT, rw_CONFIGURATION, rw_CONSTANT,
    rw_CONTINUE, rw_COS, rw_CTD, rw_CTU, rw_CTUD,
    rw_DATE, rw_DATE_AND_TIME, rw_DELETE, rw_DINT, rw_DIV,
    rw_DO, rw_DT, rw_DWORD, rw_ELSE, rw_ELSIF,
    rw_END_ACTION, rw_END_CASE, rw_END_CLASS, rw_END_CONFIGURATION, rw_END_FOR,
    rw_END_FUNCTION, rw_END_FUNCTION_BLOCK, rw_END_IF, rw_END_INTERFACE, rw_END_METHOD,
    rw_END_NAMESPACE, rw_END_PROGRAM, rw_END_REPEAT, rw_END_RESOURCE, rw_END_STEP,
    rw_END_STRUCT, rw_END_TRANSITION, rw_END_TYPE, rw_END_VAR, rw_END_WHILE,
    rw_EQ, rw_EXIT, rw_EXP, rw_EXPT, rw_EXTENDS,
    rw_F_EDGE, rw_F_TRIG, rw_FALSE, rw_FINAL, rw_FIND,
    rw_FOR, rw_FROM, rw_FUNCTION, rw_FUNCTION_BLOCK, rw_GE,
    rw_GT, rw_IF, rw_IMPLEMENTS, rw_INITIAL_STEP, rw_INSERT,
    rw_INT, rw_INTERFACE, rw_INTERNAL, rw_INTERVAL, rw_LD,
    rw_LDATE, rw_LDATE_AND_TIME, rw_LDT, rw_LE, rw_LEFT,
    rw_LEN, rw_LIMIT, rw_LINT, rw_LN, rw_LOG,
    rw_LREAL, rw_LT, rw_LTIME, rw_LTIME_OF_DAY, rw_LTOD,
    rw_LWORD, rw_MAX, rw_METHOD, rw_MID, rw_MIN,
    rw_MOD, rw_MOVE, rw_MUL, rw_MUX, rw_NAMESPACE,
    rw_NE, rw_NON_RETAIN, rw_NOT, rw_NULL, rw_OF,
    rw_ON, rw_OR, rw_OVERLAP, rw_OVERRIDE, rw_PRIORITY,
    rw_PRIVATE, rw_PROGRAM, rw_PROTECTED, rw_PUBLIC, rw_R_EDGE,
    rw_R_TRIG, rw_READ_ONLY, rw_READ_WRITE, rw_REAL, rw_REF,
    rw_REF_TO, rw_REPEAT, rw_REPLACE, rw_RESOURCE, rw_RETAIN,
    rw_RETURN, rw_RIGHT, rw_ROL, rw_ROR, rw_RS,
    rw_SEL, rw_SHL, rw_SHR, rw_SIN, rw_SINGLE,
    rw_SINT, rw_SQRT, rw_SR, rw_STEP, rw_STRING,
    rw_STRUCT, rw_SUB, rw_SUPER, rw_T, rw_TAN,
    rw_TASK, rw_THEN, rw_THIS, rw_TIME, rw_TIME_OF_DAY,
    rw_TO, rw_TOD, rw_TOF, rw_TON, rw_TP,
    rw_TRANSITION, rw_TRUE, rw_TRUNC, rw_TYPE, rw_UDINT,
    rw_UINT, rw_ULINT, rw_UNTIL, rw_USING, rw_USINT,
    rw_VAR, rw_VAR_ACCESS, rw_VAR_CONFIG, rw_VAR_EXTERNAL, rw_VAR_GLOBAL,
    rw_VAR_IN_OUT, rw_VAR_INPUT, rw_VAR_INST, rw_VAR_OUTPUT, rw_VAR_TEMP, rw_WCHAR,
    rw_WHILE, rw_WITH, rw_WORD, rw_WSTRING, rw_XOR,
    rw_UNION, rw_END_UNION,
// internal Converter-Functions
    rw_DINT_TO_REAL, rw_REAL_TO_DINT, rw_INT_TO_REAL, rw_REAL_TO_INT,
    rw_DINT_TO_LREAL, rw_LREAL_TO_DINT, rw_INT_TO_LREAL, rw_LREAL_TO_INT,
    rw_DINT_TO_INT, rw_INT_TO_DINT, rw_REAL_TO_LREAL, rw_LREAL_TO_REAL,
    rw_UDINT_TO_REAL, rw_REAL_TO_UDINT, rw_UINT_TO_REAL, rw_REAL_TO_UINT,
    rw_UDINT_TO_LREAL, rw_LREAL_TO_UDINT, rw_UINT_TO_LREAL, rw_LREAL_TO_UINT,
    rw_UDINT_TO_INT, rw_UINT_TO_DINT, rw_UDINT_TO_UINT, rw_UINT_TO_UDINT,
    rw_REAL_TO_STRING, rw_STRING_TO_REAL, rw_INT_TO_STRING, rw_STRING_TO_INT,
    rw_DINT_TO_STRING, rw_STRING_TO_DINT, rw_LREAL_TO_STRING, rw_STRING_TO_LREAL,
    rw_BOOL_TO_STRING, rw_STRING_TO_BOOL, rw_TIME_TO_STRING, rw_STRING_TO_TIME,
    rw_DATE_TO_STRING, rw_STRING_TO_DATE, rw_TIME_OF_DAY_TO_STRING, rw_STRING_TO_TIME_OF_DAY,
    rw_DATE_AND_TIME_TO_STRING, rw_STRING_TO_DATE_AND_TIME,
    rw_BYTE_TO_STRING, rw_STRING_TO_BYTE, rw_WORD_TO_STRING, rw_STRING_TO_WORD,
    rw_DWORD_TO_STRING, rw_STRING_TO_DWORD, rw_LWORD_TO_STRING, rw_STRING_TO_LWORD,
    rw_SINT_TO_STRING, rw_STRING_TO_SINT, rw_USINT_TO_STRING, rw_STRING_TO_USINT,
    rw_UINT_TO_STRING, rw_STRING_TO_UINT,
    // internal implicit type conversion
    rw_TO_STRING, rw_TO_BOOL, rw_TO_BYTE, rw_TO_CHAR, rw_TO_DATE,
    rw_TO_DATE_AND_TIME, rw_TO_DINT, rw_TO_DT, rw_TO_DWORD, rw_TO_INT,
    rw_TO_LDATE, rw_TO_LDATE_AND_TIME, rw_TO_LINT, rw_TO_LREAL, rw_TO_LTIME,
}
