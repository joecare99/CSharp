// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-26-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SnakeTiles.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The ViewModel namespace.
/// </summary>
/// <autogeneratedoc />
namespace Snake_Base.Models.Data
{
    /// <summary>
    /// Enum SnakeTiles
    /// </summary>
    public enum SnakeTiles
    {
        /// <summary>
        /// The empty field
        /// </summary>
        Empty, //0
        /// <summary>
        /// The wall
        /// </summary>
        Wall,  //1
        /// <summary>
        /// The apple
        /// </summary>
        Apple, //2
        /// <summary>
        /// The snake head north
        /// </summary>
        SnakeHead_N,//3
        /// <summary>
        /// The snake tail north
        /// </summary>
        SnakeTail_N,//4 
        /// <summary>
        /// The snake body north-south
        /// </summary>
        SnakeBody_NS,//5            
        /// <summary>
        /// The snake head west
        /// </summary>
        SnakeHead_W,
        /// <summary>
        /// The snake head south
        /// </summary>
        SnakeHead_S,
        /// <summary>
        /// The snake head east
        /// </summary>
        SnakeHead_E,
        /// <summary>
        /// The snake tail west
        /// </summary>
        SnakeTail_W,
        /// <summary>
        /// The snake tail south
        /// </summary>
        SnakeTail_S,
        /// <summary>
        /// The snake tail east
        /// </summary>
        SnakeTail_E,
        /// <summary>
        /// The snake body north-west
        /// </summary>
        SnakeBody_NW,
        /// <summary>
        /// The snake body south-west
        /// </summary>
        SnakeBody_SW,
        /// <summary>
        /// The snake body south-east
        /// </summary>
        SnakeBody_SE,
        /// <summary>
        /// The snake body north-east
        /// </summary>
        SnakeBody_NE,
        /// <summary>
        /// The snake body west-east
        /// </summary>
        SnakeBody_WE,
    };

}
