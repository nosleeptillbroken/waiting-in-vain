using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class TerrainShaderGUI : ShaderGUI 
{
	enum RenderingMode 
    {
		Opaque, Cutout, Fade, Transparent
	}

	struct RenderingSettings 
    {
		public RenderQueue queue;
		public string renderType;
		public BlendMode srcBlend, dstBlend;
		public bool zWrite;

		public static RenderingSettings[] modes = 
        {
			new RenderingSettings() 
            {
				queue = RenderQueue.Geometry,
				renderType = "",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() 
            {
				queue = RenderQueue.AlphaTest,
				renderType = "TransparentCutout",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() 
            {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.SrcAlpha,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			},
			new RenderingSettings() 
            {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			}
		};
	}

	
	static GUIContent staticLabel = new GUIContent();

	static ColorPickerHDRConfig sheenConfig =
		new ColorPickerHDRConfig(0f, 99f, 1f / 99f, 3f);

	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;
	bool shouldShowAlphaCutoff;

	public override void OnGUI (MaterialEditor editor, MaterialProperty[] properties) 
    {
		this.target = editor.target as Material;
		this.editor = editor;
		this.properties = properties;
		//DoRenderingMode();
		DoMain();
		DoShiny();
	}

	void DoRenderingMode () 
    {
		RenderingMode mode = RenderingMode.Opaque;
		shouldShowAlphaCutoff = false;
		if (IsKeywordEnabled("_RENDERING_CUTOUT")) 
        {
			mode = RenderingMode.Cutout;
			shouldShowAlphaCutoff = true;
		}
		else if (IsKeywordEnabled("_RENDERING_FADE")) 
        {
			mode = RenderingMode.Fade;
		}
		else if (IsKeywordEnabled("_RENDERING_TRANSPARENT")) 
        {
			mode = RenderingMode.Transparent;
		}

		MaterialProperty renderingOffset = FindProperty("_RenderQueueOffset");
		
		EditorGUI.BeginChangeCheck();
		mode = (RenderingMode)EditorGUILayout.EnumPopup(
			MakeLabel("Rendering Mode"), mode);
		//EditorGUILayout.IntField((int)renderingOffset.floatValue);		
		editor.ShaderProperty(renderingOffset, MakeLabel(renderingOffset, "Rendering Offset"));

		if (EditorGUI.EndChangeCheck()) 
        {
			RecordAction("Rendering Mode");
			SetKeyword("_RENDERING_CUTOUT", mode == RenderingMode.Cutout);
			SetKeyword("_RENDERING_FADE", mode == RenderingMode.Fade);
			SetKeyword("_RENDERING_TRANSPARENT", mode == RenderingMode.Transparent);

			RenderingSettings settings = RenderingSettings.modes[(int)mode];
			foreach (Material m in editor.targets) 
            {
				m.renderQueue = (int)settings.queue + (int)renderingOffset.floatValue;
				m.SetOverrideTag("RenderType", settings.renderType);
				m.SetInt("_SrcBlend", (int)settings.srcBlend);
				m.SetInt("_DstBlend", (int)settings.dstBlend);
				m.SetInt("_ZWrite", settings.zWrite ? 1 : 0);
			}
		}
	}

	void DoMain () 
    {
		GUILayout.Label("Main Maps", EditorStyles.boldLabel);

		MaterialProperty mainTex = FindProperty("_MainTex");
		editor.TexturePropertySingleLine(
			MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_ColorMult"));

		MaterialProperty cloudTex = FindProperty("_CloudTex");
		editor.TexturePropertySingleLine(
			MakeLabel(cloudTex, "Cloud Texture"), cloudTex);

		MaterialProperty cutoffSlider = FindProperty("_CloudCutoff");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(cutoffSlider, MakeLabel(cutoffSlider));
		EditorGUI.indentLevel -= 2;

		SetKeyword("_ALPHA_CUTOFF", cutoffSlider.floatValue > 0.0f);
		

		//if (shouldShowAlphaCutoff) 
        //{
		//	DoAlphaCutoff();
		//}
		editor.TextureScaleOffsetProperty(mainTex);

		
		
	}

	void DoShiny () 
	{
		GUILayout.Label("Shiny Sheen", EditorStyles.boldLabel);

		MaterialProperty shinyTex =   FindProperty("_ShinyRamp");
		MaterialProperty shinyColor = FindProperty("_ShinyColor");
		MaterialProperty shinySpeed = FindProperty("_ShinySpeed");

		EditorGUI.BeginChangeCheck();

		editor.TexturePropertyWithHDRColor(
			MakeLabel(shinyTex, "Shiny Ramp"), shinyTex, shinyColor,
			sheenConfig, false);
		editor.ShaderProperty(shinySpeed, MakeLabel(shinySpeed, "Shiny Speed"), 2);
		bool shineOffsetTex = EditorGUILayout.Toggle(
			MakeLabel("Shine Offset", "Have the shine amount be offset by the color of _ColorMult?"), 
			IsKeywordEnabled("_SHINY_RAMP_OFFSET"));
			
		bool shineWorldPos = EditorGUILayout.Toggle(
			MakeLabel("Shine World", "Use world position instead of screen position?"), 
			IsKeywordEnabled("_SHINY_RAMP_WORLD_POS"));

		if(shineWorldPos)
		{			
			MaterialProperty shineWorldPosAmount = 	FindProperty("_ShinyPosSize");
			editor.ShaderProperty(shineWorldPosAmount, 
			MakeLabel(shineWorldPosAmount, "Scaling for world pos"), 2);
		}

		bool shineColorMain = EditorGUILayout.Toggle(
			MakeLabel("Shine _ColorMult", "Use _ColorMult in shine color calc?"), 
			IsKeywordEnabled("_SHINY_RAMP_COLOR_MULT"));
			
		
		if (EditorGUI.EndChangeCheck()) 
		{
			bool blank = false;
			float color = shinyColor.colorValue.a *
			(shinyColor.colorValue.r + shinyColor.colorValue.g + shinyColor.colorValue.b);
			if(color > 0.0f)
			{
				blank = true;
			}
			
			SetKeyword("_SHINY_RAMP_OFFSET", shineOffsetTex);
			SetKeyword("_SHINY_RAMP_WORLD_POS", shineWorldPos);
			SetKeyword("_SHINY_RAMP_COLOR_MULT", shineColorMain);
			SetKeyword("_SHINY_RAMP", blank && shinyTex.textureValue);
		}
	}
	
	void DoAlphaCutoff () 
    {
		MaterialProperty slider = FindProperty("_AlphaCutoff");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel -= 2;
	}

	MaterialProperty FindProperty (string name) 
    {
		return FindProperty(name, properties);
	}

	static GUIContent MakeLabel (string text, string tooltip = null) 
    {
		staticLabel.text = text;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	static GUIContent MakeLabel (
		MaterialProperty property, string tooltip = null) 
    {
		staticLabel.text = property.displayName;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	void SetKeyword (string keyword, bool state) 
    {
		if (state) {
			foreach (Material m in editor.targets) 
            {
				m.EnableKeyword(keyword);
			}
		}
		else {
			foreach (Material m in editor.targets) 
            {
				m.DisableKeyword(keyword);
			}
		}
	}

	bool IsKeywordEnabled (string keyword) 
    {
		return target.IsKeywordEnabled(keyword);
	}

	void RecordAction (string label) 
    {
		editor.RegisterPropertyChangeUndo(label);
	}
}