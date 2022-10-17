// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="JT.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace JCAMS.Core.Logging
{
    /// <summary>
    /// Enum JT
    /// </summary>
    [Flags]
    public enum ELogTopic : ulong
    {
        /// <summary>
        /// The always
        /// </summary>
        Always = 0uL,
        /// <summary>
        /// The fail
        /// </summary>
        Fail = 1uL,
        /// <summary>
        /// The succ
        /// </summary>
        Succ = 2uL,
        /// <summary>
        /// The initialize
        /// </summary>
        Init = 4uL,
        /// <summary>
        /// The user comm
        /// </summary>
        UserComm = 8uL,
        /// <summary>
        /// The tcpip comm
        /// </summary>
        TCPIPComm = 0x10uL,
        /// <summary>
        /// The FTP comm
        /// </summary>
        FTPComm = 0x20uL,
        /// <summary>
        /// The HTTP comm
        /// </summary>
        HTTPComm = 0x40uL,
        /// <summary>
        /// The pipe comm
        /// </summary>
        PipeComm = 0x80uL,
        /// <summary>
        /// The telegram
        /// </summary>
        Telegram = 0x100uL,
        /// <summary>
        /// The communication service
        /// </summary>
        CommunicationService = 0x200uL,
        /// <summary>
        /// The pipe
        /// </summary>
        Pipe = 0x400uL,
        /// <summary>
        /// The product process
        /// </summary>
        ProdProcess = 0x800uL,
        /// <summary>
        /// The hw initialize
        /// </summary>
        HWInit = 0x1000uL,
        /// <summary>
        /// The hwi face
        /// </summary>
        HWIFace = 0x2000uL,
        /// <summary>
        /// The conf
        /// </summary>
        Conf = 0x4000uL,
        /// <summary>
        /// The command
        /// </summary>
        Command = 0x8000uL,
        /// <summary>
        /// The graphic
        /// </summary>
        Graphic = 0x10000uL,
        /// <summary>
        /// The other comm
        /// </summary>
        OtherComm = 0x20000uL,
        /// <summary>
        /// The product process decisions
        /// </summary>
        ProdProcessDecisions = 0x40000uL,
        /// <summary>
        /// The timestamp comm service
        /// </summary>
        TimestampCommService = 0x80000uL,
        /// <summary>
        /// The maintenance
        /// </summary>
        Maintenance = 0x100000uL,
        /// <summary>
        /// The prisma
        /// </summary>
        Prisma = 0x200000uL,
        /// <summary>
        /// The start up
        /// </summary>
        StartUp = 0x400000uL,
        /// <summary>
        /// The routing
        /// </summary>
        Routing = 0x800000uL,
        /// <summary>
        /// The management control code
        /// </summary>
        ManagementControlCode = 0x1000000uL,
        /// <summary>
        /// The management control interpreter
        /// </summary>
        ManagementControlInterpreter = 0x2000000uL,
        /// <summary>
        /// The handler instruction
        /// </summary>
        HandlerInstruction = 0x4000000uL,
        /// <summary>
        /// The pallet editor
        /// </summary>
        PalletEditor = 0x8000000uL,
        /// <summary>
        /// The SRL editor
        /// </summary>
        SRLEditor = 0x10000000uL,
        /// <summary>
        /// The debug
        /// </summary>
        Debug = 9223372036854775808uL,
        /// <summary>
        /// The nothing
        /// </summary>
        Nothing = 0uL,
        /// <summary>
        /// All
        /// </summary>
        All = ulong.MaxValue
    }
}
