// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 09-22-2024
//
// Last Modified By : Mir
// Last Modified On : 09-22-2024
// ***********************************************************************
// <copyright file="IGenConnects.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace GenInterfaces.Data
{
    public enum EGenConnectionType
    {
        /// <summary>
        /// The parent
        /// </summary>
        Parent,
        /// <summary>
        /// The child
        /// </summary>
        Child,
        /// <summary>
        /// The spouse
        /// </summary>
        Spouse,
        /// <summary>
        /// The sibling
        /// </summary>
        Sibling,
        /// <summary>The classmate</summary>
        Classmate,
        /// <summary>The neighbor</summary>
        Neighbor,
        /// <summary>
        /// The friend
        /// </summary>
        Friend,
        /// <summary>The employee</summary>
        Employee,
        /// <summary>The employer</summary>
        Employer,
        /// <summary>The teacher</summary>
        Teacher,
        /// <summary>The student</summary>
        Student,
        /// <summary>The head of household</summary>
        HeadOfHousehold,
        /// <summary>The member of household</summary>
        MemberOfHousehold,
        /// <summary>The coworker</summary>
        Coworker,
        /// <summary>The witness</summary>
        Witness,
        /// <summary>The executer</summary>
        Executer,
        /// <summary>The godparent</summary>
        GodParent,
        /// <summary>The fosterparent</summary>
        FosterParent,
        /// <summary>
        /// The other
        /// </summary>
        Other,
        ChildFamily,
        ParentFamily
    }
}