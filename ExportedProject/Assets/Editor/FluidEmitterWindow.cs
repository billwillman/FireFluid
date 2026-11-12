using UnityEditor;
using UnityEngine;

public class FluidEmitterWindow : EditorWindow
{
	private enum EmitterKind
	{
		Continuous,
		Explosion
	}

	private enum Preset
	{
		Custom,
		Fire,
		Poison,
		Frost,
		Heat,
		Wind,
		Swirl,
		Blast
	}

	private enum VariationMode
	{
		None,
		Rotate,
		Noise
	}

	private static readonly string[] PresetNames = { "自定义", "火", "毒", "霜", "热源", "风", "旋涡", "爆炸" };
	private static readonly string[] KindNames = { "持续发射", "爆炸" };
	private static readonly string[] VariationNames = { "不附加", "旋转", "噪声" };

	private FluidSim sim;
	private Sprite sprite;
	private Preset preset = Preset.Fire;
	private EmitterKind kind = EmitterKind.Continuous;
	private Vector3 reaction = new Vector3(0.5f, 0f, 0f);
	private float temperature = 1f;
	private float velocityX;
	private float velocityY;
	private float outwardsForce = 0.2f;
	private float noiseForce = 0.15f;
	private float rotationalForce;
	private bool inheritVelocity = true;
	private float emitterVelocityScaling = 8f;
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
	private Vector2 scroll;

	[MenuItem("Tools/流体/发射器")]
	public static void Open()
	{
		GetWindow<FluidEmitterWindow>("流体发射器");
	}

	private void OnEnable()
	{
		if (sprite == null)
		{
			sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprite/ExplosionEmitter.asset");
		}
		if (sim == null)
		{
			sim = FindFirstObjectByType<FluidSim>();
		}
		SceneView.duringSceneGui += DuringSceneGui;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= DuringSceneGui;
	}

	private void OnGUI()
	{
		scroll = EditorGUILayout.BeginScrollView(scroll);
		EditorGUILayout.HelpBox(
			"生成的实例会进入场景里的 FluidSim。反应 R/G/B、温度、速度 XY、向外、噪声、旋转都会写进发射贴图，再参与平流。进入播放模式后才会真正步进。",
			MessageType.Info);

		sim = (FluidSim)EditorGUILayout.ObjectField("流体模拟", sim, typeof(FluidSim), true);
		sprite = (Sprite)EditorGUILayout.ObjectField("发射形状", sprite, typeof(Sprite), false);
		if (GUILayout.Button("查找场景中的 FluidSim"))
		{
			sim = FindFirstObjectByType<FluidSim>();
		}

		EditorGUILayout.Space();
		EditorGUI.BeginChangeCheck();
		int presetIndex = EditorGUILayout.Popup("预设", (int)preset, PresetNames);
		if (EditorGUI.EndChangeCheck())
		{
			preset = (Preset)presetIndex;
			ApplyPreset(preset);
		}

		kind = (EmitterKind)EditorGUILayout.Popup("实例", (int)kind, KindNames);
		scaleX = EditorGUILayout.FloatField("世界缩放 X", scaleX);
		scaleY = EditorGUILayout.FloatField("世界缩放 Y", scaleY);
		spawnCount = Mathf.Max(1, EditorGUILayout.IntField("数量", spawnCount));
		if (spawnCount > 1)
		{
			spread = EditorGUILayout.FloatField("散布半径", spread);
		}

		EditorGUILayout.Space();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField("反应", EditorStyles.boldLabel);
		reaction.x = EditorGUILayout.Slider("R 火", reaction.x, 0f, 1f);
		reaction.y = EditorGUILayout.Slider("G 毒", reaction.y, 0f, 1f);
		reaction.z = EditorGUILayout.Slider("B 霜", reaction.z, 0f, 1f);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("温度", EditorStyles.boldLabel);
		temperature = EditorGUILayout.Slider("温度", temperature, -1f, 1f);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("速度", EditorStyles.boldLabel);
		velocityX = EditorGUILayout.Slider("速度 X", velocityX, -1f, 1f);
		velocityY = EditorGUILayout.Slider("速度 Y", velocityY, -1f, 1f);
		outwardsForce = EditorGUILayout.FloatField("向外", outwardsForce);
		noiseForce = EditorGUILayout.FloatField("噪声", noiseForce);
		rotationalForce = EditorGUILayout.FloatField("旋转", rotationalForce);
		inheritVelocity = EditorGUILayout.Toggle("继承发射器位移", inheritVelocity);
		emitterVelocityScaling = EditorGUILayout.FloatField("位移速度缩放", emitterVelocityScaling);

		if (kind == EmitterKind.Explosion)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("爆炸", EditorStyles.boldLabel);
			renderFrames = Mathf.Max(1, EditorGUILayout.IntField("存活模拟步", renderFrames));
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("附加行为", EditorStyles.boldLabel);
		variation = (VariationMode)EditorGUILayout.Popup("速度变化", (int)variation, VariationNames);
		if (variation != VariationMode.None)
		{
			variationIntensity = EditorGUILayout.FloatField("变化强度", variationIntensity);
			variationSpeed = EditorGUILayout.FloatField("变化速度", variationSpeed);
			EditorGUILayout.HelpBox("速度变化会在运行时覆盖速度 X/Y。", MessageType.None);
		}
		followMouse = EditorGUILayout.Toggle("跟随鼠标", followMouse);
		if (followMouse)
		{
			mouseDistance = EditorGUILayout.FloatField("到摄像机距离", mouseDistance);
		}
		if (EditorGUI.EndChangeCheck())
		{
			preset = Preset.Custom;
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledScope(sim == null || sprite == null))
		{
			if (GUILayout.Button("在模拟中心生成"))
			{
				Spawn(sim.transform.position);
			}
			placeArmed = GUILayout.Toggle(placeArmed, placeArmed ? "点击场景放置（已武装，再点一次取消）" : "点击场景放置", "Button");
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledScope(Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<FluidEmitter>() == null))
		{
			if (GUILayout.Button("从选中发射器读取"))
			{
				LoadFrom(Selection.activeGameObject.GetComponent<FluidEmitter>());
			}
			if (GUILayout.Button("写回选中发射器"))
			{
				WriteToSelection();
			}
		}

		DrawExistingEmitters();
		EditorGUILayout.EndScrollView();
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
			Handles.color = new Color(reaction.x, Mathf.Max(reaction.y, 0.15f), Mathf.Max(reaction.z, 0.15f), 0.9f);
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
		if (sim == null || sprite == null)
		{
			return;
		}

		int group = Undo.GetCurrentGroup();
		Undo.SetCurrentGroupName("生成流体发射器");
		var created = new GameObject[spawnCount];
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
		var go = new GameObject(objectName);
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
		FluidEmitter[] emitters = Selection.GetFiltered<FluidEmitter>(SelectionMode.Editable | SelectionMode.ExcludePrefab);
		if (emitters.Length == 0 && Selection.activeGameObject != null)
		{
			FluidEmitter selected = Selection.activeGameObject.GetComponent<FluidEmitter>();
			if (selected != null)
			{
				emitters = new[] { selected };
			}
		}
		foreach (FluidEmitter emitter in emitters)
		{
			Undo.RecordObject(emitter, "写回流体参数");
			WriteEmitter(emitter);
			WriteExtras(emitter.gameObject);
			EditorUtility.SetDirty(emitter);
		}
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
		if (emitter is FluidEmitterExplosion explosion)
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
				existingVariation = go.AddComponent<FluidEmitterVelocityVariation>();
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
				mouse = go.AddComponent<FluidEmitterMouseController>();
			}
			mouse.cam = Camera.main;
			mouse.distanceToCamera = mouseDistance;
		}
	}

	private void LoadFrom(FluidEmitter emitter)
	{
		preset = Preset.Custom;
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
		if (emitter is FluidEmitterExplosion explosion)
		{
			renderFrames = explosion.renderFrames;
		}

		FluidEmitterVelocityVariation existingVariation = emitter.GetComponent<FluidEmitterVelocityVariation>();
		if (existingVariation == null)
		{
			variation = VariationMode.None;
		}
		else if (existingVariation.type == FluidEmitterVelocityVariationType.Rotate)
		{
			variation = VariationMode.Rotate;
			variationIntensity = existingVariation.intensity;
			variationSpeed = existingVariation.speed;
		}
		else
		{
			variation = VariationMode.Noise;
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

		if (sim != null)
		{
			Vector3 worldScale = emitter.transform.lossyScale;
			scaleX = worldScale.x;
			scaleY = worldScale.y;
		}
	}

	private void DrawExistingEmitters()
	{
		EditorGUILayout.Space();
		FluidEmitter[] emitters = FindObjectsByType<FluidEmitter>(FindObjectsSortMode.None);
		EditorGUILayout.LabelField("场景中的发射器  " + emitters.Length, EditorStyles.boldLabel);
		int shown = Mathf.Min(emitters.Length, 40);
		for (int i = 0; i < shown; i++)
		{
			FluidEmitter emitter = emitters[i];
			if (emitter == null)
			{
				continue;
			}
			EditorGUILayout.BeginHorizontal();
			string caption = emitter.name + "  R" + emitter.reaction.x.ToString("0.##") + " T" + emitter.temperature.ToString("0.##");
			if (GUILayout.Button(caption, EditorStyles.miniButton))
			{
				Selection.activeGameObject = emitter.gameObject;
				EditorGUIUtility.PingObject(emitter.gameObject);
			}
			EditorGUILayout.EndHorizontal();
		}
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

	private void ApplyPreset(Preset next)
	{
		inheritVelocity = true;
		emitterVelocityScaling = 8f;
		followMouse = false;
		variation = VariationMode.None;
		switch (next)
		{
		case Preset.Fire:
			kind = EmitterKind.Continuous;
			reaction = new Vector3(0.5f, 0f, 0f);
			temperature = 1f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 0.2f;
			noiseForce = 0.15f;
			rotationalForce = 0f;
			scaleX = 1f;
			scaleY = 1f;
			break;
		case Preset.Poison:
			kind = EmitterKind.Continuous;
			reaction = new Vector3(0f, 0.5f, 0f);
			temperature = 0.1f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 0.05f;
			noiseForce = 0.1f;
			rotationalForce = 0f;
			scaleX = 1f;
			scaleY = 1f;
			break;
		case Preset.Frost:
			kind = EmitterKind.Continuous;
			reaction = new Vector3(0f, 0f, 0.5f);
			temperature = -0.4f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 0.1f;
			noiseForce = 0.05f;
			rotationalForce = 0f;
			scaleX = 1.2f;
			scaleY = 1.2f;
			break;
		case Preset.Heat:
			kind = EmitterKind.Continuous;
			reaction = Vector3.zero;
			temperature = 1f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 0f;
			noiseForce = 0f;
			rotationalForce = 0f;
			scaleX = 1.5f;
			scaleY = 1.5f;
			break;
		case Preset.Wind:
			kind = EmitterKind.Continuous;
			reaction = Vector3.zero;
			temperature = 0f;
			velocityX = 0f;
			velocityY = 0.45f;
			outwardsForce = 0f;
			noiseForce = 0.05f;
			rotationalForce = 0f;
			scaleX = 2f;
			scaleY = 2f;
			break;
		case Preset.Swirl:
			kind = EmitterKind.Continuous;
			reaction = new Vector3(0.45f, 0f, 0f);
			temperature = 0.6f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 0.05f;
			noiseForce = 0.05f;
			rotationalForce = 1.5f;
			variation = VariationMode.Rotate;
			variationIntensity = 0.45f;
			variationSpeed = 2f;
			scaleX = 1.4f;
			scaleY = 1.4f;
			break;
		case Preset.Blast:
			kind = EmitterKind.Explosion;
			reaction = new Vector3(0.5f, 0f, 0f);
			temperature = 1f;
			velocityX = 0f;
			velocityY = 0f;
			outwardsForce = 32f;
			noiseForce = 2f;
			rotationalForce = 0f;
			renderFrames = 3;
			scaleX = 0.75f;
			scaleY = 0.75f;
			break;
		}
	}
}
