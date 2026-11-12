using System.IO;
using UnityEditor;
using UnityEngine;

public class FluidEmitterWindow : EditorWindow
{
	private enum EmitterKind
	{
		Continuous,
		Explosion
	}

	private enum VariationMode
	{
		None,
		Rotate,
		Noise
	}

	private const int PreviewSize = 280;
	private const string SpritePath = "Assets/Fluid/SoftCircle.png";

	private static readonly Color FireColor = new Color(1f, 0.34f, 0.06f, 1f);
	private static readonly Color PoisonColor = new Color(0.2f, 0.9f, 0.28f, 1f);
	private static readonly Color FrostColor = new Color(0.35f, 0.62f, 1f, 1f);

	private struct EffectTemplate
	{
		public string name;
		public string source;
		public EmitterKind kind;
		public Vector3 reaction;
		public float temperature;
		public float velocityX;
		public float velocityY;
		public float outwardsForce;
		public float noiseForce;
		public float rotationalForce;
		public bool inheritVelocity;
		public float emitterVelocityScaling;
		public int renderFrames;
		public VariationMode variation;
		public float variationIntensity;
		public float variationSpeed;
		public float scale;
	}

	private static readonly EffectTemplate[] Templates =
	{
		T("火球", "FireBall.prefab", EmitterKind.Continuous, new Vector3(0.25f, 0f, 0f), 1f, 0f, 0f, 0f, 0.07f, 0f, true, 0.2f, 3, VariationMode.None, 1f, 1f, 1f),
		T("火球环绕", "FireOrb.prefab", EmitterKind.Continuous, new Vector3(0.25f, 0f, 0f), 1f, 0f, 0f, 0f, 0.07f, 0f, true, 0.2f, 3, VariationMode.Rotate, 1f, 16f, 1f),
		T("火球爆炸", "FireballExplosion.prefab", EmitterKind.Explosion, new Vector3(0.5f, 0f, 0f), 1f, 0f, 0f, 32f, 2f, 0f, true, 8f, 3, VariationMode.None, 1f, 1f, 2.2f),
		T("地城爆炸", "Underworld 场景", EmitterKind.Explosion, new Vector3(0.5f, 0f, 0f), 1f, 0f, 0f, 32f, 2f, 1f, true, 8f, 4, VariationMode.None, 1f, 1f, 2.2f),
		T("冲击波", "JumpImpactExplosion.prefab", EmitterKind.Explosion, Vector3.zero, 0.5f, 0f, 0f, 4f, 1f, 0f, true, 8f, 8, VariationMode.None, 1f, 1f, 1.6f),
		T("落步", "StoneGolemWalkImpact.prefab", EmitterKind.Explosion, Vector3.zero, 0.5f, 0f, 0f, 1f, 0.5f, 0f, true, 8f, 8, VariationMode.None, 1f, 1f, 1.2f),
		T("闪电", "LightningStrikes.prefab", EmitterKind.Continuous, new Vector3(0.2f, 0f, 0f), 0.1f, 0f, 0f, 2f, 1f, 0.5f, false, 1f, 3, VariationMode.None, 1f, 1f, 1.4f),
		T("火环", "RingOfFire.prefab，持续时噪声收到 0.208", EmitterKind.Continuous, new Vector3(0.22f, 0f, 0f), 0.04f, 0f, 0f, -0.04f, 0.208f, 0.2f, false, 0.5f, 3, VariationMode.None, 1f, 1f, 8f),
		T("喷射", "SummonFlame.prefab，喷出时", EmitterKind.Continuous, new Vector3(0.266f, 0f, 0f), 0.14f, 0f, 0.3f, 0f, 0f, 0f, false, 0.5f, 3, VariationMode.None, 1f, 1f, 3f),
		T("射线", "RayInstance.prefab，束流开启时", EmitterKind.Continuous, new Vector3(0.25f, 0f, 0f), 0.3f, 0f, 0f, 0f, 0.5f, 0f, false, 0f, 3, VariationMode.None, 1f, 1f, 1.2f),
		T("油渍", "OilPuddle.prefab", EmitterKind.Continuous, new Vector3(0.15f, 0f, 0f), 0.01f, 0f, 0f, 0f, 0.03f, 0f, true, 0.5f, 3, VariationMode.None, 1f, 1f, 2.4f),
		T("毒油", "PoisonOilPuddle.prefab", EmitterKind.Continuous, new Vector3(0f, 0.15f, 0f), 0.01f, 0f, 0f, 0f, 0.03f, 0f, true, 0.5f, 3, VariationMode.None, 1f, 1f, 2.4f),
		T("燃烧木", "TreePart1.prefab", EmitterKind.Continuous, new Vector3(0.5f, 0f, 0f), 0.2f, 0f, 0f, 0f, 0.5f, 0f, true, 0.5f, 3, VariationMode.None, 1f, 1f, 1.6f),
		T("菜单火苗", "MainMenu.unity", EmitterKind.Continuous, new Vector3(0.1f, 0f, 0f), 0.01f, 0f, 0f, 0.2f, 0.2f, 0f, true, 0.5f, 3, VariationMode.None, 1f, 1f, 1.6f),
		T("护盾气流", "ShieldAura.prefab", EmitterKind.Continuous, Vector3.zero, 0f, 0f, 0f, 0f, 0.02f, 0.5f, false, 0.5f, 3, VariationMode.None, 1f, 1f, 2.4f),
		T("爪击拖尾", "Claw.prefab", EmitterKind.Continuous, Vector3.zero, 1f, 0f, 0f, 0f, 0f, 0f, true, 64f, 3, VariationMode.None, 1f, 1f, 0.9f),
		T("旋风爪", "WhirlwindClaw.prefab", EmitterKind.Continuous, Vector3.zero, 1f, 0f, 0f, 0f, 0f, 0f, true, 32f, 3, VariationMode.None, 1f, 1f, 1f)
	};

	private FluidSim sim;
	private Sprite sprite;
	private int templateIndex;
	private EmitterKind kind = EmitterKind.Continuous;
	private Vector3 reaction = new Vector3(0.25f, 0f, 0f);
	private float temperature = 1f;
	private float velocityX;
	private float velocityY;
	private float outwardsForce;
	private float noiseForce = 0.07f;
	private float rotationalForce;
	private bool inheritVelocity = true;
	private float emitterVelocityScaling = 0.2f;
	private int renderFrames = 3;
	private VariationMode variation = VariationMode.None;
	private float variationIntensity = 1f;
	private float variationSpeed = 1f;
	private bool followMouse;
	private float mouseDistance = 2f;
	private float scaleX = 1f;
	private float scaleY = 1f;
	private int spawnCount = 1;
	private float spread = 0.75f;
	private bool placeArmed;
	private bool showDetails;
	private Vector2 scroll;
	private FluidEmitter previewEmitter;
	private RenderTexture viewTexture;
	private Material previewMaterial;
	private double nextStep;
	private int appliedHash = int.MinValue;
	private bool triedSandbox;
	private bool extrasDirty = true;
	private string previewError;
	private Texture2D preview;
	private int previewHash = int.MinValue;

	[MenuItem("Tools/流体/可视化发射器")]
	public static void Open()
	{
		FluidEmitterWindow window = GetWindow<FluidEmitterWindow>("流体发射器");
		window.minSize = new Vector2(860f, 640f);
	}

	[MenuItem("Tools/流体/创建沙盒")]
	public static void CreateSandbox()
	{
		FluidSim existing = Object.FindObjectOfType<FluidSim>();
		if (existing != null)
		{
			Selection.activeGameObject = existing.gameObject;
			EditorGUIUtility.PingObject(existing.gameObject);
			return;
		}

		Shader renderShader = Shader.Find("2DFluidSim/FluidRender3D");
		if (renderShader == null)
		{
			EditorUtility.DisplayDialog("流体沙盒", "还没找到 2DFluidSim/FluidRender3D。等脚本和着色器编译完再试一次。", "确定");
			return;
		}

		EnsureSprite();
		Directory.CreateDirectory("Assets/Fluid");
		Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fluid/FluidSandboxRender.mat");
		if (material == null)
		{
			material = new Material(renderShader);
			AssetDatabase.CreateAsset(material, "Assets/Fluid/FluidSandboxRender.mat");
		}
		else
		{
			material.shader = renderShader;
		}

		GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Undo.RegisterCreatedObjectUndo(quad, "创建流体沙盒");
		quad.name = "FluidSim";
		quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		quad.transform.localScale = new Vector3(24f, 24f, 1f);
		Collider collider = quad.GetComponent<Collider>();
		if (collider != null)
		{
			Undo.DestroyObjectImmediate(collider);
		}
		quad.GetComponent<MeshRenderer>().sharedMaterial = material;

		FluidSim fluid = Undo.AddComponent<FluidSim>(quad);
		fluid.enableParticles = false;
		fluid.velocityEmitterScaling = 128f;
		fluid.densityBuoyancy = 1f;
		fluid.gridWidth = 256;
		fluid.gridHeight = 256;

		Camera camera = Camera.main;
		if (camera != null)
		{
			Undo.RecordObject(camera.transform, "对准流体沙盒");
			camera.transform.position = new Vector3(0f, 16f, -12f);
			camera.transform.LookAt(Vector3.zero, Vector3.up);
		}

		Selection.activeGameObject = quad;
	}

	private void OnEnable()
	{
		sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
		if (sim == null)
		{
			sim = Object.FindObjectOfType<FluidSim>();
		}
		EditorApplication.update += EditorTick;
		SceneView.duringSceneGui += DuringSceneGui;
	}

	private void OnDisable()
	{
		EditorApplication.update -= EditorTick;
		SceneView.duringSceneGui -= DuringSceneGui;
		if (preview != null)
		{
			DestroyImmediate(preview);
			preview = null;
		}
		if (previewMaterial != null)
		{
			DestroyImmediate(previewMaterial);
			previewMaterial = null;
		}
		if (viewTexture != null)
		{
			viewTexture.Release();
			DestroyImmediate(viewTexture);
			viewTexture = null;
		}
	}

	private void EditorTick()
	{
		if (EditorApplication.isCompiling || EditorApplication.isUpdating)
		{
			return;
		}
		if (EditorApplication.timeSinceStartup < nextStep)
		{
			return;
		}
		nextStep = EditorApplication.timeSinceStartup + 0.05;
		if (!EnsurePreviewRig())
		{
			Repaint();
			return;
		}
		SyncPreviewEmitter();
		if (!EditorApplication.isPlaying)
		{
			try
			{
				sim.StepForEditor();
				previewError = null;
			}
			catch (System.Exception exception)
			{
				previewError = exception.Message;
			}
		}
		RenderView();
		Repaint();
	}

	private bool EnsurePreviewRig()
	{
		if (sim == null)
		{
			sim = Object.FindObjectOfType<FluidSim>();
		}
		if (sim == null && !triedSandbox)
		{
			triedSandbox = true;
			CreateSandbox();
			sim = Object.FindObjectOfType<FluidSim>();
		}
		if (sim == null)
		{
			previewError = "还没有流体平面";
			return false;
		}
		if (sprite == null)
		{
			sprite = EnsureSprite();
		}
		if (previewEmitter == null)
		{
			previewEmitter = sim.GetComponentInChildren<FluidEmitter>();
		}
		if (previewEmitter == null)
		{
			Spawn(sim.transform.position);
			previewEmitter = sim.GetComponentInChildren<FluidEmitter>();
		}
		return previewEmitter != null;
	}

	private void SyncPreviewEmitter()
	{
		if (previewEmitter == null || sim == null)
		{
			return;
		}
		Vector3 lossy = sim.transform.lossyScale;
		float safeX = Mathf.Approximately(lossy.x, 0f) ? 1f : lossy.x;
		float safeY = Mathf.Approximately(lossy.y, 0f) ? 1f : lossy.y;
		Vector3 desiredScale = new Vector3(scaleX / safeX, scaleY / safeY, 1f);
		int hash = ParameterHash();
		if (hash != appliedHash || extrasDirty)
		{
			WriteEmitter(previewEmitter);
			WriteExtras(previewEmitter.gameObject);
			appliedHash = hash;
			extrasDirty = false;
		}
		if (previewEmitter.transform.localScale != desiredScale)
		{
			previewEmitter.transform.localScale = desiredScale;
		}
		previewEmitter.PushProperties();
	}

	private int ParameterHash()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + reaction.GetHashCode();
			hash = hash * 31 + temperature.GetHashCode();
			hash = hash * 31 + velocityX.GetHashCode();
			hash = hash * 31 + velocityY.GetHashCode();
			hash = hash * 31 + outwardsForce.GetHashCode();
			hash = hash * 31 + noiseForce.GetHashCode();
			hash = hash * 31 + rotationalForce.GetHashCode();
			hash = hash * 31 + inheritVelocity.GetHashCode();
			hash = hash * 31 + emitterVelocityScaling.GetHashCode();
			hash = hash * 31 + renderFrames.GetHashCode();
			hash = hash * 31 + variation.GetHashCode();
			hash = hash * 31 + variationIntensity.GetHashCode();
			hash = hash * 31 + variationSpeed.GetHashCode();
			hash = hash * 31 + followMouse.GetHashCode();
			hash = hash * 31 + kind.GetHashCode();
			return hash;
		}
	}

	private void EnsureView()
	{
		if (viewTexture == null)
		{
			viewTexture = new RenderTexture(640, 640, 0, RenderTextureFormat.ARGB32);
			viewTexture.Create();
		}
		if (previewMaterial == null)
		{
			Shader shader = Shader.Find("Hidden/FluidPreview");
			if (shader == null)
			{
				previewError = "流体预览着色器还没编译好";
				return;
			}
			previewMaterial = new Material(shader);
			previewMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void RenderView()
	{
		if (sim == null || sim.GetReaction() == null)
		{
			return;
		}
		EnsureView();
		if (previewMaterial == null)
		{
			return;
		}
		Vector2 center = new Vector2(0.5f, 0.5f);
		if (previewEmitter != null)
		{
			center = sim.WorldToTextureCoords(previewEmitter.transform.position);
		}
		float plane = Mathf.Max(sim.transform.lossyScale.x, 0.001f);
		float uvWindow = Mathf.Clamp(Mathf.Max(scaleX, scaleY, 1f) * 4f / plane, 0.08f, 1f);
		Vector2 offset = center - Vector2.one * (uvWindow * 0.5f);
		previewMaterial.SetTexture("_Reaction", sim.GetReaction());
		previewMaterial.SetTexture("_Density", sim.GetDensity());
		previewMaterial.SetVector("_PreviewST", new Vector4(uvWindow, uvWindow, offset.x, offset.y));
		Graphics.Blit(null, viewTexture, previewMaterial);
	}

	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(340f));
		DrawControls();
		EditorGUILayout.EndScrollView();
		DrawPreviewColumn();
		EditorGUILayout.EndHorizontal();
	}

	private void DrawControls()
	{
		EditorGUILayout.LabelField("参数", EditorStyles.boldLabel);
		int shown = templateIndex < 0 ? 0 : templateIndex + 1;
		int picked = EditorGUILayout.Popup("效果", shown, TemplateLabels());
		if (picked == 0)
		{
			if (templateIndex >= 0 && shown != 0)
			{
				templateIndex = -1;
			}
		}
		else if (picked - 1 != templateIndex)
		{
			templateIndex = picked - 1;
			ApplyTemplate(Templates[templateIndex]);
			extrasDirty = true;
		}

		EditorGUI.BeginChangeCheck();
		reaction.x = DrawChannel("火", reaction.x, FireColor);
		reaction.y = DrawChannel("毒", reaction.y, PoisonColor);
		reaction.z = DrawChannel("霜", reaction.z, FrostColor);
		temperature = EditorGUILayout.Slider("温度", temperature, -1f, 1f);
		float scale = EditorGUILayout.Slider("大小", scaleX, 0.3f, 8f);
		scaleX = scale;
		scaleY = scale;
		outwardsForce = EditorGUILayout.FloatField("向外", outwardsForce);
		noiseForce = EditorGUILayout.FloatField("噪声", noiseForce);
		rotationalForce = EditorGUILayout.FloatField("旋转", rotationalForce);
		if (EditorGUI.EndChangeCheck())
		{
			templateIndex = -1;
		}

		EditorGUILayout.Space();
		showDetails = EditorGUILayout.Foldout(showDetails, "更多", true);
		if (!showDetails)
		{
			return;
		}

		if (GUILayout.Button("创建沙盒"))
		{
			CreateSandbox();
			sim = Object.FindObjectOfType<FluidSim>();
		}
		sim = (FluidSim)EditorGUILayout.ObjectField("流体模拟", sim, typeof(FluidSim), true);
		sprite = (Sprite)EditorGUILayout.ObjectField("发射形状", sprite, typeof(Sprite), false);
		if (sprite == null && GUILayout.Button("生成圆形发射形状"))
		{
			sprite = EnsureSprite();
		}
		bool explosion = EditorGUILayout.Toggle("放完就消失", kind == EmitterKind.Explosion);
		kind = explosion ? EmitterKind.Explosion : EmitterKind.Continuous;
		spawnCount = Mathf.Max(1, EditorGUILayout.IntField("数量", spawnCount));
		if (spawnCount > 1)
		{
			spread = EditorGUILayout.FloatField("散布半径", spread);
		}

		EditorGUI.BeginChangeCheck();
		velocityX = EditorGUILayout.Slider("速度 X", velocityX, -1f, 1f);
		velocityY = EditorGUILayout.Slider("速度 Y", velocityY, -1f, 1f);
		inheritVelocity = EditorGUILayout.Toggle("跟随移动", inheritVelocity);
		emitterVelocityScaling = EditorGUILayout.FloatField("移动带风", emitterVelocityScaling);

		if (kind == EmitterKind.Explosion)
		{
			EditorGUILayout.Space();
			renderFrames = Mathf.Max(1, EditorGUILayout.IntField("爆炸存活步数", renderFrames));
		}

		EditorGUILayout.Space();
		variation = (VariationMode)EditorGUILayout.EnumPopup("速度变化", variation);
		if (variation != VariationMode.None)
		{
			variationIntensity = EditorGUILayout.FloatField("变化强度", variationIntensity);
			variationSpeed = EditorGUILayout.FloatField("变化速度", variationSpeed);
		}
		followMouse = EditorGUILayout.Toggle("跟随鼠标", followMouse);
		if (followMouse)
		{
			mouseDistance = EditorGUILayout.FloatField("到摄像机距离", mouseDistance);
		}
		if (EditorGUI.EndChangeCheck())
		{
			templateIndex = -1;
			extrasDirty = true;
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledScope(sim == null || sprite == null))
		{
			if (GUILayout.Button("在模拟中心生成"))
			{
				Spawn(sim.transform.position);
			}
			placeArmed = GUILayout.Toggle(placeArmed, placeArmed ? "在场景里点击放置（再点取消）" : "在场景里点击放置", "Button");
		}

		using (new EditorGUI.DisabledScope(SelectedEmitter() == null))
		{
			if (GUILayout.Button("从选中读取"))
			{
				LoadFrom(SelectedEmitter());
			}
			if (GUILayout.Button("写回选中"))
			{
				WriteToSelection();
			}
		}
	}

	private static string[] templateLabels;

	private static string[] TemplateLabels()
	{
		if (templateLabels != null && templateLabels.Length == Templates.Length + 1)
		{
			return templateLabels;
		}
		templateLabels = new string[Templates.Length + 1];
		templateLabels[0] = "自定义";
		for (int i = 0; i < Templates.Length; i++)
		{
			templateLabels[i + 1] = Templates[i].name;
		}
		return templateLabels;
	}

	private void ShowEffect()
	{
		if (templateIndex >= 0)
		{
			ApplyTemplate(Templates[templateIndex]);
		}
		spawnCount = 1;
		if (sim == null)
		{
			sim = Object.FindObjectOfType<FluidSim>();
		}
		if (sim == null)
		{
			CreateSandbox();
			sim = Object.FindObjectOfType<FluidSim>();
		}
		if (sim == null)
		{
			return;
		}
		if (sprite == null)
		{
			sprite = EnsureSprite();
		}
		AimCamera();
		ClearEmitters();
		Spawn(sim.transform.position);
		FocusGameView();
		if (!EditorApplication.isPlaying)
		{
			EditorApplication.delayCall += EnterPlay;
		}
	}

	private static void EnterPlay()
	{
		if (!EditorApplication.isPlaying)
		{
			EditorApplication.EnterPlaymode();
		}
	}

	private void AimCamera()
	{
		Camera camera = Camera.main;
		if (camera == null || sim == null)
		{
			return;
		}
		Vector3 target = sim.transform.position;
		if (!EditorApplication.isPlaying)
		{
			Undo.RecordObject(camera.transform, "对准流体");
		}
		camera.transform.position = target + new Vector3(0f, 18f, -10f);
		camera.transform.LookAt(target, Vector3.up);
	}

	private void ClearEmitters()
	{
		if (sim == null)
		{
			return;
		}
		FluidEmitter[] emitters = sim.GetComponentsInChildren<FluidEmitter>();
		for (int i = 0; i < emitters.Length; i++)
		{
			if (emitters[i] == null)
			{
				continue;
			}
			if (EditorApplication.isPlaying)
			{
				Object.Destroy(emitters[i].gameObject);
			}
			else
			{
				Undo.DestroyObjectImmediate(emitters[i].gameObject);
			}
		}
	}

	private static void FocusGameView()
	{
		System.Type gameView = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
		if (gameView != null)
		{
			EditorWindow.GetWindow(gameView);
		}
	}

	private void DrawPreviewColumn()
	{
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		EditorGUILayout.LabelField("流体", EditorStyles.boldLabel);
		Rect viewRect = GUILayoutUtility.GetRect(360f, 360f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		if (viewTexture != null)
		{
			EditorGUI.DrawPreviewTexture(viewRect, viewTexture, null, ScaleMode.ScaleToFit);
		}
		else
		{
			EditorGUI.HelpBox(viewRect, string.IsNullOrEmpty(previewError) ? "正在准备模拟" : previewError, MessageType.None);
		}
		EditorGUILayout.EndVertical();
	}

	private static float DrawChannel(string label, float value, Color color)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(28f));
		Rect bar = GUILayoutUtility.GetRect(18f, 18f, GUILayout.ExpandWidth(true));
		EditorGUI.DrawRect(bar, new Color(0.12f, 0.12f, 0.12f, 1f));
		EditorGUI.DrawRect(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(value), bar.height), color);
		value = EditorGUILayout.Slider(value, 0f, 1f, GUILayout.Width(150f));
		EditorGUILayout.EndHorizontal();
		return value;
	}

	private static void DrawTemperatureBar(float value)
	{
		Rect bar = GUILayoutUtility.GetRect(18f, 16f, GUILayout.ExpandWidth(true));
		EditorGUI.DrawRect(bar, new Color(0.12f, 0.12f, 0.12f, 1f));
		float mid = bar.x + bar.width * 0.5f;
		if (value >= 0f)
		{
			EditorGUI.DrawRect(new Rect(mid, bar.y, bar.width * 0.5f * value, bar.height), new Color(1f, 0.45f, 0.1f, 1f));
		}
		else
		{
			float width = bar.width * 0.5f * -value;
			EditorGUI.DrawRect(new Rect(mid - width, bar.y, width, bar.height), FrostColor);
		}
	}

	private static void DrawSwatch(string label, Color color, float amount)
	{
		EditorGUILayout.BeginHorizontal();
		Rect chip = GUILayoutUtility.GetRect(16f, 16f, GUILayout.Width(16f));
		EditorGUI.DrawRect(chip, Color.Lerp(Color.black, color, Mathf.Clamp01(amount)));
		EditorGUILayout.LabelField(label + "  " + amount.ToString("0.00"), EditorStyles.miniLabel);
		EditorGUILayout.EndHorizontal();
	}

	private void DrawTemplateButtons()
	{
		EditorGUILayout.LabelField("点一个就填入原工程里的那组参数。世界缩放只是沙盒里方便看的大小。", EditorStyles.wordWrappedMiniLabel);
		for (int i = 0; i < Templates.Length; i += 2)
		{
			EditorGUILayout.BeginHorizontal();
			DrawTemplateButton(i);
			if (i + 1 < Templates.Length)
			{
				DrawTemplateButton(i + 1);
			}
			EditorGUILayout.EndHorizontal();
		}
		string source = templateIndex >= 0 ? Templates[templateIndex].source : "当前数值已手改";
		if (templateIndex >= 0 && ReactionChannelDiffers(Templates[templateIndex].reaction))
		{
			source += "，反应通道已改写";
		}
		EditorGUILayout.LabelField("来源  " + source, EditorStyles.miniLabel);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("改写到火"))
		{
			RemapElement(0);
		}
		if (GUILayout.Button("改写到毒"))
		{
			RemapElement(1);
		}
		if (GUILayout.Button("改写到霜"))
		{
			RemapElement(2);
		}
		EditorGUILayout.EndHorizontal();
	}

	private void DrawTemplateButton(int index)
	{
		EffectTemplate template = Templates[index];
		string caption = templateIndex == index ? template.name + "（当前）" : template.name;
		if (GUILayout.Button(caption))
		{
			templateIndex = index;
			ApplyTemplate(template);
		}
		Rect chip = GUILayoutUtility.GetRect(14f, 18f, GUILayout.Width(14f));
		EditorGUI.DrawRect(chip, TemplateColor(template));
	}

	private static Color TemplateColor(EffectTemplate template)
	{
		float amount = template.reaction.x + template.reaction.y + template.reaction.z;
		if (amount > 0.001f)
		{
			return (FireColor * template.reaction.x + PoisonColor * template.reaction.y + FrostColor * template.reaction.z) / amount;
		}
		if (template.temperature > 0.05f)
		{
			return new Color(1f, 0.45f, 0.1f, 1f);
		}
		return new Color(0.7f, 0.75f, 0.8f, 1f);
	}

	private bool ReactionChannelDiffers(Vector3 source)
	{
		return !Mathf.Approximately(reaction.x, source.x) || !Mathf.Approximately(reaction.y, source.y) || !Mathf.Approximately(reaction.z, source.z);
	}

	private void RemapElement(int channel)
	{
		float sum = reaction.x + reaction.y + reaction.z;
		reaction = Vector3.zero;
		if (channel == 0)
		{
			reaction.x = sum;
		}
		else if (channel == 1)
		{
			reaction.y = sum;
		}
		else
		{
			reaction.z = sum;
		}
	}

	private void EnsurePreview()
	{
		int hash = ComputePreviewHash();
		if (preview != null && hash == previewHash)
		{
			return;
		}
		previewHash = hash;
		if (preview == null)
		{
			preview = new Texture2D(PreviewSize, PreviewSize, TextureFormat.RGBA32, false);
			preview.hideFlags = HideFlags.HideAndDontSave;
			preview.wrapMode = TextureWrapMode.Clamp;
			preview.filterMode = FilterMode.Bilinear;
		}
		PaintPreview();
	}

	private int ComputePreviewHash()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + reaction.GetHashCode();
			hash = hash * 31 + temperature.GetHashCode();
			hash = hash * 31 + velocityX.GetHashCode();
			hash = hash * 31 + velocityY.GetHashCode();
			hash = hash * 31 + outwardsForce.GetHashCode();
			hash = hash * 31 + noiseForce.GetHashCode();
			hash = hash * 31 + rotationalForce.GetHashCode();
			return hash;
		}
	}

	private void PaintPreview()
	{
		Color[] pixels = new Color[PreviewSize * PreviewSize];
		Color background = new Color(0.08f, 0.09f, 0.11f, 1f);
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = background;
		}

		for (int y = 0; y < PreviewSize; y++)
		{
			for (int x = 0; x < PreviewSize; x++)
			{
				Vector2 uv = new Vector2((x + 0.5f) / PreviewSize, (y + 0.5f) / PreviewSize);
				Vector2 fromCenter = uv * 2f - Vector2.one;
				float distance = fromCenter.magnitude;
				float mask = Mathf.Clamp01(1f - distance);
				mask *= mask;
				Color rgb = FireColor * reaction.x + PoisonColor * reaction.y + FrostColor * reaction.z;
				if (temperature >= 0f)
				{
					rgb = Color.Lerp(rgb, new Color(1f, 0.72f, 0.25f, 1f), temperature * 0.45f);
				}
				else
				{
					rgb = Color.Lerp(rgb, FrostColor, -temperature * 0.65f);
				}
				float presence = Mathf.Clamp01(reaction.x + reaction.y + reaction.z + Mathf.Abs(temperature) * 0.35f);
				Color color = Color.Lerp(background, rgb, mask * Mathf.Max(presence, 0.15f));
				pixels[y * PreviewSize + x] = color;
			}
		}

		DrawFieldArrows(pixels);
		preview.SetPixels(pixels);
		preview.Apply(false);
	}

	private void DrawFieldArrows(Color[] pixels)
	{
		Color arrow = new Color(1f, 1f, 1f, 0.9f);
		for (int gy = 1; gy <= 5; gy++)
		{
			for (int gx = 1; gx <= 5; gx++)
			{
				Vector2 uv = new Vector2(gx / 6f, gy / 6f);
				Vector2 fromCenter = uv * 2f - Vector2.one;
				float distance = fromCenter.magnitude;
				if (distance > 0.82f)
				{
					continue;
				}
				float mask = Mathf.Clamp01(1f - distance);
				Vector2 outward = distance > 0.001f ? fromCenter / distance : Vector2.up;
				Vector2 tangent = new Vector2(-outward.y, outward.x);
				float n = Mathf.PerlinNoise(uv.x * 4f, uv.y * 4f) - 0.5f;
				Vector2 velocity = new Vector2(velocityX, velocityY);
				velocity += outward * outwardsForce;
				velocity += tangent * rotationalForce;
				velocity += new Vector2(n, -n) * noiseForce;
				velocity *= mask;
				if (velocity.sqrMagnitude < 0.0001f)
				{
					continue;
				}
				Vector2 start = uv * (PreviewSize - 1);
				Vector2 end = start + Vector2.ClampMagnitude(velocity, 1.2f) * 18f;
				Line(pixels, start, end, arrow);
			}
		}
	}

	private static void Line(Color[] pixels, Vector2 start, Vector2 end, Color color)
	{
		int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(start, end)));
		for (int i = 0; i <= steps; i++)
		{
			Vector2 point = Vector2.Lerp(start, end, i / (float)steps);
			int x = Mathf.RoundToInt(point.x);
			int y = Mathf.RoundToInt(point.y);
			if (x < 0 || y < 0 || x >= PreviewSize || y >= PreviewSize)
			{
				continue;
			}
			pixels[y * PreviewSize + x] = color;
		}
	}

	private void DuringSceneGui(SceneView view)
	{
		if (!placeArmed || sim == null)
		{
			return;
		}

		Event current = Event.current;
		Vector3 hit;
		bool hasHit = TryHitSimPlane(current.mousePosition, out hit);
		if (hasHit && current.type == EventType.Repaint)
		{
			Handles.color = new Color(Mathf.Max(reaction.x, 0.2f), Mathf.Max(reaction.y, 0.15f), Mathf.Max(reaction.z, 0.15f), 0.9f);
			Handles.DrawWireDisc(hit, sim.transform.forward, Mathf.Max(scaleX, scaleY) * 0.5f);
		}
		if (current.type == EventType.MouseMove)
		{
			view.Repaint();
		}
		if (hasHit && current.type == EventType.MouseDown && current.button == 0 && !current.alt)
		{
			Spawn(hit);
			current.Use();
		}
	}

	private void Spawn(Vector3 world)
	{
		if (sim == null)
		{
			return;
		}
		if (sprite == null)
		{
			sprite = EnsureSprite();
		}
		int group = Undo.GetCurrentGroup();
		Undo.SetCurrentGroupName("生成流体发射器");
		GameObject[] created = new GameObject[spawnCount];
		for (int i = 0; i < spawnCount; i++)
		{
			Vector3 offset = Vector3.zero;
			if (i > 0)
			{
				offset = sim.transform.right * Random.Range(-spread, spread) + sim.transform.up * Random.Range(-spread, spread);
			}
			created[i] = CreateEmitter(world + offset);
		}
		Undo.CollapseUndoOperations(group);
		Selection.objects = created;
		placeArmed = false;
	}

	private GameObject CreateEmitter(Vector3 world)
	{
		string objectName = kind == EmitterKind.Explosion ? "FluidEmitterExplosion" : "FluidEmitter";
		GameObject go = new GameObject(objectName);
		Undo.RegisterCreatedObjectUndo(go, "生成流体发射器");
		go.transform.SetParent(sim.transform, false);
		go.transform.localRotation = Quaternion.identity;
		Vector3 local = sim.transform.InverseTransformPoint(world);
		local.z = -0.55f;
		go.transform.localPosition = local;
		Vector3 lossy = sim.transform.lossyScale;
		float safeX = Mathf.Approximately(lossy.x, 0f) ? 1f : lossy.x;
		float safeY = Mathf.Approximately(lossy.y, 0f) ? 1f : lossy.y;
		go.transform.localScale = new Vector3(scaleX / safeX, scaleY / safeY, 1f);

		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		FluidEmitter emitter = kind == EmitterKind.Explosion
			? (FluidEmitter)go.AddComponent<FluidEmitterExplosion>()
			: go.AddComponent<FluidEmitter>();
		WriteEmitter(emitter);
		WriteExtras(go);
		return go;
	}

	private void WriteToSelection()
	{
		FluidEmitter emitter = SelectedEmitter();
		if (emitter == null)
		{
			return;
		}
		Undo.RecordObject(emitter, "写回流体参数");
		WriteEmitter(emitter);
		WriteExtras(emitter.gameObject);
		EditorUtility.SetDirty(emitter);
	}

	private static FluidEmitter SelectedEmitter()
	{
		return Selection.activeGameObject == null ? null : Selection.activeGameObject.GetComponent<FluidEmitter>();
	}

	private void WriteEmitter(FluidEmitter emitter)
	{
		emitter.reaction = reaction;
		emitter.temperature = temperature;
		emitter.velocityX = velocityX;
		emitter.velocityY = velocityY;
		emitter.outwardsForce = outwardsForce;
		emitter.noiseForce = noiseForce;
		emitter.rotationalForce = rotationalForce;
		emitter.inheritEmitterVelocity = inheritVelocity;
		emitter.emitterVelocityScaling = emitterVelocityScaling;
		FluidEmitterExplosion explosion = emitter as FluidEmitterExplosion;
		if (explosion != null)
		{
			explosion.renderFrames = renderFrames;
		}
	}

	private void WriteExtras(GameObject go)
	{
		FluidEmitterVelocityVariation existingVariation = go.GetComponent<FluidEmitterVelocityVariation>();
		if (variation == VariationMode.None)
		{
			if (existingVariation != null)
			{
				Undo.DestroyObjectImmediate(existingVariation);
			}
		}
		else
		{
			if (existingVariation == null)
			{
				existingVariation = Undo.AddComponent<FluidEmitterVelocityVariation>(go);
			}
			existingVariation.type = variation == VariationMode.Rotate
				? FluidEmitterVelocityVariationType.Rotate
				: FluidEmitterVelocityVariationType.Noise;
			existingVariation.intensity = variationIntensity;
			existingVariation.speed = variationSpeed;
		}

		FluidEmitterMouseController mouse = go.GetComponent<FluidEmitterMouseController>();
		if (!followMouse)
		{
			if (mouse != null)
			{
				Undo.DestroyObjectImmediate(mouse);
			}
		}
		else
		{
			if (mouse == null)
			{
				mouse = Undo.AddComponent<FluidEmitterMouseController>(go);
			}
			mouse.cam = Camera.main;
			mouse.distanceToCamera = mouseDistance;
		}
	}

	private void LoadFrom(FluidEmitter emitter)
	{
		templateIndex = -1;
		kind = emitter is FluidEmitterExplosion ? EmitterKind.Explosion : EmitterKind.Continuous;
		reaction = emitter.reaction;
		temperature = emitter.temperature;
		velocityX = emitter.velocityX;
		velocityY = emitter.velocityY;
		outwardsForce = emitter.outwardsForce;
		noiseForce = emitter.noiseForce;
		rotationalForce = emitter.rotationalForce;
		inheritVelocity = emitter.inheritEmitterVelocity;
		emitterVelocityScaling = emitter.emitterVelocityScaling;
		FluidEmitterExplosion explosion = emitter as FluidEmitterExplosion;
		if (explosion != null)
		{
			renderFrames = explosion.renderFrames;
		}

		FluidEmitterVelocityVariation existingVariation = emitter.GetComponent<FluidEmitterVelocityVariation>();
		if (existingVariation == null)
		{
			variation = VariationMode.None;
		}
		else
		{
			variation = existingVariation.type == FluidEmitterVelocityVariationType.Rotate ? VariationMode.Rotate : VariationMode.Noise;
			variationIntensity = existingVariation.intensity;
			variationSpeed = existingVariation.speed;
		}

		FluidEmitterMouseController mouse = emitter.GetComponent<FluidEmitterMouseController>();
		followMouse = mouse != null;
		if (mouse != null)
		{
			mouseDistance = mouse.distanceToCamera;
		}

		SpriteRenderer spriteRenderer = emitter.GetComponent<SpriteRenderer>();
		if (spriteRenderer != null && spriteRenderer.sprite != null)
		{
			sprite = spriteRenderer.sprite;
		}
		Vector3 worldScale = emitter.transform.lossyScale;
		scaleX = worldScale.x;
		scaleY = worldScale.y;
	}

	private bool TryHitSimPlane(Vector2 guiPosition, out Vector3 world)
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
		Plane plane = new Plane(sim.transform.forward, sim.transform.position);
		float distance;
		if (plane.Raycast(ray, out distance))
		{
			world = ray.GetPoint(distance);
			return true;
		}
		world = sim.transform.position;
		return false;
	}

	private static Sprite EnsureSprite()
	{
		Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
		if (existing != null)
		{
			return existing;
		}
		Directory.CreateDirectory("Assets/Fluid");
		Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
		for (int y = 0; y < 128; y++)
		{
			for (int x = 0; x < 128; x++)
			{
				float dx = (x + 0.5f) / 128f * 2f - 1f;
				float dy = (y + 0.5f) / 128f * 2f - 1f;
				float alpha = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
				alpha *= alpha;
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
		Object.DestroyImmediate(texture);
		AssetDatabase.ImportAsset(SpritePath);
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
		importer.textureType = TextureImporterType.Sprite;
		importer.alphaIsTransparency = true;
		importer.mipmapEnabled = false;
		importer.spritePixelsPerUnit = 128f;
		importer.SaveAndReimport();
		return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
	}

	private static EffectTemplate T(string name, string source, EmitterKind kind, Vector3 reactionValue, float temperatureValue, float velocityXValue, float velocityYValue, float outwards, float noise, float rotational, bool inherit, float velocityScale, int frames, VariationMode variationMode, float variationIntensityValue, float variationSpeedValue, float scale)
	{
		EffectTemplate template = new EffectTemplate();
		template.name = name;
		template.source = source;
		template.kind = kind;
		template.reaction = reactionValue;
		template.temperature = temperatureValue;
		template.velocityX = velocityXValue;
		template.velocityY = velocityYValue;
		template.outwardsForce = outwards;
		template.noiseForce = noise;
		template.rotationalForce = rotational;
		template.inheritVelocity = inherit;
		template.emitterVelocityScaling = velocityScale;
		template.renderFrames = frames;
		template.variation = variationMode;
		template.variationIntensity = variationIntensityValue;
		template.variationSpeed = variationSpeedValue;
		template.scale = scale;
		return template;
	}

	private void ApplyTemplate(EffectTemplate template)
	{
		kind = template.kind;
		reaction = template.reaction;
		temperature = template.temperature;
		velocityX = template.velocityX;
		velocityY = template.velocityY;
		outwardsForce = template.outwardsForce;
		noiseForce = template.noiseForce;
		rotationalForce = template.rotationalForce;
		inheritVelocity = template.inheritVelocity;
		emitterVelocityScaling = template.emitterVelocityScaling;
		renderFrames = template.renderFrames;
		variation = template.variation;
		variationIntensity = template.variationIntensity;
		variationSpeed = template.variationSpeed;
		scaleX = template.scale;
		scaleY = template.scale;
		followMouse = false;
	}
}
