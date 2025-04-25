using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TurnBased.Rendering {
    /// <summary>
    /// Editor script for the Lit material inspector.
    /// </summary>
    public static class LitRMOGUI {
        /// <summary>
        /// Workflow modes for the shader.
        /// </summary>
        public enum WorkflowMode {
            /// <summary>
            /// Use this for RMO maps.
            /// </summary>
            RMO = 0,

            /// <summary>
            /// Use this for SMO maps.
            /// </summary>
            SMO
        }

        /// <summary>
        /// Container for the text and tooltips used to display the shader.
        /// </summary>
        public static class Styles {
            /// <summary>
            /// The text and tooltip for the workflow Mode GUI.
            /// </summary>
            public static GUIContent workflowModeText = EditorGUIUtility.TrTextContent("Workflow Mode",
                "Select a workflow that fits your textures. Choose between RMO or SMO.");
            /// <summary>
            /// The text and tooltip for the metallic Map GUI.
            /// </summary>
            public static GUIContent metallicMapText =
                EditorGUIUtility.TrTextContent("Metallic Map", "Sets and configures the map for the Metallic workflow.");

            /// <summary>
            /// The text and tooltip for the smoothness GUI.
            /// </summary>
            public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface.");

            /// <summary>
            /// The text and tooltip for the smoothness source GUI.
            /// </summary>
            public static GUIContent smoothnessMapChannelText =
                EditorGUIUtility.TrTextContent("Source",
                    "Specifies where to sample a smoothness map from. By default, uses the alpha channel for your map.");

            /// <summary>
            /// The text and tooltip for the specular Highlights GUI.
            /// </summary>
            public static GUIContent highlightsText = EditorGUIUtility.TrTextContent("Specular Highlights",
                "When enabled, the Material reflects the shine from direct lighting.");

            /// <summary>
            /// The text and tooltip for the environment Reflections GUI.
            /// </summary>
            public static GUIContent reflectionsText =
                EditorGUIUtility.TrTextContent("Environment Reflections",
                    "When enabled, the Material samples reflections from the nearest Reflection Probes or Lighting Probe.");

            /// <summary>
            /// The text and tooltip for the height map GUI.
            /// </summary>
            public static GUIContent heightMapText = EditorGUIUtility.TrTextContent("Height Map",
                "Defines a Height Map that will drive a parallax effect in the shader making the surface seem displaced.");

            /// <summary>
            /// The text and tooltip for the enabling/disabling clear coat GUI.
            /// </summary>
            public static GUIContent clearCoatText = EditorGUIUtility.TrTextContent("Clear Coat",
                "A multi-layer material feature which simulates a thin layer of coating on top of the surface material." +
                "\nPerformance cost is considerable as the specular component is evaluated twice, once per layer.");

            /// <summary>
            /// The text and tooltip for the clear coat Mask GUI.
            /// </summary>
            public static GUIContent clearCoatMaskText = EditorGUIUtility.TrTextContent("Mask",
                "Specifies the amount of the coat blending." +
                "\nActs as a multiplier of the clear coat map mask value or as a direct mask value if no map is specified." +
                "\nThe map specifies clear coat mask in the red channel and clear coat smoothness in the green channel.");

            /// <summary>
            /// The text and tooltip for the clear coat smoothness GUI.
            /// </summary>
            public static GUIContent clearCoatSmoothnessText = EditorGUIUtility.TrTextContent("Smoothness",
                "Specifies the smoothness of the coating." +
                "\nActs as a multiplier of the clear coat map smoothness value or as a direct smoothness value if no map is specified.");


            public static GUIContent outlineEnabledText = EditorGUIUtility.TrTextContent("Outline Enabled",
                "외곽선 효과 켜기/끄기");

            public static GUIContent outlineColorText = EditorGUIUtility.TrTextContent("Outline Color",
                "외곽선 색상");

            public static GUIContent outlineWidthText = EditorGUIUtility.TrTextContent("Outline Width",
                "외곽선 굵기");

            public static GUIContent outlineFadeStartText = EditorGUIUtility.TrTextContent("Outline Fade Start",
                "외곽선이 흐려지기 시작하는 거리");

            public static GUIContent outlineFadeEndText = EditorGUIUtility.TrTextContent("Outline Fade End",
                "외곽선이 완전히 사라지는 거리");
        }

        /// <summary>
        /// Container for the properties used in the <c>LitGUI</c> editor script.
        /// </summary>
        public struct LitProperties {
            // Surface Input Props

            /// <summary>
            /// The MaterialProperty for workflow mode.
            /// </summary>
            public MaterialProperty workflowMode;

            /// <summary>
            /// The MaterialProperty for metallic value.
            /// </summary>
            public MaterialProperty metallic;

            /// <summary>
            /// The MaterialProperty for metallic Smoothness map.
            /// </summary>
            public MaterialProperty metallicGlossMap;

            /// <summary>
            /// The MaterialProperty for smoothness value.
            /// </summary>
            public MaterialProperty smoothness;

            /// <summary>
            /// The MaterialProperty for normal map.
            /// </summary>
            public MaterialProperty bumpMapProp;

            /// <summary>
            /// The MaterialProperty for normal map scale.
            /// </summary>
            public MaterialProperty bumpScaleProp;

            /// <summary>
            /// The MaterialProperty for height map.
            /// </summary>
            public MaterialProperty parallaxMapProp;

            /// <summary>
            /// The MaterialProperty for height map scale.
            /// </summary>
            public MaterialProperty parallaxScaleProp;

            /// <summary>
            /// The MaterialProperty for occlusion strength.
            /// </summary>
            public MaterialProperty occlusionStrength;


            // Advanced Props

            /// <summary>
            /// The MaterialProperty for specular highlights.
            /// </summary>
            public MaterialProperty highlights;

            /// <summary>
            /// The MaterialProperty for environment reflections.
            /// </summary>
            public MaterialProperty reflections;

            /// <summary>
            /// The MaterialProperty for enabling/disabling clear coat.
            /// </summary>
            public MaterialProperty clearCoat;  // Enable/Disable dummy property

            /// <summary>
            /// The MaterialProperty for clear coat map.
            /// </summary>
            public MaterialProperty clearCoatMap;

            /// <summary>
            /// The MaterialProperty for clear coat mask.
            /// </summary>
            public MaterialProperty clearCoatMask;

            /// <summary>
            /// The MaterialProperty for clear coat smoothness.
            /// </summary>
            public MaterialProperty clearCoatSmoothness;

            public MaterialProperty outlineEnabled;

            public MaterialProperty outlineColor;

            public MaterialProperty outlineWidth;

            public MaterialProperty outlineFadeStart;

            public MaterialProperty outlineFadeEnd;

            /// <summary>
            /// Constructor for the <c>LitProperties</c> container struct.
            /// </summary>
            /// <param name="properties"></param>
            public LitProperties(MaterialProperty[] properties) {
                // Surface Option Props
                workflowMode = BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
                metallic = BaseShaderGUI.FindProperty("_Metallic", properties);
                metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties);
                smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
                bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
                bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);
                parallaxMapProp = BaseShaderGUI.FindProperty("_ParallaxMap", properties, false);
                parallaxScaleProp = BaseShaderGUI.FindProperty("_Parallax", properties, false);
                occlusionStrength = BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
                // Advanced Props
                highlights = BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
                reflections = BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);

                clearCoat = BaseShaderGUI.FindProperty("_ClearCoat", properties, false);
                clearCoatMap = BaseShaderGUI.FindProperty("_ClearCoatMap", properties, false);
                clearCoatMask = BaseShaderGUI.FindProperty("_ClearCoatMask", properties, false);
                clearCoatSmoothness = BaseShaderGUI.FindProperty("_ClearCoatSmoothness", properties, false);

                // Outlines
                outlineEnabled = BaseShaderGUI.FindProperty("_EnableOutline", properties, false);
                outlineColor = BaseShaderGUI.FindProperty("_OutlineColor", properties, false);
                outlineWidth = BaseShaderGUI.FindProperty("_OutlineWidth", properties, false);
                outlineFadeStart = BaseShaderGUI.FindProperty("_OutlineFadeStart", properties, false);
                outlineFadeEnd = BaseShaderGUI.FindProperty("_OutlineFadeEnd", properties, false);
            }
        }

        /// <summary>
        /// Draws the surface inputs GUI.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        public static void Inputs(LitProperties properties, MaterialEditor materialEditor, Material material) {
            DoMetallicSpecularArea(properties, materialEditor, material);
            BaseShaderGUI.DrawNormalArea(materialEditor, properties.bumpMapProp, properties.bumpScaleProp);

            if (HeightmapAvailable(material))
                DoHeightmapArea(properties, materialEditor);

            // Check that we have all the required properties for clear coat,
            // otherwise we will get null ref exception from MaterialEditor GUI helpers.
            if (ClearCoatAvailable(material))
                DoClearCoat(properties, materialEditor, material);
        }

        private static bool ClearCoatAvailable(Material material) {
            return material.HasProperty("_ClearCoat")
                && material.HasProperty("_ClearCoatMap")
                && material.HasProperty("_ClearCoatMask")
                && material.HasProperty("_ClearCoatSmoothness");
        }

        private static bool HeightmapAvailable(Material material) {
            return material.HasProperty("_Parallax")
                && material.HasProperty("_ParallaxMap");
        }

        private static void DoHeightmapArea(LitProperties properties, MaterialEditor materialEditor) {
            materialEditor.TexturePropertySingleLine(Styles.heightMapText, properties.parallaxMapProp,
                properties.parallaxMapProp.textureValue != null ? properties.parallaxScaleProp : null);
        }

        private static bool ClearCoatEnabled(Material material) {
            return material.HasProperty("_ClearCoat") && material.GetFloat("_ClearCoat") > 0.0;
        }

        /// <summary>
        /// Draws the clear coat GUI.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        public static void DoClearCoat(LitProperties properties, MaterialEditor materialEditor, Material material) {
            materialEditor.ShaderProperty(properties.clearCoat, Styles.clearCoatText);
            var coatEnabled = material.GetFloat("_ClearCoat") > 0.0;

            EditorGUI.BeginDisabledGroup(!coatEnabled);
            {
                EditorGUI.indentLevel += 2;
                materialEditor.TexturePropertySingleLine(Styles.clearCoatMaskText, properties.clearCoatMap, properties.clearCoatMask);

                // Texture and HDR color controls
                materialEditor.ShaderProperty(properties.clearCoatSmoothness, Styles.clearCoatSmoothnessText);

                EditorGUI.indentLevel -= 2;
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws the metallic/specular area GUI.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        public static void DoMetallicSpecularArea(LitProperties properties, MaterialEditor materialEditor, Material material) {
            bool hasGlossMap = false;
            materialEditor.TexturePropertySingleLine(Styles.metallicMapText, properties.metallicGlossMap,
                hasGlossMap ? null : properties.metallic);
            DoSmoothness(materialEditor, material, properties.smoothness);
        }

        internal static bool IsOpaque(Material material) {
            bool opaque = true;
            if (material.HasProperty("_Surface"))
                opaque = ((BaseShaderGUI.SurfaceType)material.GetFloat("_Surface") == BaseShaderGUI.SurfaceType.Opaque);
            return opaque;
        }

        /// <summary>
        /// Draws the smoothness GUI.
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        /// <param name="smoothness"></param>
        /// <param name="smoothnessMapChannel"></param>
        /// <param name="smoothnessChannelNames"></param>
        public static void DoSmoothness(MaterialEditor materialEditor, Material material, MaterialProperty smoothness) {
            EditorGUI.indentLevel += 2;

            materialEditor.ShaderProperty(smoothness, Styles.smoothnessText);
            EditorGUI.indentLevel -= 2;
        }

        // (shared by all lit shaders, including shadergraph Lit Target and Lit.shader)
        internal static void SetupSpecularWorkflowKeyword(Material material, out bool isRMOWorkFlow) {
            isRMOWorkFlow = true;     // default is RMO
            if (material.HasProperty("_WorkflowMode"))
                isRMOWorkFlow = ((WorkflowMode)material.GetFloat("_WorkflowMode")) == WorkflowMode.RMO;
            CoreUtils.SetKeyword(material, "_RMO_SETUP", isRMOWorkFlow);
        }

        /// <summary>
        /// Sets up the keywords for the Lit shader and material.
        /// </summary>
        /// <param name="material"></param>
        public static void SetMaterialKeywords(Material material) {
            SetupSpecularWorkflowKeyword(material, out bool isSpecularWorkFlow);

            var specularGlossMap = "_MetallicGlossMap";
            var hasGlossMap = material.GetTexture(specularGlossMap) != null;

            CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap);

            if (material.HasProperty("_SpecularHighlights"))
                CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                    material.GetFloat("_SpecularHighlights") == 0.0f);
            if (material.HasProperty("_EnvironmentReflections"))
                CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                    material.GetFloat("_EnvironmentReflections") == 0.0f);

            if (material.HasProperty("_ParallaxMap"))
                CoreUtils.SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));

            // Clear coat keywords are independent to remove possibility of invalid combinations.
            if (ClearCoatEnabled(material)) {
                var hasMap = material.HasProperty("_ClearCoatMap") && material.GetTexture("_ClearCoatMap") != null;
                if (hasMap) {
                    CoreUtils.SetKeyword(material, "_CLEARCOAT", false);
                    CoreUtils.SetKeyword(material, "_CLEARCOATMAP", true);
                }
                else {
                    CoreUtils.SetKeyword(material, "_CLEARCOAT", true);
                    CoreUtils.SetKeyword(material, "_CLEARCOATMAP", false);
                }
            }
            else {
                CoreUtils.SetKeyword(material, "_CLEARCOAT", false);
                CoreUtils.SetKeyword(material, "_CLEARCOATMAP", false);
            }
        }
    }
}
