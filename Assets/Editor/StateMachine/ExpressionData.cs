//using System;
//using System.Collections.Generic;

//namespace HFantasy.Editor.StateMachine
//{
//    public enum ExpressionType
//    {
//        Property,   // IsGrounded, IsJumping 等属性
//        Vector2Property, // MoveInput
//        Value,     // 数值
//        Operator,  // 运算符
//        Function,  // 函数调用
//        Group     // 括号组
//    }

//    public enum OperatorType
//    {
//        And,        // &&
//        Or,         // ||
//        Not,        // !
//        Greater,    // >
//        Less,       // <
//        Equal,      // ==
//        NotEqual,   // !=
//        Add,        // +
//        Subtract,   // -
//        Multiply,   // *
//        Divide,     // /
//        Magnitude   // .magnitude
//    }

//    public class ExpressionNode
//    {
//        public ExpressionType Type { get; set; }
//        public string Value { get; set; }
//        public OperatorType? Operator { get; set; }
//        public List<ExpressionNode> Children { get; set; } = new List<ExpressionNode>();
//        public bool IsGroup => Type == ExpressionType.Group;
//    }
//}