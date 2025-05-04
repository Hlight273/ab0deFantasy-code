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

//        // ״̬�ڵ�����
//        private List<StateNode> stateNodes = new List<StateNode>();
//        private List<TransitionData> transitions = new List<TransitionData>();
//        private Vector2 scrollPosition;
//        private StateNode selectedNode;
//        private TransitionData selectedTransition;
//        private Vector2 dragOffset;
//        private bool isDragging;

//        // ���ʽ�༭��
//        private bool showExpressionEditor;
//        private string expressionInput = "";
//        private Dictionary<string, string> variableBindings = new Dictionary<string, string>();
//        private List<PropertyInfo> availableProperties;
//        private string[] propertyNames;

//        // ��ֵ����
//        private Dictionary<string, float> numericVariables = new Dictionary<string, float>();
//        private string newNumericVarName = "";
//        private float newNumericVarValue = 0f;

//        // �����Ż�
//        private Texture2D gridTexture;
//        private GUIStyle nodeStyle;
//        private GUIStyle labelStyle;
//        private GUIStyle centerLabelStyle;

//        [MenuItem("״̬��/ת���༭��")]
//        public static void ShowWindow()
//        {
//            GetWindow<StateTransitionEditorWindow>("״̬��ת���༭��");
//        }

//        private void OnEnable()
//        {
//            InitializeNodes();
//            LoadAvailableProperties();
//            LoadFromXml();

//            // ��ʼ����ʽ
//            InitializeStyles();

//            // ������������
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
//            // ���������¼�
//            HandleInput();

//            // ����������
//            DrawMainView();

//            // ���Ʊ��ʽ�༭��
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

//            // ���״̬ͼ
//            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
//            DrawToolbar();

//            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
//            DrawGrid();
//            DrawConnections(); // �Ȼ�����
//            DrawNodes();      // �ٻ��ڵ�
//            EditorGUILayout.EndScrollView();

//            EditorGUILayout.EndVertical();

//            // �Ҳ��������
//            DrawInspector();

//            EditorGUILayout.EndHorizontal();
//        }

//        private void DrawToolbar()
//        {
//            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
//            if (GUILayout.Button("��������", EditorStyles.toolbarButton))
//            {
//                SaveToXml();
//            }
//            if (GUILayout.Button("���ɴ���", EditorStyles.toolbarButton))
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
//                        // ����Ƿ���ڷ�������
//                        bool hasReverse = transitions.Any(t =>
//                            t.FromState == transition.ToState &&
//                            t.ToState == transition.FromState);

//                        // �������ߵ������յ�
//                        Vector3 startPos = new Vector3(startNode.Rect.xMax, startNode.Rect.center.y, 0);
//                        Vector3 endPos = new Vector3(endNode.Rect.xMin, endNode.Rect.center.y, 0);

//                        // ������ڷ������ӣ�ʹ�ñ���������
//                        if (hasReverse)
//                        {
//                            // ȷ�����������Ƿ�������
//                            bool isReverse = transition.FromState.ToString().CompareTo(transition.ToState.ToString()) > 0;
//                            float offset = isReverse ? 30f : -30f;

//                            // ������Ƶ�
//                            Vector3 startTangent = startPos + Vector3.right * 50;
//                            Vector3 endTangent = endPos + Vector3.left * 50;
//                            startTangent.y += offset;
//                            endTangent.y += offset;

//                            // ���Ʊ���������
//                            Color lineColor = selectedTransition == transition ? Color.yellow : Color.white;
//                            Handles.color = lineColor;
//                            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, 2f);

//                            // �����ͷλ�ã��������ϵĵ㣩
//                            float t = 0.8f; // ��ͷλ�ã�0-1��
//                            Vector3 arrowPos = GetPointOnBezierCurve(startPos, endPos, startTangent, endTangent, t);
//                            Vector3 arrowDir = GetTangentOnBezierCurve(startPos, endPos, startTangent, endTangent, t).normalized;

//                            // ���Ƽ�ͷ
//                            DrawArrow(arrowPos, arrowDir);

//                            // �������е���������ı�
//                            Vector3 textPos = GetPointOnBezierCurve(startPos, endPos, startTangent, endTangent, 0.5f);
//                            DrawTransitionLabel(transition, textPos);
//                        }
//                        else
//                        {
//                            // ����ֱ��
//                            Color lineColor = selectedTransition == transition ? Color.yellow : Color.white;
//                            Handles.color = lineColor;
//                            Handles.DrawLine(startPos, endPos);

//                            // ���Ƽ�ͷ
//                            Vector3 direction = (endPos - startPos).normalized;
//                            Vector3 arrowTip = endPos - direction * 15f;
//                            DrawArrow(arrowTip, direction);

//                            // ���ߵ��е���������ı�
//                            Vector3 midPoint = (startPos + endPos) * 0.5f;
//                            DrawTransitionLabel(transition, midPoint);
//                        }
//                    }
//                }

//                Handles.EndGUI();
//            }

//            // ������ߵ��
//            HandleConnectionClick();
//        }

//        private void DrawTransitionLabel(TransitionData transition, Vector3 position)
//        {
//            string conditionText = string.IsNullOrEmpty(transition.Expression) ? "������" : transition.DisplayExpression;
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

//                            // �������Ƿ��ڱ��������߸���
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
//            EditorGUILayout.LabelField("ת�������", EditorStyles.boldLabel);

//            if (selectedTransition != null)
//            {
//                EditorGUILayout.Space(10);
//                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

//                PlayerMovementState newFromState = (PlayerMovementState)EditorGUILayout.EnumPopup("��ʼ״̬", selectedTransition.FromState);
//                if (newFromState != selectedTransition.FromState)
//                {
//                    selectedTransition.FromState = newFromState;
//                    Repaint();
//                }

//                PlayerMovementState newToState = (PlayerMovementState)EditorGUILayout.EnumPopup("Ŀ��״̬", selectedTransition.ToState);
//                if (newToState != selectedTransition.ToState)
//                {
//                    selectedTransition.ToState = newToState;
//                    Repaint();
//                }

//                EditorGUILayout.Space(5);
//                EditorGUILayout.LabelField("�������ʽ", EditorStyles.boldLabel);
//                EditorGUILayout.LabelField("��ǰ���ʽ��", selectedTransition.DisplayExpression ?? "������");

//                if (GUILayout.Button(showExpressionEditor ? "�رձ��ʽ�༭��" : "�򿪱��ʽ�༭��"))
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

//                if (GUILayout.Button("ɾ��ת��"))
//                {
//                    transitions.Remove(selectedTransition);
//                    selectedTransition = null;
//                    Repaint();
//                }

//                EditorGUILayout.EndVertical();
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("ѡ��һ��ת�������༭����", MessageType.Info);
//            }

//            EditorGUILayout.EndVertical();
//        }

//        private void DrawExpressionEditorWindow()
//        {
//            GUILayout.Space(10);
//            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.LabelField("���ʽ�༭��", EditorStyles.boldLabel);
//            EditorGUILayout.HelpBox("���ʽ��ֻ��ʹ�ñ���������ֱ��ʹ�����֡���ʹ���·�����ֵ��������ʾ���֡�", MessageType.Info);

//            // ���ʽ�����
//            GUI.SetNextControlName("ExpressionInput");
//            string newInput = EditorGUILayout.TextArea(expressionInput, GUILayout.Height(50));
//            if (newInput != expressionInput)
//            {
//                expressionInput = newInput;
//                UpdateVariableBindings();
//                Repaint();
//            }

//            // �������б�
//            if (variableBindings.Count > 0)
//            {
//                EditorGUILayout.Space();
//                EditorGUILayout.LabelField("������", EditorStyles.boldLabel);

//                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                foreach (var binding in variableBindings.ToList())
//                {
//                    EditorGUILayout.BeginHorizontal();

//                    // ��������ǩ
//                    EditorGUILayout.LabelField(binding.Key, GUILayout.Width(80));

//                    // ����ѡ���б��������Ժ���ֵѡ��
//                    var options = new List<string>(propertyNames);
//                    options.Add("��ֵ����");

//                    // ȷ����ǰѡ�е�ѡ��
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

//                    // �������������ù̶����
//                    int newIndex = EditorGUILayout.Popup(currentIndex, options.ToArray(), GUILayout.Width(120));

//                    // ����ѡ��仯
//                    if (newIndex != currentIndex)
//                    {
//                        if (newIndex == options.Count - 1)
//                        {
//                            // ѡ������ֵ����
//                            if (!numericVariables.ContainsKey(binding.Key))
//                            {
//                                numericVariables[binding.Key] = 0f;
//                                variableBindings.Remove(binding.Key);
//                            }
//                        }
//                        else
//                        {
//                            // ѡ��������
//                            if (numericVariables.ContainsKey(binding.Key))
//                            {
//                                numericVariables.Remove(binding.Key);
//                            }
//                            variableBindings[binding.Key] = propertyNames[newIndex];
//                        }
//                        Repaint();
//                    }

//                    // �������ֵ��������ʾ��ֵ�����
//                    if (numericVariables.ContainsKey(binding.Key))
//                    {
//                        GUILayout.Space(10); // ���һЩ���
//                        EditorGUILayout.LabelField("ֵ:", GUILayout.Width(30));
//                        float newValue = EditorGUILayout.FloatField(numericVariables[binding.Key], GUILayout.Width(60));
//                        if (newValue != numericVariables[binding.Key])
//                        {
//                            numericVariables[binding.Key] = newValue;
//                        }
//                    }

//                    GUILayout.FlexibleSpace(); // ȷ���Ҷ���
//                    EditorGUILayout.EndHorizontal();
//                }
//                EditorGUILayout.EndVertical();
//            }
//            else if (!string.IsNullOrEmpty(expressionInput) && !ContainsOnlyNumericVars())
//            {
//                EditorGUILayout.HelpBox("δ��⵽��Ч��������������������ı��ʽ�����磺speed > minSpeed", MessageType.Info);
//            }

//            // ������ť
//            EditorGUILayout.Space();
//            EditorGUILayout.BeginHorizontal();

//            if (GUILayout.Button("Ӧ��"))
//            {
//                ApplyExpression();
//            }

//            if (GUILayout.Button("ȡ��"))
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
//            // �������а󶨵�ֵ
//            Dictionary<string, string> oldBindings = new Dictionary<string, string>(variableBindings);
//            variableBindings.Clear();

//            // ʹ��������ʽƥ�����������ĸ�����ֺ��»��ߵ���ϣ�
//            var matches = Regex.Matches(expressionInput, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
//            HashSet<string> processedVars = new HashSet<string>();

//            foreach (Match match in matches)
//            {
//                string varName = match.Value;

//                // �����Ѵ���ı������ؼ��ֺ���ֵ����
//                if (processedVars.Contains(varName) ||
//                    IsKeyword(varName) ||
//                    numericVariables.ContainsKey(varName))
//                {
//                    continue;
//                }

//                processedVars.Add(varName);

//                // ���֮ǰ�а󶨣�������
//                if (oldBindings.ContainsKey(varName))
//                {
//                    variableBindings[varName] = oldBindings[varName];
//                }
//                else
//                {
//                    // ����ƥ�������
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
//                        // ���û��ƥ��ģ�ʹ�õ�һ������
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
//                // ��֤���ʽ�Ļ����﷨
//                if (ValidateExpression(expressionInput))
//                {
//                    // ����ԭʼ���ʽ�ͱ�����
//                    selectedTransition.DisplayExpression = expressionInput;
//                    selectedTransition.VariableBindings = new Dictionary<string, string>(variableBindings);
//                    selectedTransition.NumericVariables = new Dictionary<string, float>(numericVariables);

//                    // �������ڴ������ɵı��ʽ
//                    selectedTransition.Expression = GenerateCodeExpression(expressionInput, variableBindings, numericVariables);

//                    showExpressionEditor = false;
//                    Repaint();
//                }
//                else
//                {
//                    EditorUtility.DisplayDialog("����", "���ʽ�﷨��Ч", "ȷ��");
//                }
//            }
//            catch (Exception ex)
//            {
//                EditorUtility.DisplayDialog("����", $"������ʽʱ����{ex.Message}", "ȷ��");
//            }
//        }

//        private string GenerateCodeExpression(string expression, Dictionary<string, string> bindings, Dictionary<string, float> numVars)
//        {
//            string processedExpression = expression;

//            // �滻��ֵ����
//            foreach (var numVar in numVars)
//            {
//                processedExpression = Regex.Replace(
//                    processedExpression,
//                    $@"\b{Regex.Escape(numVar.Key)}\b",
//                    numVar.Value.ToString());
//            }

//            // �滻������Ϊ������������
//            foreach (var binding in bindings.OrderByDescending(x => x.Key.Length))
//            {
//                // ������ֵ����
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
//            // ���������Ӹ����ӵı��ʽ��֤�߼�
//            // Ŀǰֻ������������ƥ����
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
//            if (e.button == 0) // ���
//            {
//                selectedNode = stateNodes.FirstOrDefault(n => n.Rect.Contains(e.mousePosition));
//                if (selectedNode != null)
//                {
//                    isDragging = true;
//                    dragOffset = e.mousePosition - selectedNode.Rect.position;
//                    e.Use();
//                }
//            }
//            else if (e.button == 1) // �Ҽ�
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
//                    string menuPath = $"���ת����/{targetNode.StateType}";
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
//            // ����Ƿ��Ѵ�����ͬ��ת��
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
//            EditorUtility.DisplayDialog("�ɹ�", "�����ѱ���", "ȷ��");
//        }

//        private void LoadFromXml()
//        {
//            string configPath = Path.Combine(Application.dataPath, "..", "StateMachineConfig.xml");
//            if (!File.Exists(configPath)) return;

//            try
//            {
//                var doc = XDocument.Load(configPath);

//                // ���ؽڵ�λ��
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

//                // ����ת��
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

//                        // ���Լ�����ʾ���ʽ
//                        var displayExpressionElement = transitionElement.Element("DisplayExpression");
//                        if (displayExpressionElement != null)
//                        {
//                            displayExpression = displayExpressionElement.Value;
//                        }

//                        // ����ת������
//                        var transitionData = new TransitionData
//                        {
//                            FromState = fromState,
//                            ToState = toState,
//                            Expression = expression,
//                            DisplayExpression = displayExpression,
//                            VariableBindings = new Dictionary<string, string>(),
//                            NumericVariables = new Dictionary<string, float>()
//                        };

//                        // ���ر�����
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

//                        // ������ֵ����
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
//                Debug.LogError($"����״̬������ʧ�ܣ�{ex.Message}");
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
//                EditorUtility.DisplayDialog("�ɹ�", "�������ɳɹ���", "ȷ��");
//            }
//            catch (Exception ex)
//            {
//                EditorUtility.DisplayDialog("����", $"���ɴ���ʱ����{ex.Message}", "ȷ��");
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