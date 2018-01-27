using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class ToonShaderGUI : ShaderGUI 
{

	enum SmoothnessSource 
    {
		Uniform, Albedo, Metallic
	}

	enum LightingType
	{
		PBR, Toon
	}

	enum RenderingMode 
    {
		Opaque, Cutout, Fade, Transparent
	}

	enum CullingMode 
    {
		Off = 0, Front = 1, Back = 2
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

	struct CullSettings 
    {
		public CullMode cull;
		public static CullSettings[] modes = 
        {
			new CullSettings() 
            {
				cull = CullMode.Off
			},
			new CullSettings() 
            {
				cull =  CullMode.Front
			},
			new CullSettings() 
            {
				cull =  CullMode.Back
			}
		};
	}

	static GUIContent staticLabel = new GUIContent();

	static ColorPickerHDRConfig emissionConfig =
		new ColorPickerHDRConfig(0f, 99f, 1f / 99f, 3f);

	static ColorPickerHDRConfig rimConfig =
		new ColorPickerHDRConfig(0f, 99f, 1f / 99f, 3f);
	
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
		DoRenderingMode();
		DoCullingMode();
		DoMain();
		DoSecondary();
		DoDither();
		DoRim();
		DoOutline();
		DoShiny();
		DoDiscard();
		DoAdvanced();
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

		if (mode == RenderingMode.Fade || mode == RenderingMode.Transparent) 
        {
			DoSemitransparentShadows();
		}
	}

	void DoCullingMode()
	{
		MaterialProperty type = FindProperty("_Cull");
		CullingMode mode = (CullingMode)type.floatValue;

		EditorGUI.BeginChangeCheck();
		mode = (CullingMode)EditorGUILayout.EnumPopup(
			MakeLabel("Culling Mode"), mode);
		if (EditorGUI.EndChangeCheck()) 
        {
			RecordAction("Culling Mode");

			CullSettings settings = CullSettings.modes[(int)mode];
			foreach (Material m in editor.targets) 
            {
				m.SetInt("_Cull", (int)settings.cull);
			}
		}
	}

    void DoToon()
    {
        LightingType lightType = LightingType.PBR;
		bool rampTexColor = false;
		//if (IsKeywordEnabled("_LIGHT_PBR")) 
        //{
		//	lightType = LightingType.PBR;
		//}
		if (IsKeywordEnabled("_LIGHT_TOON")) 
        {
			lightType = LightingType.Toon;
		}
		//MaterialProperty slider = FindProperty("_Smoothness");
		EditorGUI.indentLevel += 2;
		//editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel += 1;
		EditorGUI.BeginChangeCheck();
		lightType = (LightingType)EditorGUILayout.EnumPopup(
			MakeLabel("Lighting Type"), lightType);
		
		if (IsKeywordEnabled("_LIGHT_TOON")) 
		{			
			MaterialProperty rampTex = FindProperty("_Ramp");
			editor.TexturePropertySingleLine(
			MakeLabel(rampTex, "Ramp Texture"), rampTex);
			rampTexColor =
			EditorGUILayout.Toggle(
				MakeLabel("Colored Toon Ramp", "Colored Toon Ramp"),
				IsKeywordEnabled("_TOON_COLOR"));
		}
		
			
		if (EditorGUI.EndChangeCheck()) 
        {
			RecordAction("Lighting Type");
			SetKeyword("_TOON_COLOR", rampTexColor);
			SetKeyword("_LIGHT_TOON", lightType == LightingType.Toon);
		}
		EditorGUI.indentLevel -= 3;
    }

	void DoOutline()
	{		
		//EditorGUI.indentLevel += 2;
		if(FindProperty("_Outline", properties, false) != null)
		{
			GUILayout.Label("Outline", EditorStyles.boldLabel);
			MaterialProperty outline = FindProperty("_Outline");
			MaterialProperty outlineColor = FindProperty("_OutlineColor");

			EditorGUI.BeginChangeCheck();
			
			editor.ShaderProperty(outlineColor, MakeLabel(outlineColor));
			editor.ShaderProperty(outline, MakeLabel(outline));

			if(FindProperty("_Outline1", properties, false) != null)
			{
				MaterialProperty outline1 = FindProperty("_Outline1");
				MaterialProperty outlineColor1 = FindProperty("_OutlineColor1");
					
				editor.ShaderProperty(outlineColor1, MakeLabel(outlineColor1));
				editor.ShaderProperty(outline1, MakeLabel(outline1));

				if (EditorGUI.EndChangeCheck()) 
    	  	 	{

				}

			}
		}
		//EditorGUI.indentLevel -= 2;
	}

	void DoSemitransparentShadows () 
    {
		EditorGUI.BeginChangeCheck();
		bool semitransparentShadows =
			EditorGUILayout.Toggle(
				MakeLabel("Semitransp. Shadows", "Semitransparent Shadows"),
				IsKeywordEnabled("_SEMITRANSPARENT_SHADOWS"));
		if (EditorGUI.EndChangeCheck()) 
        {
			SetKeyword("_SEMITRANSPARENT_SHADOWS", semitransparentShadows);
		}
		if (!semitransparentShadows) 
        {
			shouldShowAlphaCutoff = true;
		}
	}

	void DoMain () 
    {
		GUILayout.Label("Main Maps", EditorStyles.boldLabel);

		MaterialProperty mainTex = FindProperty("_MainTex");
		editor.TexturePropertySingleLine(
			MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Color"));

		if (shouldShowAlphaCutoff) 
        {
			DoAlphaCutoff();
		}
        DoToon();
		DoMetallic();
		DoSmoothness();
		DoNormals();
		DoParallax();
		DoOcclusion();
		DoEmission();
		DoDetailMask();
		editor.TextureScaleOffsetProperty(mainTex);
	}
	
	void DoDither ()
	{
		GUILayout.Label("Dither Tranparency", EditorStyles.boldLabel);

		MaterialProperty ditherDistance = 	FindProperty("_DitherDistance");
		MaterialProperty ditherTex = 	FindProperty("_DitherTex");

		EditorGUI.BeginChangeCheck();

		bool ditherActive = EditorGUILayout.Toggle(
				MakeLabel("Dither Active", "Dither Transparency Active"),
				IsKeywordEnabled("_DITHER_MAP"));
		if(ditherActive)
		{
			editor.TexturePropertySingleLine(MakeLabel(ditherTex, "Dither Texture"), ditherTex);
			editor.ShaderProperty(ditherDistance, MakeLabel(ditherDistance, "Distance for dither transparency to take effect \nX controls Min Distance\nY controls Max Distance"), 2);
		}

		if (EditorGUI.EndChangeCheck()) 
		{			
			//SetKeyword("_DITHER_MAP", (Mathf.Abs(ditherDistance.vectorValue.x) + Mathf.Abs(ditherDistance.vectorValue.y) +
			// Mathf.Abs(ditherDistance.vectorValue.z) + Mathf.Abs(ditherDistance.vectorValue.w)) > 0.0f);
			SetKeyword("_DITHER_MAP", ditherActive);
		}
	}

	void DoAlphaCutoff () 
    {
		MaterialProperty slider = FindProperty("_AlphaCutoff");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel -= 2;
	}

	void DoNormals () 
    {
		MaterialProperty map = FindProperty("_BumpTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map), map,
			tex ? FindProperty("_BumpScale") : null
		);
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) {
			SetKeyword("_NORMAL_MAP", map.textureValue);
		}
	}

	void DoMetallic () 
    {
		MaterialProperty map = FindProperty("_MetallicTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map, "Metallic (R)"), map,
			tex ? null : FindProperty("_Metallic")
		);
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) 
        {
			SetKeyword("_METALLIC_MAP", map.textureValue);
		}
	}

	void DoSmoothness () 
    {
		SmoothnessSource source = SmoothnessSource.Uniform;
		if (IsKeywordEnabled("_SMOOTHNESS_ALBEDO")) 
        {
			source = SmoothnessSource.Albedo;
		}
		else if (IsKeywordEnabled("_SMOOTHNESS_METALLIC")) 
        {
			source = SmoothnessSource.Metallic;
		}
		MaterialProperty slider = FindProperty("_Smoothness");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel += 1;
		EditorGUI.BeginChangeCheck();
		source = (SmoothnessSource)EditorGUILayout.EnumPopup(
			MakeLabel("Source"), source
		);
		if (EditorGUI.EndChangeCheck()) 
        {
			RecordAction("Smoothness Source");
			SetKeyword("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
			SetKeyword("_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic);
		}
		EditorGUI.indentLevel -= 3;
	}

	void DoParallax () 
    {
		MaterialProperty map = FindProperty("_ParallaxTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map, "Parallax (G)"), map,
			tex ? FindProperty("_ParallaxStrength") : null);
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) 
        {
			SetKeyword("_PARALLAX_MAP", map.textureValue);
		}
	}

	void DoOcclusion () 
    {
		MaterialProperty map = FindProperty("_OcclusionTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map, "Occlusion (G)"), map,
			tex ? FindProperty("_OcclusionStrength") : null);
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) 
        {
			SetKeyword("_OCCLUSION_MAP", map.textureValue);
		}
	}

	void DoEmission () 
    {
		MaterialProperty map = FindProperty("_EmissionTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertyWithHDRColor(
			MakeLabel(map, "Emission (RGB)"), map, FindProperty("_Emission"),
			emissionConfig, false);
		editor.LightmapEmissionProperty(2);
		if (EditorGUI.EndChangeCheck()) 
        {
			if (tex != map.textureValue) 
            {
				SetKeyword("_EMISSION_MAP", map.textureValue);
			}

			foreach (Material m in editor.targets) 
            {
				m.globalIlluminationFlags &=
					~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
			}
		}
	}

	void DoDetailMask () 
	{
		MaterialProperty mask = FindProperty("_DetailMask");
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(mask, "Detail Mask (A)"), mask);
		if (EditorGUI.EndChangeCheck()) 
        {
			SetKeyword("_DETAIL_MASK", mask.textureValue);
		}
	}

	void DoDiscard () 
	{
		GUILayout.Label("Discard Interpolation", EditorStyles.boldLabel);

		MaterialProperty discScale = FindProperty("_DiscScale");
		MaterialProperty discSize =  FindProperty("_DiscSize");

		EditorGUI.BeginChangeCheck();

		editor.ShaderProperty(discScale, MakeLabel(discScale, "Discard Scale"), 2);
		editor.ShaderProperty(discSize, MakeLabel(discSize, "Discard Size"), 2);
		
		if (EditorGUI.EndChangeCheck()) 
		{
			bool blank = false;
			if(discScale.floatValue > 0.0f)
			{
				blank = true;
			}
			SetKeyword("_DISCARD_INTERP", blank);
		}
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
		bool shineMetallicTex = EditorGUILayout.Toggle(
			MakeLabel("Shine Metallic", "Have the shine amount be sampled from Metallic Texture (G)?"), 
			IsKeywordEnabled("_SHINY_METALLIC_TEX"));
		//editor.TexturePropertySingleLine(
		//	MakeLabel(shinySpeed, "Shiny Speed"), shinySpeed);
		//editor.TexturePropertySingleLine(
		//	MakeLabel(shinyTex, "Shiny Ramp (RGB)"), shinyTex,
		//	shinyColor, shinySpeed);
		
		if (EditorGUI.EndChangeCheck()) 
		{
			bool blank = false;
			float color = shinyColor.colorValue.a *
			(shinyColor.colorValue.r + shinyColor.colorValue.g + shinyColor.colorValue.b);
			if(color > 0.0f)
			{
				blank = true;
			}
			
			SetKeyword("_SHINY_METALLIC_TEX", shineMetallicTex);
			SetKeyword("_SHINY_RAMP", blank && shinyTex.textureValue);
		}
	}

    void DoRim () 
	{
		GUILayout.Label("Rim Light", EditorStyles.boldLabel);

		MaterialProperty rimColor = FindProperty("_RimColor");
		MaterialProperty rimPower = FindProperty("_RimPower");
		MaterialProperty rimTex = 	FindProperty("_RimTex");
		MaterialProperty rimRampTex = 	FindProperty("_RimRampTex");
		MaterialProperty rimScroll = 	FindProperty("_RimScroll");

		EditorGUI.BeginChangeCheck();
		
		bool reverseRim = EditorGUILayout.Toggle(
				MakeLabel("Reverse Rim Light", "Reverse Rim Light"),
				IsKeywordEnabled("_RIM_REVERSE"));

		editor.TexturePropertyWithHDRColor(
			MakeLabel(rimTex, "Rim Light"), rimTex, rimColor,
			rimConfig, false);

		//editor.TexturePropertySingleLine(
		//	MakeLabel(rimTex, "Rim Light"), rimTex,
		//	rimColor, rimPower);

			editor.TexturePropertySingleLine(
			MakeLabel(rimRampTex, "Rim Ramp"), rimRampTex, rimPower);
			
		editor.ShaderProperty(rimScroll, MakeLabel(rimScroll, "Rim Scroll Speed"), 2);
		//editor.ShaderProperty(rimColor, MakeLabel(rimColor, "Rim Color"), 2);
		//editor.ShaderProperty(rimPower, MakeLabel(rimPower, "Rim Power"), 2);
		//editor.ColorProperty(
		//	MakeLabel(rimColor, "Rim Color"), rimColor, rimPower);

		
		
		if (EditorGUI.EndChangeCheck()) 
		{
			SetKeyword("_RIM_LIGHT_MAP", rimTex.textureValue);
			SetKeyword("_RIM_LIGHT_RAMP", rimRampTex.textureValue);
			
			SetKeyword("_RIM_SCROLL", (Mathf.Abs(rimScroll.vectorValue.x) + Mathf.Abs(rimScroll.vectorValue.y) +
			 Mathf.Abs(rimScroll.vectorValue.z) + Mathf.Abs(rimScroll.vectorValue.w)) > 0.0f);
			
			bool blank = false;
			float color = rimColor.colorValue.a *
			(rimColor.colorValue.r + rimColor.colorValue.g + rimColor.colorValue.b);
			if(color > 0.0f)
			{
				blank = true;
			}
			SetKeyword("_RIM_LIGHT", blank);			
			SetKeyword("_RIM_REVERSE", reverseRim);
		}
	}

	void DoSecondary () 
    {
		GUILayout.Label("Secondary Maps", EditorStyles.boldLabel);

		MaterialProperty detailTex = FindProperty("_DetailTex");
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(detailTex, "Detail Albedo (RGB) multiplied by 2\n" +
			"The slider controls the amount of detail\n" +
			"0 is none, 1 is max"), 
			detailTex,
			detailTex.textureValue ? FindProperty("_DetailScale") : null);
		if (EditorGUI.EndChangeCheck()) 
        {
			SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
		}
		DoSecondaryNormals();
		editor.TextureScaleOffsetProperty(detailTex);
	}

	void DoSecondaryNormals () 
    {
		MaterialProperty map = FindProperty("_DetailBumpTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map), map,
			tex ? FindProperty("_DetailBumpScale") : null
		);
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) 
        {
			SetKeyword("_DETAIL_NORMAL_MAP", map.textureValue);
		}
	}

	void DoAdvanced () 
    {
		GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

		editor.EnableInstancingField();
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