//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;
//using System.Xml.Linq;
//using HFantasy.Script.Player.StateMachine;
//using HFantasy.Script.Player.Movement;
//using HFantasy.Script.Configs;
//using System;
//using System.Text.RegularExpressions;
//using UnityEditor.Experimental.GraphView;

//namespace HFantasy.Editor.StateMachine
//{
//    public class StateTransitionEditorWindow : EditorWindow
//    {
//        private const float NODE_WIDTH = 150;
//        private const float NODE_HEIGHT = 50;
//        private const float NODE_SPACING = 200;

//        // 状态节点数据
//        private List<StateNode> stateNodes = new List<StateNode>();
//        private List<TransitionData> transitions = new List<TransitionData>();
//        private Vector2 scrollPosition;
//        private StateNode selectedNode;
//        private TransitionData selectedTransition;
//        private Vector2 dragOffset;
//        private bool isDragging;

//        // 表达式编辑器
//        private bool showExpressionEditor;
//        private string expressionInput = "";
//        private Dictionary<string, string> variableBindings = new Dictionary<string, string>();
//        private List<PropertyInfo> availableProperties;
//        private string[] propertyNames;

//        // 数值变量
//        private Dictionary<string, float> numericVariables = new Dictionary<string, float>();
//        private string newNumericVarName = "";
//        private float newNumericVarValue = 0f;

//        // 性能优化
//        private Texture2D gridTexture;
//        private GUIStyle nodeStyle;
//        private GUIStyle labelStyle;
//        private GUIStyle centerLabelStyle;

//        [MenuItem("状态机/转换编辑器")]
//        public static void ShowWindow()
//        {
//            GetWindow<StateTransitionEditorWindow>("状态机转换编辑器");
//        }

//        private void OnEnable()
//        {
//            InitializeNodes();
//            LoadAvailableProperties();
//            LoadFromXml();

//            // 初始化样式
//            InitializeStyles();

//            // 加载网格纹理
//            gridTexture = EditorGUIUtility.Load("Grid.png") as Texture2D;
//        }

//        private void InitializeStyles()
//        {
//            nodeStyle = new GUIStyle();
//            nodeStyle.normal.background = EditorGUIUtility.Load("node1.png") as Texture2D;
//            if (nodeStyle.normal.background == null)
//            {
//                nodeStyle.normal.background = MakeTexture(20, 20, new Color(0.2f, 0.2f, 0.2f, 0.8f));
//            }

//            labelStyle = new GUIStyle(EditorStyles.label);
//            labelStyle.normal.textColor = Color.white;

//            centerLabelStyle = new GUIStyle(EditorStyles.label);
//            centerLabelStyle.alignment = TextAnchor.MiddleCenter;
//            centerLabelStyle.normal.textColor = Color.white;
//        }

//        private Texture2D MakeTexture(int width, int height, Color color)
//        {
//            Color[] pixels = new Color[width * height];
//            for (int i = 0; i < pixels.Length; i++)
//            {
//                pixels[i] = color;
//            }

//            Texture2D texture = new Texture2D(width, height);
//            texture.SetPixels(pixels);
//            texture.Apply();
//            return texture;
//        }

//        private void InitializeNodes()
//        {
//            stateNodes.Clear();
//            var values = System.Enum.GetValues(typeof(PlayerMovementState));
//            float x = 50;
//            float y = 50;

//            foreach (PlayerMovementState state in values)
//            {
//                stateNodes.Add(new StateNode
//                {
//                    StateType = state,
//                    Rect = new Rect(x, y, NODE_WIDTH, NODE_HEIGHT)
//                });
//                x += NODE_SPACING;
//            }
//        }

//        private void LoadAvailableProperties()
//        {
//            availableProperties = typeof(ICharactorMovement).GetProperties()
//                .Where(p => p.CanRead)
//                .ToList();

//            propertyNames = availableProperties.Select(p => p.Name).ToArray();
//        }

//        private void OnGUI()
//        {
//            // 处理输入事件
//            HandleInput();

//            // 绘制主界面
//            DrawMainView();

//            // 绘制表达式编辑器
//            if (showExpressionEditor && selectedTransition != null)
//            {
//                DrawExpressionEditorWindow();
//            }
//        }

//        private void HandleInput()
//        {
//            var e = Event.current;

//            if (e.type == EventType.MouseDown)
//            {
//                HandleMouseDown(e);
//            }
//            else if (e.type == EventType.MouseDrag)
//            {
//                HandleMouseDrag(e);
//            }
//            else if (e.type == EventType.MouseUp)
//            {
//                HandleMouseUp(e);
//            }
//        }

//        private void DrawMainView()
//        {
//            EditorGUILayout.BeginHorizontal();

//            // 左侧状态图
//            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
//            DrawToolbar();

//            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
//            DrawGrid();
//            DrawConnections(); // 先画连线
//            DrawNodes();      // 再画节点
//            EditorGUILayout.EndScrollView();

//            EditorGUILayout.EndVertical();

//            // 右侧属性面板
//            DrawInspector();

//            EditorGUILayout.EndHorizontal();
//        }

//        private void DrawToolbar()
//        {
//            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
//            if (GUILayout.Button("保存配置", EditorStyles.toolbarButton))
//            {
//                SaveToXml();
//            }
//            if (GUILayout.Button("生成代码", EditorStyles.toolbarButton))
//            {
//                GenerateCode();
//            }
//            EditorGUILayout.EndHorizontal();
//        }

//        private void DrawGrid()
//        {
//            if (gridTexture != null)
//            {
//                Rect gridRect = GUILayoutUtility.GetRect(position.width, position.height);
//                GUI.DrawTextureWithTexCoords(gridRect, gridTexture,
//                    new Rect(0, 0, gridRect.width / 20, gridRect.height / 20));
//            }
//        }

//        private void DrawConnections()
//        {
//            if (Event.current.type == EventType.Repaint)
//            {
//                Handles.BeginGUI();

//                foreach (var transition in transitions)
//                {
//                    var startNode = stateNodes.Find(n => n.StateType == transition.FromState);
//                    var endNode = stateNodes.Find(n => n.StateType == transition.ToState);

//                    if (startNode != null && endNode != null)
//                    {
//                        // 检查是否存在反向连接
//                        bool hasReverse = transitions.Any(t =>
//                            t.FromState == transition.ToState &&
//                            t.ToState == transition.FromState);

//                        // 计算连线的起点和终点
//                        Vector3 startPos = new Vector3(startNode.Rect.xMax, startNode.Rect.center.y, 0);
//                        Vector3 endPos = new Vector3(endNode.Rect.xMin, endNode.Rect.center.y, 0);

//                        // 如果存在反向连接，使用贝塞尔曲线
//                        if (hasReverse)
//                        {
//                            // 确定这是正向还是反向连接
//                            bool isReverse = transition.FromState.ToString().CompareTo(transition.ToState.ToString()) > 0;
//                            float offset = isReverse ? 30f : -30f;

//                            // 计算控制点
//                            Vector3 startTangent = startPos + Vector3.right * 50;
//                            Vector3 endTangent = endPos + Vector3.left * 50;
//                            startTangent.y += offset;
//                            endTangent.y += offset;

//                            // 绘制贝塞尔曲线
//                            Color lineColor = selectedTransition == transition ? Color.yellow : Color.white;
//                            Handles.color = lineColor;
//                            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, 2f);

//                            // 计算箭头位置（在曲线上的点）
//                            float t = 0.8f; // 箭头位置（0-1）
//                            Vector3 arrowPos = GetPointOnBezierCurve(startPos, endPos, startTangent, endTangent, t);
//                            Vector3 arrowDir = GetTangentOnBezierCurve(startPos, endPos, startTangent, endTangent, t).normalized;

//                            // 绘制箭头
//                            DrawArrow(arrowPos, arrowDir);

//                            // 在曲线中点绘制条件文本
//                            Vector3 textPos = GetPointOnBezierCurve(startPos, endPos, startTangent, endTangent, 0.5f);
//                            DrawTransitionLabel(transition, textPos);
//                        }
//                        else
//                        {
//                            // 绘制直线
//                            Color lineColor = selectedTransition == transition ? Color.yellow : Color.white;
//                            Handles.color = lineColor;
//                            Handles.DrawLine(startPos, endPos);

//                            // 绘制箭头
//                            Vector3 direction = (endPos - startPos).normalized;
//                            Vector3 arrowTip = endPos - direction * 15f;
//                            DrawArrow(arrowTip, direction);

//                            // 在线的中点绘制条件文本
//                            Vector3 midPoint = (startPos + endPos) * 0.5f;
//                            DrawTransitionLabel(transition, midPoint);
//                        }
//                    }
//                }

//                Handles.EndGUI();
//            }

//            // 检测连线点击
//            HandleConnectionClick();
//        }

//        private void DrawTransitionLabel(TransitionData transition, Vector3 position)
//        {
//            string conditionText = string.IsNullOrEmpty(transition.Expression) ? "无条件" : transition.DisplayExpression;
//            var content = new GUIContent(conditionText);
//            var textSize = centerLabelStyle.CalcSize(content);
//            var textRect = new Rect(position.x - textSize.x * 0.5f,
//                                  position.y - textSize.y * 0.5f,
//                                  textSize.x,
//                                  textSize.y);

//            EditorGUI.DrawRect(textRect, new Color(0, 0, 0, 0.7f));
//            GUI.Label(textRect, conditionText, centerLabelStyle);
//        }

//        private void HandleConnectionClick()
//        {
//            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
//            {
//                Vector2 mousePos = Event.current.mousePosition;
//                bool foundTransition = false;

//                foreach (var transition in transitions)
//                {
//                    var startNode = stateNodes.Find(n => n.StateType == transition.FromState);
//                    var endNode = stateNodes.Find(n => n.StateType == transition.ToState);

//                    if (startNode != null && endNode != null)
//                    {
//                        Vector3 startPos = new Vector3(startNode.Rect.xMax, startNode.Rect.center.y, 0);
//                        Vector3 endPos = new Vector3(endNode.Rect.xMin, endNode.Rect.center.y, 0);

//                        bool hasReverse = transitions.Any(t =>
//                            t.FromState == transition.ToState &&
//                            t.ToState == transition.FromState);

//                        if (hasReverse)
//                        {
//                            bool isReverse = transition.FromState.ToString().CompareTo(transition.ToState.ToString()) > 0;
//                            float offset = isReverse ? 30f : -30f;

//                            Vector3 startTangent = startPos + Vector3.right * 50;
//                            Vector3 endTangent = endPos + Vector3.left * 50;
//                            startTangent.y += offset;
//                            endTangent.y += offset;

//                            // 检查鼠标是否在贝塞尔曲线附近
//                            for (float t = 0; t <= 1; t += 0.05f)
//                            {
//                                Vector3 point = GetPointOnBezierCurve(startPos, endPos, startTangent, endTangent, t);
//                                if (Vector2.Distance(mousePos, point) < 10f)
//                                {
//                                    selectedTransition = transition;
//                                    foundTransition = true;
//                                    Event.current.Use();
//                                    break;
//                                }
//                            }
//                        }
//                        else if (HandleUtility.DistancePointLine(mousePos, startPos, endPos) < 10f)
//                        {
//                            selectedTransition = transition;
//                            foundTransition = true;
//                            Event.current.Use();
//                        }
//                    }
//                }

//                if (!foundTransition && !stateNodes.Any(n => n.Rect.Contains(mousePos)))
//                {
//                    selectedTransition = null;
//                    Repaint();
//                }
//            }
//        }

//        private Vector3 GetPointOnBezierCurve(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float t)
//        {
//            float u = 1 - t;
//            float tt = t * t;
//            float uu = u * u;
//            float uuu = uu * u;
//            float ttt = tt * t;

//            Vector3 point = uuu * start;
//            point += 3 * uu * t * startTangent;
//            point += 3 * u * tt * endTangent;
//            point += ttt * end;

//            return point;
//        }

//        private Vector3 GetTangentOnBezierCurve(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float t)
//        {
//            float u = 1 - t;
//            float tt = t * t;
//            float uu = u * u;

//            Vector3 tangent = -3 * uu * start;
//            tangent += 3 * (3 * tt - 4 * t + 1) * startTangent;
//            tangent += 3 * (2 * t - 3 * tt) * endTangent;
//            tangent += 3 * tt * end;

//            return tangent;
//        }

//        private void DrawArrow(Vector3 position, Vector3 direction)
//        {
//            float arrowSize = 10f;
//            Vector3 right = new Vector3(-direction.y, direction.x, 0) * arrowSize * 0.5f;
//            Vector3[] arrowPoints = new Vector3[] {
//                position + direction * arrowSize,
//                position + right,
//                position - right
//            };
//            Handles.DrawAAConvexPolygon(arrowPoints);
//        }

//        private void DrawNodes()
//        {
//            foreach (var node in stateNodes)
//            {
//                EditorGUI.DrawRect(node.Rect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
//                GUI.Label(node.Rect, node.StateType.ToString(), centerLabelStyle);
//            }
//        }

//        private void DrawInspector()
//        {
//            EditorGUILayout.BeginVertical(GUILayout.Width(300));
//            EditorGUILayout.LabelField("转换检查器", EditorStyles.boldLabel);

//            if (selectedTransition != null)
//            {
//                EditorGUILayout.Space(10);
//                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

//                PlayerMovementState newFromState = (PlayerMovementState)EditorGUILayout.EnumPopup("起始状态", selectedTransition.FromState);
//                if (newFromState != selectedTransition.FromState)
//                {
//                    selectedTransition.FromState = newFromState;
//                    Repaint();
//                }

//                PlayerMovementState newToState = (PlayerMovementState)EditorGUILayout.EnumPopup("目标状态", selectedTransition.ToState);
//                if (newToState != selectedTransition.ToState)
//                {
//                    selectedTransition.ToState = newToState;
//                    Repaint();
//                }

//                EditorGUILayout.Space(5);
//                EditorGUILayout.LabelField("条件表达式", EditorStyles.boldLabel);
//                EditorGUILayout.LabelField("当前表达式：", selectedTransition.DisplayExpression ?? "无条件");

//                if (GUILayout.Button(showExpressionEditor ? "关闭表达式编辑器" : "打开表达式编辑器"))
//                {
//                    showExpressionEditor = !showExpressionEditor;
//                    if (showExpressionEditor)
//                    {
//                        expressionInput = selectedTransition.DisplayExpression ?? "";
//                        UpdateVariableBindings();
//                        LoadNumericVariables(selectedTransition);
//                    }
//                    Repaint();
//                }

//                if (GUILayout.Button("删除转换"))
//                {
//                    transitions.Remove(selectedTransition);
//                    selectedTransition = null;
//                    Repaint();
//                }

//                EditorGUILayout.EndVertical();
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("选择一个转换线来编辑条件", MessageType.Info);
//            }

//            EditorGUILayout.EndVertical();
//        }

//        private void DrawExpressionEditorWindow()
//        {
//            GUILayout.Space(10);
//            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.LabelField("表达式编辑器", EditorStyles.boldLabel);
//            EditorGUILayout.HelpBox("表达式中只能使用变量，不能直接使用数字。请使用下方的数值变量来表示数字。", MessageType.Info);

//            // 表达式输入框
//            GUI.SetNextControlName("ExpressionInput");
//            string newInput = EditorGUILayout.TextArea(expressionInput, GUILayout.Height(50));
//            if (newInput != expressionInput)
//            {
//                expressionInput = newInput;
//                UpdateVariableBindings();
//                Repaint();
//            }

//            // 变量绑定列表
//            if (variableBindings.Count > 0)
//            {
//                EditorGUILayout.Space();
//                EditorGUILayout.LabelField("变量绑定", EditorStyles.boldLabel);

//                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                foreach (var binding in variableBindings.ToList())
//                {
//                    EditorGUILayout.BeginHorizontal();

//                    // 变量名标签
//                    EditorGUILayout.LabelField(binding.Key, GUILayout.Width(80));

//                    // 创建选项列表，包括属性和数值选项
//                    var options = new List<string>(propertyNames);
//                    options.Add("数值变量");

//                    // 确定当前选中的选项
//                    int currentIndex = -1;
//                    if (numericVariables.ContainsKey(binding.Key))
//                    {
//                        currentIndex = options.Count - 1;
//                    }
//                    else
//                    {
//                        currentIndex = Array.IndexOf(propertyNames, binding.Value);
//                        if (currentIndex < 0) currentIndex = 0;
//                    }

//                    // 绘制下拉框，设置固定宽度
//                    int newIndex = EditorGUILayout.Popup(currentIndex, options.ToArray(), GUILayout.Width(120));

//                    // 处理选项变化
//                    if (newIndex != currentIndex)
//                    {
//                        if (newIndex == options.Count - 1)
//                        {
//                            // 选择了数值变量
//                            if (!numericVariables.ContainsKey(binding.Key))
//                            {
//                                numericVariables[binding.Key] = 0f;
//                                variableBindings.Remove(binding.Key);
//                            }
//                        }
//                        else
//                        {
//                            // 选择了属性
//                            if (numericVariables.ContainsKey(binding.Key))
//                            {
//                                numericVariables.Remove(binding.Key);
//                            }
//                            variableBindings[binding.Key] = propertyNames[newIndex];
//                        }
//                        Repaint();
//                    }

//                    // 如果是数值变量，显示数值输入框
//                    if (numericVariables.ContainsKey(binding.Key))
//                    {
//                        GUILayout.Space(10); // 添加一些间距
//                        EditorGUILayout.LabelField("值:", GUILayout.Width(30));
//                        float newValue = EditorGUILayout.FloatField(numericVariables[binding.Key], GUILayout.Width(60));
//                        if (newValue != numericVariables[binding.Key])
//                        {
//                            numericVariables[binding.Key] = newValue;
//                        }
//                    }

//                    GUILayout.FlexibleSpace(); // 确保右对齐
//                    EditorGUILayout.EndHorizontal();
//                }
//                EditorGUILayout.EndVertical();
//            }
//            else if (!string.IsNullOrEmpty(expressionInput) && !ContainsOnlyNumericVars())
//            {
//                EditorGUILayout.HelpBox("未检测到有效变量。请输入包含变量的表达式，例如：speed > minSpeed", MessageType.Info);
//            }

//            // 操作按钮
//            EditorGUILayout.Space();
//            EditorGUILayout.BeginHorizontal();

//            if (GUILayout.Button("应用"))
//            {
//                ApplyExpression();
//            }

//            if (GUILayout.Button("取消"))
//            {
//                showExpressionEditor = false;
//                Repaint();
//            }

//            EditorGUILayout.EndHorizontal();
//            EditorGUILayout.EndVertical();
//        }

//        private bool ContainsOnlyNumericVars()
//        {
//            var matches = Regex.Matches(expressionInput, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
//            foreach (Match match in matches)
//            {
//                if (!numericVariables.ContainsKey(match.Value) && !IsKeyword(match.Value))
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        private void UpdateVariableBindings()
//        {
//            // 保存现有绑定的值
//            Dictionary<string, string> oldBindings = new Dictionary<string, string>(variableBindings);
//            variableBindings.Clear();

//            // 使用正则表达式匹配变量名（字母、数字和下划线的组合）
//            var matches = Regex.Matches(expressionInput, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
//            HashSet<string> processedVars = new HashSet<string>();

//            foreach (Match match in matches)
//            {
//                string varName = match.Value;

//                // 跳过已处理的变量、关键字和数值变量
//                if (processedVars.Contains(varName) ||
//                    IsKeyword(varName) ||
//                    numericVariables.ContainsKey(varName))
//                {
//                    continue;
//                }

//                processedVars.Add(varName);

//                // 如果之前有绑定，保留它
//                if (oldBindings.ContainsKey(varName))
//                {
//                    variableBindings[varName] = oldBindings[varName];
//                }
//                else
//                {
//                    // 查找匹配的属性
//                    var matchingProps = availableProperties
//                        .Where(p => p.Name.Equals(varName, StringComparison.OrdinalIgnoreCase) ||
//                                   p.Name.StartsWith(varName, StringComparison.OrdinalIgnoreCase))
//                        .ToList();

//                    if (matchingProps.Any())
//                    {
//                        variableBindings[varName] = matchingProps[0].Name;
//                    }
//                    else if (availableProperties.Any())
//                    {
//                        // 如果没有匹配的，使用第一个属性
//                        variableBindings[varName] = availableProperties[0].Name;
//                    }
//                }
//            }
//        }

//        private bool IsKeyword(string text)
//        {
//            string[] keywords = {
//                "and", "or", "not", "true", "false", "if", "else", "return",
//                "null", "new", "this", "base", "void", "int", "float", "bool",
//                "string", "var", "object", "class", "struct", "enum", "interface"
//            };
//            return keywords.Contains(text.ToLower());
//        }

//        private void ApplyExpression()
//        {
//            if (selectedTransition == null) return;

//            try
//            {
//                // 验证表达式的基本语法
//                if (ValidateExpression(expressionInput))
//                {
//                    // 保存原始表达式和变量绑定
//                    selectedTransition.DisplayExpression = expressionInput;
//                    selectedTransition.VariableBindings = new Dictionary<string, string>(variableBindings);
//                    selectedTransition.NumericVariables = new Dictionary<string, float>(numericVariables);

//                    // 生成用于代码生成的表达式
//                    selectedTransition.Expression = GenerateCodeExpression(expressionInput, variableBindings, numericVariables);

//                    showExpressionEditor = false;
//                    Repaint();
//                }
//                else
//                {
//                    EditorUtility.DisplayDialog("错误", "表达式语法无效", "确定");
//                }
//            }
//            catch (Exception ex)
//            {
//                EditorUtility.DisplayDialog("错误", $"处理表达式时出错：{ex.Message}", "确定");
//            }
//        }

//        private string GenerateCodeExpression(string expression, Dictionary<string, string> bindings, Dictionary<string, float> numVars)
//        {
//            string processedExpression = expression;

//            // 替换数值变量
//            foreach (var numVar in numVars)
//            {
//                processedExpression = Regex.Replace(
//                    processedExpression,
//                    $@"\b{Regex.Escape(numVar.Key)}\b",
//                    numVar.Value.ToString());
//            }

//            // 替换变量名为完整的属性名
//            foreach (var binding in bindings.OrderByDescending(x => x.Key.Length))
//            {
//                // 跳过数值变量
//                if (numVars.ContainsKey(binding.Key))
//                    continue;

//                processedExpression = Regex.Replace(
//                    processedExpression,
//                    $@"\b{Regex.Escape(binding.Key)}\b",
//                    $"m.{binding.Value}");
//            }

//            return processedExpression;
//        }

//        private void LoadNumericVariables(TransitionData transition)
//        {
//            numericVariables.Clear();
//            if (transition.NumericVariables != null)
//            {
//                foreach (var pair in transition.NumericVariables)
//                {
//                    numericVariables[pair.Key] = pair.Value;
//                }
//            }
//        }

//        private bool ValidateExpression(string expression)
//        {
//            // 这里可以添加更复杂的表达式验证逻辑
//            // 目前只做基本的括号匹配检查
//            int bracketCount = 0;
//            foreach (char c in expression)
//            {
//                if (c == '(') bracketCount++;
//                else if (c == ')') bracketCount--;

//                if (bracketCount < 0) return false;
//            }
//            return bracketCount == 0;
//        }

//        private void HandleMouseDown(Event e)
//        {
//            if (e.button == 0) // 左键
//            {
//                selectedNode = stateNodes.FirstOrDefault(n => n.Rect.Contains(e.mousePosition));
//                if (selectedNode != null)
//                {
//                    isDragging = true;
//                    dragOffset = e.mousePosition - selectedNode.Rect.position;
//                    e.Use();
//                }
//            }
//            else if (e.button == 1) // 右键
//            {
//                var clickedNode = stateNodes.FirstOrDefault(n => n.Rect.Contains(e.mousePosition));
//                if (clickedNode != null)
//                {
//                    ShowContextMenu(clickedNode);
//                    e.Use();
//                }
//            }
//        }

//        private void HandleMouseDrag(Event e)
//        {
//            if (isDragging && selectedNode != null)
//            {
//                selectedNode.Rect.position = e.mousePosition - dragOffset;
//                Repaint();
//                e.Use();
//            }
//        }

//        private void HandleMouseUp(Event e)
//        {
//            if (e.button == 0)
//            {
//                isDragging = false;
//            }
//        }

//        private void ShowContextMenu(StateNode node)
//        {
//            var menu = new GenericMenu();

//            foreach (var targetNode in stateNodes)
//            {
//                if (targetNode != node)
//                {
//                    string menuPath = $"添加转换到/{targetNode.StateType}";
//                    menu.AddItem(new GUIContent(menuPath), false, () =>
//                    {
//                        AddTransition(node.StateType, targetNode.StateType);
//                        Repaint();
//                    });
//                }
//            }

//            menu.ShowAsContext();
//        }

//        private void AddTransition(PlayerMovementState from, PlayerMovementState to)
//        {
//            // 检查是否已存在相同的转换
//            bool exists = transitions.Any(t => t.FromState == from && t.ToState == to);
//            if (!exists)
//            {
//                var newTransition = new TransitionData
//                {
//                    FromState = from,
//                    ToState = to,
//                    Expression = null,
//                    DisplayExpression = null,
//                    VariableBindings = new Dictionary<string, string>(),
//                    NumericVariables = new Dictionary<string, float>()
//                };

//                transitions.Add(newTransition);
//                selectedTransition = newTransition;
//            }
//        }

//        private void SaveToXml()
//        {
//            var doc = new XDocument(
//                new XElement("StateMachine",
//                    new XElement("Nodes",
//                        stateNodes.Select(n =>
//                            new XElement("Node",
//                                new XElement("StateType", n.StateType),
//                                new XElement("X", n.Rect.x),
//                                new XElement("Y", n.Rect.y)
//                            )
//                        )
//                    ),
//                    new XElement("Transitions",
//                        transitions.Select(t =>
//                            new XElement("Transition",
//                                new XElement("FromState", t.FromState),
//                                new XElement("ToState", t.ToState),
//                                new XElement("Expression", t.Expression ?? ""),
//                                new XElement("DisplayExpression", t.DisplayExpression ?? ""),
//                                new XElement("VariableBindings",
//                                    t.VariableBindings?.Select(vb =>
//                                        new XElement("Binding",
//                                            new XElement("VarName", vb.Key),
//                                            new XElement("PropertyName", vb.Value)
//                                        )
//                                    ) ?? Enumerable.Empty<XElement>()
//                                ),
//                                new XElement("NumericVariables",
//                                    t.NumericVariables?.Select(nv =>
//                                        new XElement("NumVar",
//                                            new XElement("VarName", nv.Key),
//                                            new XElement("Value", nv.Value)
//                                        )
//                                    ) ?? Enumerable.Empty<XElement>()
//                                )
//                            )
//                        )
//                    )
//                )
//            );

//            string configPath = Path.Combine(Application.dataPath, "..", "StateMachineConfig.xml");
//            doc.Save(configPath);
//            AssetDatabase.Refresh();
//            EditorUtility.DisplayDialog("成功", "配置已保存", "确定");
//        }

//        private void LoadFromXml()
//        {
//            string configPath = Path.Combine(Application.dataPath, "..", "StateMachineConfig.xml");
//            if (!File.Exists(configPath)) return;

//            try
//            {
//                var doc = XDocument.Load(configPath);

//                // 加载节点位置
//                var nodesElement = doc.Root.Element("Nodes");
//                if (nodesElement != null)
//                {
//                    foreach (var nodeElement in nodesElement.Elements("Node"))
//                    {
//                        PlayerMovementState stateType = (PlayerMovementState)Enum.Parse(
//                            typeof(PlayerMovementState), nodeElement.Element("StateType").Value);

//                        float x = float.Parse(nodeElement.Element("X").Value);
//                        float y = float.Parse(nodeElement.Element("Y").Value);

//                        var node = stateNodes.FirstOrDefault(n => n.StateType == stateType);
//                        if (node != null)
//                        {
//                            node.Rect.x = x;
//                            node.Rect.y = y;
//                        }
//                    }
//                }

//                // 加载转换
//                transitions.Clear();
//                var transitionsElement = doc.Root.Element("Transitions");
//                if (transitionsElement != null)
//                {
//                    foreach (var transitionElement in transitionsElement.Elements("Transition"))
//                    {
//                        PlayerMovementState fromState = (PlayerMovementState)Enum.Parse(typeof(PlayerMovementState),
//                            transitionElement.Element("FromState").Value);
//                        PlayerMovementState toState = (PlayerMovementState)Enum.Parse(typeof(PlayerMovementState),
//                            transitionElement.Element("ToState").Value);

//                        string expression = transitionElement.Element("Expression")?.Value ?? "";
//                        string displayExpression = expression;

//                        // 尝试加载显示表达式
//                        var displayExpressionElement = transitionElement.Element("DisplayExpression");
//                        if (displayExpressionElement != null)
//                        {
//                            displayExpression = displayExpressionElement.Value;
//                        }

//                        // 创建转换数据
//                        var transitionData = new TransitionData
//                        {
//                            FromState = fromState,
//                            ToState = toState,
//                            Expression = expression,
//                            DisplayExpression = displayExpression,
//                            VariableBindings = new Dictionary<string, string>(),
//                            NumericVariables = new Dictionary<string, float>()
//                        };

//                        // 加载变量绑定
//                        var bindingsElement = transitionElement.Element("VariableBindings");
//                        if (bindingsElement != null)
//                        {
//                            foreach (var bindingElement in bindingsElement.Elements("Binding"))
//                            {
//                                string varName = bindingElement.Element("VarName").Value;
//                                string propName = bindingElement.Element("PropertyName").Value;
//                                transitionData.VariableBindings[varName] = propName;
//                            }
//                        }

//                        // 加载数值变量
//                        var numVarsElement = transitionElement.Element("NumericVariables");
//                        if (numVarsElement != null)
//                        {
//                            foreach (var numVarElement in numVarsElement.Elements("NumVar"))
//                            {
//                                string varName = numVarElement.Element("VarName").Value;
//                                float value = float.Parse(numVarElement.Element("Value").Value);
//                                transitionData.NumericVariables[varName] = value;
//                            }
//                        }

//                        transitions.Add(transitionData);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError($"加载状态机配置失败：{ex.Message}");
//            }
//        }

//        private void GenerateCode()
//        {
//            var sb = new StringBuilder();
//            sb.AppendLine("        private void InitializeTransitions()");
//            sb.AppendLine("        {");

//            foreach (var transition in transitions)
//            {
//                if (!string.IsNullOrEmpty(transition.Expression))
//                {
//                    sb.AppendLine($"            AddTransition(PlayerMovementState.{transition.FromState}, PlayerMovementState.{transition.ToState},");
//                    sb.AppendLine($"                m => {transition.Expression});");
//                }
//                else
//                {
//                    sb.AppendLine($"            AddTransition(PlayerMovementState.{transition.FromState}, PlayerMovementState.{transition.ToState});");
//                }
//                sb.AppendLine();
//            }

//            sb.AppendLine("        }");

//            string code = sb.ToString();
//            string filePath = PathConfig.PlayerSMLogicPath;

//            try
//            {
//                ReplaceMethodInFile(filePath, "InitializeTransitions", code);
//                AssetDatabase.Refresh();
//                EditorUtility.DisplayDialog("成功", "代码生成成功！", "确定");
//            }
//            catch (Exception ex)
//            {
//                EditorUtility.DisplayDialog("错误", $"生成代码时出错：{ex.Message}", "确定");
//            }
//        }

//        private void ReplaceMethodInFile(string filePath, string methodName, string newCode)
//        {
//            string[] lines = File.ReadAllLines(filePath);
//            var sb = new StringBuilder();
//            bool isInMethod = false;
//            int braceCount = 0;

//            foreach (string line in lines)
//            {
//                if (line.Contains($"void {methodName}"))
//                {
//                    isInMethod = true;
//                    sb.AppendLine(newCode);
//                    continue;
//                }

//                if (isInMethod)
//                {
//                    if (line.Contains("{")) braceCount++;
//                    if (line.Contains("}")) braceCount--;
//                    if (braceCount < 0) isInMethod = false;
//                }

//                if (!isInMethod)
//                {
//                    sb.AppendLine(line);
//                }
//            }

//            File.WriteAllText(filePath, sb.ToString());
//        }
//    }

//    public class StateNode
//    {
//        public PlayerMovementState StateType;
//        public Rect Rect;
//    }

//    public class TransitionData
//    {
//        public PlayerMovementState FromState { get; set; }
//        public PlayerMovementState ToState { get; set; }
//        public string Expression { get; set; }
//        public string DisplayExpression { get; set; }
//        public Dictionary<string, string> VariableBindings { get; set; }
//        public Dictionary<string, float> NumericVariables { get; set; }
//    }
//}