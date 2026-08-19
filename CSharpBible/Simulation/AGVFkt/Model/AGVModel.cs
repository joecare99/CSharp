using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using AGVFkt.Model.Interface;
using MathLibrary.TwoDim;

namespace AGVFkt.Model;

public record ST_CfgLimitsIntern(
    double lrStdAccRotational,
    double lrStdDecRotational,
    double lrEStopDecRotational,
    double lrMovementDeadbandThresholdRotational)
{
    public ST_CfgLimitsIntern() : this(6d, 6d, 8d, 0.001d) { }
}
public record ST_ConfigLimitsUser(
    double lrSpeedLimitForward,
    double lrSpeedLimitBackward,
    double lrSpeedLimitRotational,
    double lrMovementDeadbandThreshold,
    double lrStdAccLateral,
    double lrStdDecLateral,
    double lrEStopDecLateral,
    UInt16 uiStdJerkLateral,
    ST_CfgLimitsIntern stCfgLimitsIntern)
{
    public ST_ConfigLimitsUser() : this(1000.0, 1000.0, 3.0, 0.001, 900.0, 1200.0, 1500.0, 2, new()) { }
}

public record ST_ConfigMaster(double lrEStopDecLateral)
{
    public ST_ConfigMaster() : this(1500d) { }
}

public class AGVModel
{
    const double dHyst = 10.0 / 180.0 * Math.PI;
    bool xPos, xNeg;
    public (double dAngle, double dSpeedF) LenkHystereseEugen(double dAngle)
    {
        double dSpeedF = 1d;
        if (xPos && dAngle < (-Math.PI / 2 + dHyst))
        {
            dAngle = Math.PI + dAngle;
            dSpeedF *= -1d;
        }
        else
            xPos = false;

        if (dAngle > Math.PI / 4)
            xPos = true;

        if (xNeg && dAngle > (Math.PI / 2 - dHyst))
        {
            dAngle = -Math.PI + dAngle;
            dSpeedF *= -1d;
        }
        else
            xNeg = false;

        if (dAngle < -Math.PI / 4)
            xNeg = true;

        return (dAngle, dSpeedF);
    }

    public (double dAngle, double dSpeedF) LenkHysterese(double dSetAngle, double dActAngle)
    {
        if (Math.Abs(Math2d.WinkelNorm(dSetAngle, dActAngle) - dActAngle) > Math.PI / 2)
            return (Math2d.WinkelNorm(dSetAngle + Math.PI, dActAngle), -1d);
        else
            return (dSetAngle, 1d);
    }
    /*      public double Alining(bool xIsAlining,double dSetAngle,double dActAngle)
          {
              /* (***********************************************************
           (*actual velocity of the vehicle*)
           ***********************************************************)
           IF _stSwivel.xIsAligning THEN
               _lrActualRotVelo:= 0;
           ELSE
               IF eTypeOfAgv = 10 THEN             // Single Swivel
                           (*Omega: Drehrate *)
                           _lrActualRotVelo:= ((stInterfaceToWheelLeft.lrActualVelocity + stInterfaceToWheelRight.lrActualVelocity) / 2) * SIN(_stSwivel.lrActualSteering / _clrRad) / rDistanceToCenterForward;
               ELSIF eTypeOfAgv = 20 THEN        // Omni
                           (*Omega: Drehrate *)
                           _lrActualRotVelo:= ((stInterfaceToWheelRight.lrActualVelocity - stInterfaceToWheelLeft.lrActualVelocity) / REAL_TO_LREAL(rDistanceWheels));
               ELSIF eTypeOfAgv = 30 THEN        // Differential
                           (*Omega: Drehrate *)
                           _lrActualRotVelo:= ((stInterfaceToWheelRight.lrActualVelocity - stInterfaceToWheelLeft.lrActualVelocity) / REAL_TO_LREAL(rDistanceWheels));
               END_IF
           END_IF * /
              if (xIsAlining)
              {
                  var _lrActualRotVelo = 0d;
              }
              else
              {

              }

  /*Jitter supression *)
  IF ABS(_lrActualRotVelo) < 0.01 THEN
              _lrActualRotVelo := 0;
              END_IF

              _lrActualTransVelo                            := (stInterfaceToWheelRight.lrActualVelocity + stInterfaceToWheelLeft.lrActualVelocity) / 2;

          _lrCOSSteering:= COS(_stSwivel.lrActualSteering / _clrRad);
          _lrSINSteering:= SIN(_stSwivel.lrActualSteering / _clrRad);

          _lrActualCalcRotVeloForward:= (REAL_TO_LREAL(rDistanceToCenterSideways) * _lrActualRotVelo);
          _lrActualCalcRotVeloSideways:= (REAL_TO_LREAL(rDistanceToCenterForward) * _lrActualRotVelo);

          _lrActualVelocityForward:= (_lrActualTransVelo * _lrCOSSteering) + _lrActualCalcRotVeloForward;
          _lrActualVelocitySideways:= (_lrActualTransVelo * _lrSINSteering) - _lrActualCalcRotVeloSideways;


          }
          public AGVModel() { }*/

    /*
     *CASE TO_INT(_stInputDataTxPDO1^.eNumberLCP AND INT_TO_BYTE(SEW_MKC_VTGS.E_NumberLCP.E_LCP_3_TRACK_FOUND_OR_INTERSECTION)) OF

SEW_MKC_VTGS.E_NumberLCP.E_LCP_NO_TRACK_FOUND : // No track found
_usiNoOfTracks := 0;
_eChosenTrack := SEW_MKC_VTGS.E_ChosenTrack.e_no_value;

SEW_MKC_VTGS.E_NumberLCP.E_LCP_1_TRACK_FOUND : // One Track found
_usiNoOfTracks := 1;
_lrTrackOffset := TO_LREAL(_stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor);
_eChosenTrack := SEW_MKC_VTGS.E_ChosenTrack.e_center_value;

SEW_MKC_VTGS.E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV : // two tracks found, left side
IF ABS(_stInputDataTxPDO1^.iDeviationLCP1 * _lrFactor - _stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor) > _lrCfgOffsetDifferenceThreshold THEN
_usiNoOfTracks := 2;
IF _eSelectedTrack = SEW_MKC_IAGVSensors.E_SelectTrack.Left THEN
// MAX, because for going left the most positive values are needed!!
_lrTrackOffset := MAX(_stInputDataTxPDO1^.iDeviationLCP1 * _lrFactor, _stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor);
_eChosenTrack := SEW_MKC_VTGS.E_ChosenTrack.e_maximum_value;
ELSE // Default
// MIN, because for going elsewhere the most negative values are needed!!
_lrTrackOffset := MIN(_stInputDataTxPDO1^.iDeviationLCP1 * _lrFactor, _stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor);
_eChosenTrack := SEW_MKC_VTGS.E_ChosenTrack.e_minimum_value;
END_IF
ELSE
_usiNoOfTracks := 1;
_lrTrackOffset := TO_LREAL(_stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor);
_eChosenTrack := SEW_MKC_VTGS.E_ChosenTrack.e_center_value;
END_IF

SEW_MKC_VTGS.E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV : // two tracks found, right side
IF ABS(_stInputDataTxPDO1^.iDeviationLCP3 * _lrFactor - _stInputDataTxPDO1^.iDeviationLCP2 * _lrFactor) > _lrCfgOffsetDifferenceThreshold THEN
_usiNoOfTracks := 2;
IF _eSelectedTrack = SEW_MKC_IAGVSensors.E_SelectTrack.Right THEN
// MIN, because for going rigt the most negative values a
     */

    public E_ChosenTrack _eChosenTrack = E_ChosenTrack.e_no_value;
    public int _usiNoOfTracks = 0;
    public double iActTrackOffset(E_NumberLCP eNumberLCP, int iDeviationLCP1, int iDeviationLCP2, int iDeviationLCP3, double lrFactor, E_SelectTrack eSelectedTrack, double lrCfgOffsetDifferenceThreshold)
    {
        double _lrTrackOffset = 0d;
        switch (eNumberLCP & E_NumberLCP.E_LCP_3_TRACK_FOUND_OR_INTERSECTION)
        {
            case E_NumberLCP.E_LCP_NO_TRACK_FOUND:
                _usiNoOfTracks = 0;
                _eChosenTrack = E_ChosenTrack.e_no_value;
                break;
            case E_NumberLCP.E_LCP_1_TRACK_FOUND:
                _usiNoOfTracks = 1;
                _lrTrackOffset = iDeviationLCP2 * lrFactor;
                _eChosenTrack = E_ChosenTrack.e_center_value;
                break;
            case E_NumberLCP.E_LCP_2_TRACK_FOUND_LEFT_DIV:
                if (Math.Abs(iDeviationLCP1 * lrFactor - iDeviationLCP2 * lrFactor) > lrCfgOffsetDifferenceThreshold)
                {
                    _usiNoOfTracks = 2;
                    if (eSelectedTrack == E_SelectTrack.Left)
                    {
                        _lrTrackOffset = Math.Max(iDeviationLCP1 * lrFactor, iDeviationLCP2 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_maximum_value;
                    }
                    else
                    {
                        _lrTrackOffset = Math.Min(iDeviationLCP1 * lrFactor, iDeviationLCP2 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_minimum_value;
                    }
                }
                else
                {
                    _usiNoOfTracks = 1;
                    _lrTrackOffset = iDeviationLCP2 * lrFactor;
                    _eChosenTrack = E_ChosenTrack.e_center_value;
                }
                break;
            case E_NumberLCP.E_LCP_2_TRACK_FOUND_RIGHT_DIV:
                if (Math.Abs(iDeviationLCP3 * lrFactor - iDeviationLCP2 * lrFactor) > lrCfgOffsetDifferenceThreshold)
                {
                    _usiNoOfTracks = 2;
                    if (eSelectedTrack == E_SelectTrack.Left)
                    {
                        _lrTrackOffset = Math.Max(iDeviationLCP3 * lrFactor, iDeviationLCP2 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_maximum_value;
                    }
                    else
                    {
                        _lrTrackOffset = Math.Min(iDeviationLCP3 * lrFactor, iDeviationLCP2 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_minimum_value;
                    }
                }
                else
                {
                    _usiNoOfTracks = 1;
                    _lrTrackOffset = iDeviationLCP2 * lrFactor;
                    _eChosenTrack = E_ChosenTrack.e_center_value;
                }
                break;
            case E_NumberLCP.E_LCP_3_TRACK_FOUND_OR_INTERSECTION:
                if (Math.Abs(iDeviationLCP3 * lrFactor - iDeviationLCP2 * lrFactor) > lrCfgOffsetDifferenceThreshold)
                {
                    _usiNoOfTracks = 3;
                    if (eSelectedTrack == E_SelectTrack.Left)
                    {
                        _lrTrackOffset = Math.Max(iDeviationLCP3 * lrFactor, iDeviationLCP1 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_maximum_value;
                    }
                    else if (eSelectedTrack == E_SelectTrack.Right)
                    {
                        _lrTrackOffset = Math.Max(iDeviationLCP3 * lrFactor, iDeviationLCP1 * lrFactor);
                        _eChosenTrack = E_ChosenTrack.e_maximum_value;
                    }
                    else
                    {
                        _lrTrackOffset = iDeviationLCP2 * lrFactor;
                        _eChosenTrack = E_ChosenTrack.e_minimum_value;
                    }
                }
                else
                {
                    _usiNoOfTracks = 1;
                    _lrTrackOffset = iDeviationLCP2 * lrFactor;
                    _eChosenTrack = E_ChosenTrack.e_center_value;
                }
                break;

        }
        return _lrTrackOffset;
    }

    double FilterTrackOffset;

    //    METHOD PRIVATE M30_LimitMasterFlt : BOOL
    //VAR_INPUT

    //    xEnable : BOOL;
    //	xEmergencyStopOk : BOOL;
    //	vVelocitySetpMaster	: SEW_MKC_IMath2D.stVector;
    //	lrOmegaSetpMaster : LREAL;
    //	lrLimitFactor : LREAL;
    //END_VAR
    //VAR_OUTPUT

    //    vVelocitySetpLtdFlt	: SEW_MKC_IMath2D.stVector;
    //	lrOmegaSetpLtdFlt : LREAL;	
    //	xSetpointZero : BOOL;
    //END_VAR
    //    _lrM30ActualVelocityLimit := ABS(SEL(vVelocitySetpMaster.lrSign.X >= 0, stConfigLimitsUser.lrSpeedLimitBackward, stConfigLimitsUser.lrSpeedLimitForward));

    //lrTargetFactor := 1 / MAX(1/lrLimitFactor, _lrM30AmountMovement / _lrM30ActualVelocityLimit, (ABS(lrOmegaSetpMaster) / stConfigLimitsUser.lrSpeedLimitRotational));

    //_vM30TargetVelocityLtd := SEW_MKC_Math2D.fcVecScalarMult(vVelocitySetpMaster, lrTargetFactor);
    //_lrM30TargetRotationLtd := lrOmegaSetpMaster* lrTargetFactor;


    //    (* Set actual deceleration*)
    //_lrM30ActualAccStepLateral := stConfigLimitsUser.lrStdAccLateral* lrLastCycleDuration;
    //    _lrM30ActualDecStepLateral := SEL(xEmergencyStopOk , stConfigMaster.lrEStopDecLateral, stConfigLimitsUser.lrStdDecLateral) * lrLastCycleDuration;
    //_lrM30ActualAccStepRotational := stConfigLimitsUser.stCfgLimitsIntern.lrStdAccRotational* lrLastCycleDuration;
    //    _lrM30ActualDecStepRotational := SEL(xEmergencyStopOk , stConfigLimitsUser.stCfgLimitsIntern.lrEStopDecRotational, stConfigLimitsUser.stCfgLimitsIntern.lrStdDecRotational) * lrLastCycleDuration;

    //accX:= SEL((_vM30VelocitySetpLtdM.lrSign.X >= 0) XOR(_vM30VelocitySetpLtdM.lrSign.X<_vM30TargetVelocityLtd.lrSign.X),_lrM30ActualAccStepLateral,_lrM30ActualDecStepLateral);
    //accY:= SEL((_vM30VelocitySetpLtdM.lrSign.Y >= 0) XOR(_vM30VelocitySetpLtdM.lrSign.Y<_vM30TargetVelocityLtd.lrSign.Y),_lrM30ActualAccStepLateral,_lrM30ActualDecStepLateral);
    //accO:= SEL((_lrM30OmegaSetpLtdM >= 0) XOR(_lrM30OmegaSetpLtdM<_lrM30TargetRotationLtd),_lrM30ActualAccStepRotational,_lrM30ActualDecStepRotational);

    //dtX := _vM30TargetVelocityLtd.lrSign.X-_vM30VelocitySetpLtdM.lrSign.X;
    //dtY := _vM30TargetVelocityLtd.lrSign.Y-_vM30VelocitySetpLtdM.lrSign.Y;
    //dtO := _lrM30TargetRotationLtd-_lrM30OmegaSetpLtdM;

    //dtXstep := dtX/accX;
    //dtYstep := dtY/accY;
    //dtOstep := dtO/accO;

    //dtMax := MAX(ABS(dtXstep), ABS(dtYstep), ABS(dtOstep));
    //    IF dtMax>1.0 THEN
    //        vVelocitySetpLtd.lrSign.X :=_vM30VelocitySetpLtdM.lrSign.X + (_vM30TargetVelocityLtd.lrSign.X-_vM30VelocitySetpLtdM.lrSign.X)/ABS(dtMax);
    //    vVelocitySetpLtd.lrSign.Y :=_vM30VelocitySetpLtdM.lrSign.Y + (_vM30TargetVelocityLtd.lrSign.Y-_vM30VelocitySetpLtdM.lrSign.Y)/ABS(dtMax);
    //    lrOmegaSetpLtd :=_lrM30OmegaSetpLtdM + (_lrM30TargetRotationLtd -_lrM30OmegaSetpLtdM)/ABS(dtMax);
    //    ELSE
    //        vVelocitySetpLtd :=_vM30TargetVelocityLtd;
    //	lrOmegaSetpLtd := _lrM30TargetRotationLtd;
    //END_IF

    //_vM30VelocitySetpLtdM := vVelocitySetpLtd;
    //_lrM30OmegaSetpLtdM := lrOmegaSetpLtd;

    //(* Apply MAVG filter*)
    //fbMAVG_SetpLtdX(X:= vVelocitySetpLtd.lrSign.X, N:= stConfigLimitsUser.uiStdJerkLateral, RST:= FALSE, Y=> vVelocitySetpLtdFlt.lrSign.X);
    //    fbMAVG_SetpLtdY(X:= vVelocitySetpLtd.lrSign.Y, N:= stConfigLimitsUser.uiStdJerkLateral, RST:= FALSE, Y=> vVelocitySetpLtdFlt.lrSign.Y);
    //    fbMAVG_SetpLtdO(X:= lrOmegaSetpLtd, N:= stConfigLimitsUser.uiStdJerkLateral, RST:= FALSE, Y=> lrOmegaSetpLtdFlt);

    //    vVelocitySetpLtdFlt := SEL(_xSwitch, vVelocitySetpLtdFlt, vVelocitySetpLtd);
    //    lrOmegaSetpLtdFlt := SEL(_xSwitch, lrOmegaSetpLtdFlt, lrOmegaSetpLtd);
    //    xSetpointZero := SEL(SQRT((vVelocitySetpLtdFlt.lrSign.X* vVelocitySetpLtdFlt.lrSign.X) + (vVelocitySetpLtdFlt.lrSign.Y* vVelocitySetpLtdFlt.lrSign.Y)) < stConfigLimitsUser.lrMovementDeadbandThreshold
    //                     AND ABS(lrOmegaSetpLtdFlt) < stConfigLimitsUser.stCfgLimitsIntern.lrMovementDeadbandThresholdRotational, FALSE, TRUE);

    //(* Diag *)
    //_lrActAccMasterX := vVelocitySetpLtdFlt.lrSign.X - _vVelocitySetpLtdFltM.lrSign.X;
    //_lrActAccMasterY := vVelocitySetpLtdFlt.lrSign.Y - _vVelocitySetpLtdFltM.lrSign.Y;
    //_lrActAlphaMaster := lrOmegaSetpLtdFlt - _lrOmegaSetpLtdFltM;

    //_vVelocitySetpLtdFltM := vVelocitySetpLtdFlt;
    //_lrOmegaSetpLtdFltM := lrOmegaSetpLtdFlt;

    static bool _xM30InitLimit = false;
    static Math2d.Vector _vM30VelocitySetpLtdM = new();
    static double _lrM30OmegaSetpLtdM;
    static Math2d.Vector _vM30VelocitySetpLtdMFlt = new();
    static double _lrM30OmegaSetpLtdMFlt;
    static ST_ConfigLimitsUser stConfigLimitsUser = new();
    static ST_ConfigMaster stConfigMaster = new();
    static double lrLastCycleDuration = 0.01d;

    FB_MAVG fbMAVG_X = new(5);
    FB_MAVG fbMAVG_Y = new(5);
    FB_MAVG fbMAVG_w = new(5);
    private bool xSwitch;
    public double _lrActAccMasterX;
    public double _lrActAccMasterY;
    public double _lrActAlphaMaster;
    public void SetLastVal(double[] l, double o, bool xInit = true)
    {
        _vM30VelocitySetpLtdM = new(l[0], l[1]);
        _lrM30OmegaSetpLtdM = o;
        _xM30InitLimit = xInit;
    }
    public bool M30_LimitMasterFlt(bool xEnable, bool xEmergencyStopOK, Math2d.Vector vVelocitySetpMaster, double lrOmegaSetpMaster, double lrLimitFactor, out Math2d.Vector vVelocitySetpLtd, out double lrOmegaSetpLtd, out bool xSetpointZero)
    {
        double SGN(double x) => x >= 0d ? 1d : -1d;

        var (_vVelocitySetpMaster, _lrOmegaSetpMaster) = (vVelocitySetpMaster, lrOmegaSetpMaster);

        var _lrM30ActualVelocityLimit = Math.Abs(vVelocitySetpMaster.x < 0 ? stConfigLimitsUser.lrSpeedLimitBackward : stConfigLimitsUser.lrSpeedLimitForward);
        
        // Faktor der Angeforderten Bewegung (Einheitenlos!)
        var _lrM30AmountMovement = Math.Sqrt(Math.Pow(_vVelocitySetpMaster.Length() / _lrM30ActualVelocityLimit, 2) + Math.Pow(_lrOmegaSetpMaster / stConfigLimitsUser.lrSpeedLimitRotational, 2));

        // Limitierung der Bewegung (Einheitenlos!)
        var lrTargetFactor = 1d / Math.Max(
            1d / lrLimitFactor,
            _lrM30AmountMovement);

        // Zeit der Bewegung [s]
        var _lrM30AmountSetMovement2 = Math.Sqrt(Math.Pow(_vVelocitySetpMaster.Length() / stConfigLimitsUser.lrStdDecLateral, 2) + Math.Pow(_lrOmegaSetpMaster / stConfigLimitsUser.stCfgLimitsIntern.lrStdDecRotational, 2)) * lrTargetFactor;
        var _lrM30AmountActMovement2 = Math.Sqrt(Math.Pow(_vM30VelocitySetpLtdM.Length() / stConfigLimitsUser.lrStdDecLateral, 2) + Math.Pow(_lrM30OmegaSetpLtdM / stConfigLimitsUser.stCfgLimitsIntern.lrStdDecRotational, 2));

        // Richtungsabschätzung durch Vektormultiplikation
        var _lr30MovementDirectionAssessment = SGN((_vM30VelocitySetpLtdM.x * _vVelocitySetpMaster.x + _vM30VelocitySetpLtdM.y * _vVelocitySetpMaster.y) / Math.Pow(stConfigLimitsUser.lrStdDecLateral, 2) + (_lrM30OmegaSetpLtdM * _lrOmegaSetpMaster) / Math.Pow(stConfigLimitsUser.stCfgLimitsIntern.lrStdDecRotational, 2));

        double lrSign = (xEmergencyStopOK && xEnable ? _lr30MovementDirectionAssessment * _lrM30AmountSetMovement2 : 0d) - _lrM30AmountActMovement2;
        var _lrM30AmountActMovement2LH = Math.Max(0d, _lrM30AmountActMovement2 + lrLastCycleDuration * 2 * SGN(lrSign) );
        if (xEmergencyStopOK && xEnable && _lr30MovementDirectionAssessment >= 0d && Math.Abs(_lrM30AmountSetMovement2 - _lrM30AmountActMovement2) < lrLastCycleDuration * 2)
            _lrM30AmountActMovement2LH = _lrM30AmountSetMovement2;

        var _vM30TargetVelocityLtd = _vVelocitySetpMaster.Mult( lrTargetFactor);
        var _lrM30TargetRotationLtd = _lrOmegaSetpMaster * lrTargetFactor;
        if (_lrM30AmountSetMovement2 > lrLastCycleDuration*0.01)
        {
            _vM30TargetVelocityLtd = _vVelocitySetpMaster.Mult(_lr30MovementDirectionAssessment * _lrM30AmountActMovement2LH*lrTargetFactor / _lrM30AmountSetMovement2);
            _lrM30TargetRotationLtd = _lrOmegaSetpMaster * _lr30MovementDirectionAssessment * _lrM30AmountActMovement2LH * lrTargetFactor / _lrM30AmountSetMovement2;
        }
        else if (_lrM30AmountActMovement2 > lrLastCycleDuration)
        {
            _vM30TargetVelocityLtd = _vM30VelocitySetpLtdM.Mult(_lrM30AmountActMovement2LH / _lrM30AmountActMovement2);
            _lrM30TargetRotationLtd = _lrM30OmegaSetpLtdM * _lrM30AmountActMovement2LH / _lrM30AmountActMovement2;
        }
        // ====================== Alter Teil ========================
        if (!_xM30InitLimit)
        {
            _xM30InitLimit = true;
            _vM30VelocitySetpLtdM = _vM30TargetVelocityLtd;
            _lrM30OmegaSetpLtdM = _lrM30TargetRotationLtd;
        }

        var _lrM30ActualAccStepLateral = stConfigLimitsUser.lrStdAccLateral * lrLastCycleDuration;
        var _lrM30ActualDecStepLateral = (!xEmergencyStopOK ? stConfigMaster.lrEStopDecLateral : stConfigLimitsUser.lrStdDecLateral) * lrLastCycleDuration;
        var _lrM30ActualAccStepRotational = stConfigLimitsUser.stCfgLimitsIntern.lrStdAccRotational * lrLastCycleDuration;
        var _lrM30ActualDecStepRotational = (!xEmergencyStopOK ? stConfigLimitsUser.stCfgLimitsIntern.lrEStopDecRotational : stConfigLimitsUser.stCfgLimitsIntern.lrStdDecRotational) * lrLastCycleDuration;

        var accX = _vM30TargetVelocityLtd.x >= 0 ^ _vM30TargetVelocityLtd.x > _vM30VelocitySetpLtdM.x ? _lrM30ActualDecStepLateral : _lrM30ActualAccStepLateral;
        var accY = _vM30TargetVelocityLtd.y >= 0 ^ _vM30TargetVelocityLtd.y > _vM30VelocitySetpLtdM.y ? _lrM30ActualDecStepLateral : _lrM30ActualAccStepLateral;
        var accO = _lrM30TargetRotationLtd >= 0 ^ _lrM30TargetRotationLtd > _lrM30OmegaSetpLtdM ? _lrM30ActualDecStepRotational : _lrM30ActualAccStepRotational;

        var dtX = _vM30TargetVelocityLtd.x - _vM30VelocitySetpLtdM.x;
        var dtY = _vM30TargetVelocityLtd.y - _vM30VelocitySetpLtdM.y;
        var dtO = _lrM30TargetRotationLtd - _lrM30OmegaSetpLtdM;

        var dtXstep = dtX / accX;
        var dtYstep = dtY / accY;
        var dtOstep = dtO / accO;

        Math2d.Vector vVelocitySetpLtdFlt;
        double lrOmegaSetpLtdFlt;

        var dtMax = Math.Sqrt(Math.Pow(dtXstep, 2) + Math.Pow(dtYstep, 2) + Math.Pow(dtOstep, 2));
        if (dtMax > 1.0)
        {
            vVelocitySetpLtd = _vM30VelocitySetpLtdM.Add(_vM30TargetVelocityLtd.Subtract(_vM30VelocitySetpLtdM).Div(Math.Abs(dtMax)));
            lrOmegaSetpLtd = _lrM30OmegaSetpLtdM + (_lrM30TargetRotationLtd - _lrM30OmegaSetpLtdM) / Math.Abs(dtMax);
        }
        else
        {
            vVelocitySetpLtd = _vM30TargetVelocityLtd;
            lrOmegaSetpLtd = _lrM30TargetRotationLtd;
        }

        // Diag
        _lrActAccMasterX = vVelocitySetpLtd.x - _vM30VelocitySetpLtdM.x;
        _lrActAccMasterY = vVelocitySetpLtd.y - _vM30VelocitySetpLtdM.y;
        _lrActAlphaMaster = lrOmegaSetpLtd - _lrM30OmegaSetpLtdM;

        _vM30VelocitySetpLtdM = vVelocitySetpLtd;
        _lrM30OmegaSetpLtdM = lrOmegaSetpLtd;

        vVelocitySetpLtdFlt = new();
        // Apply MAVG filter
        vVelocitySetpLtdFlt.x = fbMAVG_X.Calc(vVelocitySetpLtd.x, stConfigLimitsUser.uiStdJerkLateral, false);
        vVelocitySetpLtdFlt.y = fbMAVG_Y.Calc(vVelocitySetpLtd.y, stConfigLimitsUser.uiStdJerkLateral, false);
        lrOmegaSetpLtdFlt = fbMAVG_w.Calc(lrOmegaSetpLtd, stConfigLimitsUser.uiStdJerkLateral, false);

        vVelocitySetpLtdFlt = !xSwitch ? vVelocitySetpLtdFlt : vVelocitySetpLtd;
        lrOmegaSetpLtdFlt = !xSwitch ? lrOmegaSetpLtdFlt : lrOmegaSetpLtd;

        xSetpointZero = Math.Sqrt(vVelocitySetpLtdFlt.x * vVelocitySetpLtdFlt.x + vVelocitySetpLtdFlt.y * vVelocitySetpLtdFlt.y) < stConfigLimitsUser.lrMovementDeadbandThreshold
                        && Math.Abs(lrOmegaSetpLtdFlt) < stConfigLimitsUser.stCfgLimitsIntern.lrMovementDeadbandThresholdRotational;


        _vM30VelocitySetpLtdMFlt = vVelocitySetpLtdFlt;
        _lrM30OmegaSetpLtdMFlt = lrOmegaSetpLtdFlt;

        return true;
    }

}